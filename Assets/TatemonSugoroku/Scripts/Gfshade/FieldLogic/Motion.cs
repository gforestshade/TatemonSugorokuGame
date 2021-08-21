using System.Collections.Generic;
using UniRx;

namespace TatemonSugoroku.Scripts.FieldLogic
{
    /// <summary>
    /// 動けるかどうかの状態
    /// </summary>
    public enum MotionStatus
    {
        Unmovable,
        Movable,
        Return
    }
    
    public class Motion
    {
        private readonly int cellCount;
        private readonly int maxX;
        private readonly int maxY;
        
        // 出目
        private readonly int numberOfDice;
        // 移動前の座標
        private readonly int initialPosition;

        // マスごとの情報
        // そのマスに動けるならtrue, 動けないならfalse
        private readonly bool[] movableField;

        // 今移動中のマス
        private readonly List<int> movingList;
        // 次に動けるマス
        private readonly HashSet<int> movablePositions = new HashSet<int>();

        // 方向ごとに動けるかどうかの状態を持つ
        private readonly ReactiveProperty<MotionStatus> motionStatusUp;
        private readonly ReactiveProperty<MotionStatus> motionStatusDown;
        private readonly ReactiveProperty<MotionStatus> motionStatusLeft;
        private readonly ReactiveProperty<MotionStatus> motionStatusRight;

        // 方向ごとに動けるかどうか(公開用)
        public IReadOnlyReactiveProperty<MotionStatus> MotionStatusUp => motionStatusUp;
        public IReadOnlyReactiveProperty<MotionStatus> MotionStatusDown => motionStatusDown;
        public IReadOnlyReactiveProperty<MotionStatus> MotionStatusLeft => motionStatusLeft;
        public IReadOnlyReactiveProperty<MotionStatus> MotionStatusRight => motionStatusRight;

        // 残り歩数
        private readonly ReactiveProperty<int> numberOfMovableCells;
        public IReadOnlyReactiveProperty<int> NumberOfMovableCells => numberOfMovableCells;

        // 現在地
        private readonly ReactiveProperty<int> peekPosition;
        public IReadOnlyReactiveProperty<int> PeekPosition => peekPosition;


        private int CurrentPosition => movingList[movingList.Count - 1];
        private int PrevPosition => movingList.Count >= 2 ? movingList[movingList.Count - 2] : -1;


        public Motion(int playerId, int numberOfDice, Cell[] fieldCells, int currentPosition, int maxX, int maxY)
        {
            this.maxX = maxX;
            this.maxY = maxY;
            cellCount = maxX * maxY;
            this.numberOfDice = numberOfDice;
            initialPosition = currentPosition;

            movableField = new bool[cellCount];
            for (int i = 0; i < cellCount; i++)
            {
                int tid = fieldCells[i].TatemonPlayerId;
                movableField[i] = tid < 0 || tid == playerId;
            }

            movingList = new List<int>();
            movingList.Add(currentPosition);
            motionStatusUp = new ReactiveProperty<MotionStatus>(MotionStatus.Unmovable);
            motionStatusDown = new ReactiveProperty<MotionStatus>(MotionStatus.Unmovable);
            motionStatusLeft = new ReactiveProperty<MotionStatus>(MotionStatus.Unmovable);
            motionStatusRight = new ReactiveProperty<MotionStatus>(MotionStatus.Unmovable);
            numberOfMovableCells = new ReactiveProperty<int>(numberOfDice);
            peekPosition = new ReactiveProperty<int>(currentPosition);

            UpdateMotionStatus();
        }


        /// <summary>
        /// 移動先を末尾に増やす
        /// </summary>
        /// <param name="position"></param>
        public bool PushMotionPosition(int position)
        {
            if (PrevPosition == position)
            {
                movingList.RemoveAt(movingList.Count - 1);
            }
            else if (movablePositions.Contains(position))
            {
                movingList.Add(position);
            }
            else
            {
                return false;
            }
            
            UpdateMotionStatus();
            return true;
        }

        /// <summary>
        /// 方向ごとに動けるかどうかの状態を更新する
        /// </summary>
        private void UpdateMotionStatus()
        {
            movablePositions.Clear();
            
            int previousPosition = PrevPosition;
            int currentPosition = CurrentPosition;

            // Debug.Log(currentPosition);

            UpdateMotionStatusOneDirection(motionStatusUp, previousPosition, currentPosition, MoveDirection.Up);
            UpdateMotionStatusOneDirection(motionStatusDown, previousPosition, currentPosition, MoveDirection.Down);
            UpdateMotionStatusOneDirection(motionStatusLeft, previousPosition, currentPosition, MoveDirection.Left);
            UpdateMotionStatusOneDirection(motionStatusRight, previousPosition, currentPosition, MoveDirection.Right);

            numberOfMovableCells.Value = numberOfDice - movingList.Count + 1;
            peekPosition.Value = currentPosition;
        }

        private void UpdateMotionStatusOneDirection(ReactiveProperty<MotionStatus> rp, int previousPosition, int currentPosition, MoveDirection dir)
        {
            rp.Value = MotionStatus.Unmovable;
            int nextPosition = CellVector2.IdMove(currentPosition, dir, maxX, maxY);
            if (nextPosition >= 0)
            {
                if (previousPosition == nextPosition)
                {
                    rp.Value = MotionStatus.Return;
                }
                else if (movableField[nextPosition])
                {
                    movablePositions.Add(nextPosition);
                    rp.Value = MotionStatus.Movable;
                }
            }
        }


        public bool IsFinished()
        {
            return movingList.Count - 1 >= numberOfDice;
        }


        public Queue<int> GetMotionsAsQueue()
        {
            Queue<int> r = new Queue<int>();
            foreach (int position in movingList)
            {
                r.Enqueue(position);
            }

            return r;
        }
    }
}
