namespace TatemonSugoroku.Scripts.FieldLogic
{
    public enum MoveDirection
    {
        Up,
        Right,
        Down,
        Left
    }

    struct CellVector2
    {
        public int X;
        public int Y;
        public CellVector2(int x, int y)
        {
            X = x;
            Y = y;
        }
        public static CellVector2? FromCellId(int id, int maxX, int maxY)
        {
            if (id < 0 || id >= maxX * maxY)
                return null;
            else
                return new CellVector2 { X = id % maxX, Y = id / maxX };
        }
        public int ToCellId(int maxX, int maxY)
        {
            if (!(X >= 0 && X < maxX && Y >= 0 && Y < maxY))
                return -1;
            else
                return Y * maxX + X;
        }
        public static CellVector2 MovePeekToDirection(CellVector2 peek, MoveDirection dir)
        {
            switch (dir)
            {
                case MoveDirection.Up:
                    return new CellVector2(peek.X, peek.Y-1);
                case MoveDirection.Right:
                    return new CellVector2(peek.X+1, peek.Y);
                case MoveDirection.Down:
                    return new CellVector2(peek.X, peek.Y+1);
                case MoveDirection.Left:
                    return new CellVector2(peek.X-1, peek.Y);
                default:
                    throw new System.ArgumentException();
            }
        }
        public static int IdMove(int id, MoveDirection dir, int maxX, int maxY)
        {
            CellVector2? pos = FromCellId(id, maxX, maxY);
            if (!pos.HasValue) return -1;

            CellVector2 next = MovePeekToDirection(pos.Value, dir);
            return next.ToCellId(maxX, maxY);
        }
    }
    
    public struct MoveResult
    {
        public bool IsOppositeEnter { get; private set; }
        public int oppositePlayerId { get; private set; }

        public static MoveResult None => new MoveResult { IsOppositeEnter = false };
        public static MoveResult OppositeCellEntered(int id) => new MoveResult { IsOppositeEnter = true, oppositePlayerId = id };
    }

    public class Cell
    {
        public int TatemonSpinPower;
        public int TatemonPlayerId;
        public int DomainPlayerId;
    }


    public class Field
    {
        // 盤面の大きさ
        public readonly int maxX;
        public readonly int maxY;
        private readonly int cellCount;

        // 盤面の中身
        private readonly Cell[] cells;
        // プレイヤーの位置
        private readonly int[] playerPositions;

        public int CellCount => cellCount;
        public int PlayerCount => playerPositions.Length;

        public Field(int maxX, int maxY, int[] playerPositionList)
        {
            this.maxX = maxX;
            this.maxY = maxY;
            this.cellCount = maxX * maxY;

            cells = new Cell[cellCount];
            for (int i = 0; i < cellCount; i++)
            {
                cells[i] = new Cell
                {
                    TatemonSpinPower = 0,
                    TatemonPlayerId = -1,
                    DomainPlayerId = -1,
                };
            }

            playerPositions = new int[playerPositionList.Length];
            for (int i = 0; i < playerPositionList.Length; i++)
            {
                int posId = playerPositionList[i];
                playerPositions[i] = posId;
                cells[posId].DomainPlayerId = i;
            }
        }

        public Field(Field field)
        {
            maxX = field.maxX;
            maxY = field.maxY;
            cellCount = field.cellCount;

            cells = new Cell[cellCount];
            for (int i = 0; i < cellCount; i++)
            {
                Cell c = field.cells[i];
                cells[i] = new Cell
                {
                    TatemonSpinPower = c.TatemonSpinPower,
                    TatemonPlayerId = c.TatemonPlayerId,
                    DomainPlayerId = c.DomainPlayerId,
                };
            }

            playerPositions = new int[field.playerPositions.Length];
            for (int i = 0; i < field.playerPositions.Length; i++)
            {
                int posId = field.playerPositions[i];
                playerPositions[i] = posId;
                cells[posId].DomainPlayerId = i;
            }
        }

        public bool CanMove(int playerId, int moveTo)
        {
            int tid = cells[moveTo].TatemonPlayerId;
            return tid < 0 || tid == playerId;
        }

        public MoveResult MovePlayer(int playerId, int moveTo)
        {
            if (moveTo < 0 || moveTo >= cellCount)
            {
                return MoveResult.None;
            }
            
            playerPositions[playerId] = moveTo;
            int previousDomainPlayerId = cells[moveTo].DomainPlayerId;

            cells[moveTo].DomainPlayerId = playerId;
            
            int[] domainInformation = new int[cellCount];
            for (int i = 0; i < cellCount; i++)
            {
                Cell fieldCell = cells[i];
                domainInformation[i] = fieldCell.DomainPlayerId;
            }
            
            if (previousDomainPlayerId >= 0 && previousDomainPlayerId != playerId)
            {
                return MoveResult.OppositeCellEntered(previousDomainPlayerId);
            }
            
            return MoveResult.None;
        }

        public void PutTatemonAtCurrentPosition(int playerId, int spinPower)
        {
            int currentPosition = playerPositions[playerId];
            cells[currentPosition].TatemonPlayerId = playerId;
            cells[currentPosition].TatemonSpinPower = spinPower;
        }

        public Motion CreateMotion(int playerId, int numberOfDice)
        {
            return new Motion(playerId, numberOfDice, cells, playerPositions[playerId], maxX, maxY);
        }
        
        public Cell[] GetFieldCells()
        {
            return cells;
        }

        public int Domain(int id) => cells[id].DomainPlayerId;

        public int Tatemon(int id) => cells[id].TatemonPlayerId;

        public int SpinPower(int id) => cells[id].TatemonSpinPower;

        public int GetCurrentPositionByPlayerId(int playerId)
        {
            return playerPositions[playerId];
        }
    }
}
