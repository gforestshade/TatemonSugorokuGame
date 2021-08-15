using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace TatemonSugoroku.Scripts.FieldLogic
{
    public enum MotionStatus
    {
        Unmovable,
        Movable,
        Return
    }
    
    public class Vertex2
    {
        public int V;
        public int BeforeV;
        public int Depth;
    }
    
    public class Motion
    {
        private readonly int cellCount;
        private readonly int maxX;
        private readonly int maxY;
        
        private int _numberOfDice;
        private int _initialPosition;

        // マスごとの情報
        // そのマスに動けるならtrue, 動けないならfalse
        private readonly bool[] _movableField;

        // 今移動中のマス
        private readonly List<int> _listPreparedMotions;
        private readonly HashSet<int> _movablePositions = new HashSet<int>();

        private readonly ReactiveProperty<MotionStatus> _motionStatusUp;
        private readonly ReactiveProperty<MotionStatus> _motionStatusDown;
        private readonly ReactiveProperty<MotionStatus> _motionStatusLeft;
        private readonly ReactiveProperty<MotionStatus> _motionStatusRight;

        public IReadOnlyReactiveProperty<MotionStatus> MotionStatusUp => _motionStatusUp;
        public IReadOnlyReactiveProperty<MotionStatus> MotionStatusDown => _motionStatusDown;
        public IReadOnlyReactiveProperty<MotionStatus> MotionStatusLeft => _motionStatusLeft;
        public IReadOnlyReactiveProperty<MotionStatus> MotionStatusRight => _motionStatusRight;

        private readonly ReactiveProperty<int> _numberOfMovableCells;
        public IReadOnlyReactiveProperty<int> NumberOfMovableCells => _numberOfMovableCells;

        private readonly ReactiveProperty<int> _peekPosition;
        public IReadOnlyReactiveProperty<int> PeekPosition => _peekPosition;

        public Motion(int playerId, int numberOfDice, Cell[] fieldCells, int currentPosition, int maxX, int maxY)
        {
            this.maxX = maxX;
            this.maxY = maxY;
            cellCount = maxX * maxY;
            _numberOfDice = numberOfDice;
            _initialPosition = currentPosition;

            _movableField = new bool[cellCount];
            for (int i = 0; i < cellCount; i++)
            {
                int tid = fieldCells[i].TatemonPlayerId;
                _movableField[i] = tid < 0 || tid == playerId;
            }

            _listPreparedMotions = new List<int>();
            _listPreparedMotions.Add(currentPosition);
            _motionStatusUp = new ReactiveProperty<MotionStatus>(MotionStatus.Unmovable);
            _motionStatusDown = new ReactiveProperty<MotionStatus>(MotionStatus.Unmovable);
            _motionStatusLeft = new ReactiveProperty<MotionStatus>(MotionStatus.Unmovable);
            _motionStatusRight = new ReactiveProperty<MotionStatus>(MotionStatus.Unmovable);
            _numberOfMovableCells = new ReactiveProperty<int>(numberOfDice);
            _peekPosition = new ReactiveProperty<int>(currentPosition);
        }

        public bool CanMoveAnyRoute()
        {
            // 隣接リスト形式でグラフを構築したい
            List<List<int>> graph = new List<List<int>>();
            for (int i = 0; i < cellCount; i++)
            {
                graph.Add(new List<int>());
            }

            // 全セルに対して
            for (int i = 0; i < cellCount; i++)
            {
                // 自分が通行不可なら何もしない
                if (!_movableField[i])
                {
                    continue;
                }

                // 自分と右が両方通行可能なら辺を張る
                if (i % maxX < maxX - 1) 
                {
                    int j = i + 1;
                    if (_movableField[j])
                    {
                        graph[i].Add(j);
                        graph[j].Add(i);
                    }
                }

                // 自分と下が両方通行可能なら辺を張る
                if (i / maxY < maxY - 1)
                {
                    int j = i + maxX;
                    if (_movableField[j])
                    {
                        graph[i].Add(j);
                        graph[j].Add(i);
                    }
                }
            }

            // DFSをしたいのでスタックを用意する           
            Stack<Vertex2> stack = new Stack<Vertex2>();

            // 原点は深さ0
            stack.Push(new Vertex2
            {
                V = _initialPosition,
                BeforeV = -1,
                Depth = 0
            });
            
            // すべてのルートを周りきるまで
            while (stack.Count > 0)
            {
                Vertex2 v = stack.Pop();

                if (v.Depth >= _numberOfDice)
                {
                    // 出目の分だけ移動できるルートがあった！
                    // 3頂点以上でループしている可能性があるので、
                    // 1つでもあれば即early return
                    return true;
                }

                // 自分との間に辺が張られているすべての頂点に対して
                foreach (int nv in graph[v.V])
                {
                    // 来た道は除外しつつ(2頂点間のループは考えない)
                    if (nv == v.BeforeV)
                    {
                        continue;
                    }

                    // 次の頂点をスタックに積む(深さを1増やす)
                    stack.Push(new Vertex2
                    {
                        V = nv,
                        BeforeV = v.V,
                        Depth = v.Depth + 1
                    });
                }
            }

            return false;
        }
        public void PushMotionPosition(int position)
        {
            if (_listPreparedMotions.Count >= 2)
            {
                if (_listPreparedMotions[_listPreparedMotions.Count - 2] == position)
                {
                    _listPreparedMotions.RemoveAt(_listPreparedMotions.Count - 1);
                }
                else if (_movablePositions.Contains(position))
                {
                    _listPreparedMotions.Add(position);
                }
            }
            else if (_listPreparedMotions.Count == 1)
            {
                if (_initialPosition == position)
                {
                    _listPreparedMotions.RemoveAt(_listPreparedMotions.Count - 1);
                }
                else if (_movablePositions.Contains(position))
                {
                    _listPreparedMotions.Add(position);
                }
            }
            else if (_movablePositions.Contains(position))
            {
                _listPreparedMotions.Add(position);
            }
            
            UpdateMotionStatus();
        }

        private void UpdateMotionStatus()
        {
            _movablePositions.Clear();

            _motionStatusUp.Value = MotionStatus.Unmovable;
            _motionStatusRight.Value = MotionStatus.Unmovable;
            _motionStatusDown.Value = MotionStatus.Unmovable;
            _motionStatusLeft.Value = MotionStatus.Unmovable;
            
            int previousPosition;
            int currentPosition;
            
            if (_listPreparedMotions.Count >= 2)
            {
                previousPosition = _listPreparedMotions[_listPreparedMotions.Count - 2];
                currentPosition = _listPreparedMotions[_listPreparedMotions.Count - 1];
            }
            else
            {
                previousPosition = -1;
                currentPosition = _initialPosition;
            }

            Debug.Log(currentPosition);
            
            if (currentPosition >= maxX)
            {
                int index = currentPosition - maxX;

                if (previousPosition == index)
                {
                    _motionStatusUp.Value = MotionStatus.Return;
                }
                else if (_movableField[index])
                {
                    _movablePositions.Add(index);
                    _motionStatusUp.Value = MotionStatus.Movable;
                }
            }

            if (currentPosition % maxX < maxX - 1)
            {
                int index = currentPosition + 1;

                if (previousPosition == index)
                {
                    _motionStatusRight.Value = MotionStatus.Return;
                }
                else if (_movableField[index])
                {
                    _movablePositions.Add(index);
                    _motionStatusRight.Value = MotionStatus.Movable;
                }
            }

            if (currentPosition / maxX < maxY - 1)
            {
                int index = currentPosition + maxX;

                if (previousPosition == index)
                {
                    _motionStatusDown.Value = MotionStatus.Return;
                }
                else if (_movableField[index])
                {
                    _movablePositions.Add(index);
                    _motionStatusDown.Value = MotionStatus.Movable;
                }
            }

            if (currentPosition % maxX > 0)
            {
                int index = currentPosition - 1;

                if (previousPosition == index)
                {
                    _motionStatusLeft.Value = MotionStatus.Return;
                }
                else if (_movableField[index])
                {
                    _movablePositions.Add(index);
                    _motionStatusLeft.Value = MotionStatus.Movable;
                }
            }

            _numberOfMovableCells.Value = _numberOfDice - _listPreparedMotions.Count + 1;

            _peekPosition.Value = _listPreparedMotions[_listPreparedMotions.Count - 1];
        }

        public bool IsFinished()
        {
            return _listPreparedMotions.Count - 1 >= _numberOfDice;
        }


        public Queue<int> GetMotionsAsQueue()
        {
            Queue<int> r = new Queue<int>();
            foreach (int position in _listPreparedMotions)
            {
                r.Enqueue(position);
            }

            return r;
        }
    }
}
