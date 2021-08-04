using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEditor;

namespace TatemonSugoroku.Scripts
{
    public enum MoveDirection
    {
        Up,
        Right,
        Down,
        Left
    }
    
    public class FieldCell
    {
        public int TatemonSpinPower;
        public int TatemonPlayerId;
        public int DomainPlayerId;
    }

    public class Vertex2
    {
        public int V;
        public int BeforeV;
        public int Depth;
    }
    public class FieldModel
    {
        private const int MAX_NUMBER_OF_CELLS = 64;
        private const int MAX_X_DIRECTION_OF_CELLS = 8;
        private const int MAX_Y_DIRECTION_OF_CELLS = 8;

        private MotionModel _motionModel;
        
        private readonly FieldCell[] _fieldCells = new FieldCell[MAX_NUMBER_OF_CELLS];
        private readonly List<int> _playerPosition = new List<int>();
        
        private readonly Subject<int[]> _tatemonSpinPowerInformation = new Subject<int[]>();
        private readonly Subject<int[]> _tatemonInformation = new Subject<int[]>();
        private readonly Subject<int[]> _domainInformation = new Subject<int[]>();
        public IObservable<int[]> TatemonSpinPowerInformation => _tatemonSpinPowerInformation;
        public IObservable<int[]> TatemonInformation => _tatemonInformation;
        public IObservable<int[]> DomainInformation => _domainInformation;
        
        public FieldModel()
        {
            for (int i = 0; i < MAX_NUMBER_OF_CELLS; i++)
            {
                _fieldCells[i] = new FieldCell
                {
                    TatemonSpinPower = 0,
                    TatemonPlayerId = -1,
                    DomainPlayerId = -1
                };
            }
        }

        public void SetMotionModel(MotionModel motionModel)
        {
            _motionModel = motionModel;
        }
        
        public void Initialize(int numberOfPlayers)
        {
            _playerPosition.Clear();
            foreach (FieldCell fieldCell in _fieldCells)
            {
                fieldCell.TatemonSpinPower = 0;
                fieldCell.TatemonPlayerId = -1;
                fieldCell.DomainPlayerId = -1;
            }

            switch (numberOfPlayers)
            {
                case 2:
                    _fieldCells[0].DomainPlayerId = 0;
                    _fieldCells[63].DomainPlayerId = 1;
                    
                    _playerPosition.Add(0);
                    _playerPosition.Add(63);
                    break;
            }

            int[] tatemonSpinPowerInformation = new int[MAX_NUMBER_OF_CELLS];
            int[] tatemonInformation = new int[MAX_NUMBER_OF_CELLS];
            int[] domainInformation = new int[MAX_NUMBER_OF_CELLS];
            for (int i = 0; i < MAX_NUMBER_OF_CELLS; i++)
            {
                FieldCell fieldCell = _fieldCells[i];
                tatemonSpinPowerInformation[i] = fieldCell.TatemonSpinPower;
                tatemonInformation[i] = fieldCell.TatemonPlayerId;
                domainInformation[i] = fieldCell.DomainPlayerId;
            }
            _tatemonSpinPowerInformation.OnNext(tatemonSpinPowerInformation);
            _tatemonInformation.OnNext(tatemonInformation);
            _domainInformation.OnNext(domainInformation);
        }

        public bool InspectPlayerCanMove(int playerId, int numberOfDice)
        {
            List<List<int>> graph = new List<List<int>>();
            for (int i = 0; i < 64; i++)
            {
                graph.Add(new List<int>());
            }

            for (int i = 0; i < 64; i++)
            {
                if (_fieldCells[i].TatemonPlayerId >= 0 && _fieldCells[i].TatemonPlayerId != playerId)
                {
                    continue;
                }

                if (i % MAX_X_DIRECTION_OF_CELLS < MAX_X_DIRECTION_OF_CELLS - 1) 
                {
                    int j = i + 1;
                    if (_fieldCells[j].TatemonPlayerId < 0 || _fieldCells[j].TatemonPlayerId == playerId)
                    {
                        graph[i].Add(j);
                        graph[j].Add(i);
                    }
                }

                if (i / MAX_Y_DIRECTION_OF_CELLS < MAX_Y_DIRECTION_OF_CELLS - 1)
                {
                    int j = i + MAX_X_DIRECTION_OF_CELLS;
                    if (_fieldCells[j].TatemonPlayerId < 0 || _fieldCells[j].TatemonPlayerId == playerId)
                    {
                        graph[i].Add(j);
                        graph[j].Add(i);
                    }
                }
            }

            HashSet<int> hashSetPositions = new HashSet<int>();
            
            Stack<Vertex2> stack = new Stack<Vertex2>();

            stack.Push(new Vertex2
            {
                V = _playerPosition[playerId],
                BeforeV = -1,
                Depth = 0
            });
            
            while (stack.Count > 0)
            {
                Vertex2 v = stack.Pop();

                if (v.Depth >= numberOfDice)
                {
                    hashSetPositions.Add(v.V);
                    continue;
                }

                foreach (int nv in graph[v.V])
                {
                    if (nv == v.BeforeV)
                    {
                        continue;
                    }

                    stack.Push(new Vertex2
                    {
                        V = nv,
                        BeforeV = v.V,
                        Depth = v.Depth + 1
                    });
                }
            }

            return hashSetPositions.Count > 0;
        }

        public void MovePlayer(int playerId, MoveDirection direction)
        {
            int moveTo = -1;
            int currentPosition = _playerPosition[playerId];
            switch (direction)
            {
                case MoveDirection.Up:
                    if (currentPosition >= MAX_X_DIRECTION_OF_CELLS)
                    {
                        moveTo = currentPosition - MAX_X_DIRECTION_OF_CELLS;
                    }
                    
                    break;
                case MoveDirection.Right:
                    if (currentPosition % MAX_X_DIRECTION_OF_CELLS < MAX_X_DIRECTION_OF_CELLS - 1)
                    {
                        moveTo = currentPosition + 1;
                    }
                    break;
                case MoveDirection.Down:
                    if (currentPosition / MAX_X_DIRECTION_OF_CELLS < MAX_Y_DIRECTION_OF_CELLS - 1)
                    {
                        moveTo = currentPosition + MAX_X_DIRECTION_OF_CELLS;
                    }
                    break;
                case MoveDirection.Left:
                    if (currentPosition % MAX_X_DIRECTION_OF_CELLS > 0)
                    {
                        moveTo = currentPosition - 1;
                    }
                    break;
            }

            MovePlayer(playerId, moveTo);
        }

        public void MovePlayer(int playerId, int moveTo)
        {
            if (moveTo < 0 || moveTo >= MAX_NUMBER_OF_CELLS)
            {
                return;
            }

            _playerPosition[playerId] = moveTo;
        }

        public void PutTatemonAtCurrentPosition(int playerId, int spinPower)
        {
            int currentPosition = _playerPosition[playerId];
            _fieldCells[currentPosition].TatemonPlayerId = playerId;
            _fieldCells[currentPosition].TatemonSpinPower = spinPower;
        }

        public void DoubleTatemonSpinPower()
        {
            for (int i = 0; i < MAX_NUMBER_OF_CELLS; i++)
            {
                _fieldCells[i].TatemonSpinPower *= 2;
            }
        }

        public int GetMovableDirections(int playerId, IList<int> vertexes)
        {
            bool canMoveUp = false;
            bool canMoveRight = false;
            bool canMoveDown = false;
            bool canMoveLeft = false;
            
            List<int> positions = new List<int>();

            positions.Add(_playerPosition[playerId]);

            foreach (int vertex in vertexes)
            {
                positions.Add(vertex);
            }

            int currentPosition = positions[positions.Count - 1];
            int previousPosition = -1;
            if (positions.Count >= 2)
            {
                previousPosition = positions[positions.Count - 2];
            }

            if (currentPosition - MAX_X_DIRECTION_OF_CELLS != previousPosition &&
                currentPosition >= MAX_X_DIRECTION_OF_CELLS)
            {
                int toPosition = currentPosition - MAX_X_DIRECTION_OF_CELLS;
                if (_fieldCells[toPosition].TatemonPlayerId < 0 || _fieldCells[toPosition].TatemonPlayerId == playerId)
                {
                    canMoveUp = true;
                }
            }

            if (currentPosition + 1 != previousPosition &&
                currentPosition % MAX_X_DIRECTION_OF_CELLS < MAX_X_DIRECTION_OF_CELLS - 1)
            {
                int toPosition = currentPosition + 1;
                if (_fieldCells[toPosition].TatemonPlayerId < 0 || _fieldCells[toPosition].TatemonPlayerId == playerId)
                {
                    canMoveRight = true;
                }
            }

            if (currentPosition + MAX_X_DIRECTION_OF_CELLS != previousPosition &&
                currentPosition / MAX_X_DIRECTION_OF_CELLS < MAX_Y_DIRECTION_OF_CELLS - 1)
            {
                int toPosition = currentPosition + MAX_X_DIRECTION_OF_CELLS;
                if (_fieldCells[toPosition].TatemonPlayerId < 0 || _fieldCells[toPosition].TatemonPlayerId == playerId)
                {
                    canMoveDown = true;
                }
            }

            if (currentPosition - 1 != previousPosition &&
                currentPosition % MAX_X_DIRECTION_OF_CELLS > 0)
            {
                int toPosition = currentPosition - 1;
                if (_fieldCells[toPosition].TatemonPlayerId < 0 || _fieldCells[toPosition].TatemonPlayerId == playerId)
                {
                    canMoveLeft = true;
                }
            }
            int r = 0;
            if (canMoveUp) r += 1;
            if (canMoveRight) r += 2;
            if (canMoveDown) r += 4;
            if (canMoveLeft) r += 8;
            return r;
        }

        public FieldCell[] GetFieldCells()
        {
            return _fieldCells;
        }
    }
}
