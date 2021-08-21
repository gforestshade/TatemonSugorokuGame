
namespace TatemonSugoroku
{
    /// <summary>
    /// ルール上の定数
    /// </summary>
    public class Difficulty
    {
        /// <summary>マスのX方向の幅</summary>
        public readonly int maxX;
        
        /// <summary>マスのY方向の幅</summary>
        public readonly int maxY;
        
        /// <summary>ターン数</summary>
        public readonly int maxTurn;
        
        /// <summary>フィーバーに入るターン数</summary>
        public readonly int feverTurn;

        /// <summary>敵マス踏むと相手に入るポイント</summary>
        public readonly int oppositeEnterBonus;

        /// <summary>回転数</summary>
        public readonly ISpinPower spinPower;

        /// <summary>フィーバー中の回転数</summary>
        public readonly ISpinPower feverSpinPower;

        public Difficulty(int maxX, int maxY, int maxTurn, int feverTurn, int oppositeEnterBonus, ISpinPower spinPower, ISpinPower feverSpinPower)
        {
            this.maxX = maxX;
            this.maxY = maxY;
            this.maxTurn = maxTurn;
            this.feverTurn = feverTurn;
            this.oppositeEnterBonus = oppositeEnterBonus;
            this.spinPower = spinPower;
            this.feverSpinPower = feverSpinPower;
        }

        public int GetSpinPower(int turnIndex, int dice)
        {
            if (turnIndex < feverTurn)
            {
                return spinPower[dice];
            }
            else
            {
                return feverSpinPower[dice];
            }
        }

        /// <summary>
        /// 標準設定
        /// </summary>
        public static Difficulty Normal =>
            new Difficulty(
                maxX: 8,
                maxY: 8, 
                maxTurn: 7,
                feverTurn: 7 - 2,
                oppositeEnterBonus: 20,
                spinPower: SpinPowerNormal.Instance,
                feverSpinPower: new SpinPowerDouble(SpinPowerNormal.Instance, 2));

        /// <summary>
        /// セブン設定
        /// </summary>
        public static Difficulty Seven =>
            new Difficulty(
                maxX: 7,
                maxY: 7,
                maxTurn: 7,
                feverTurn: 7 - 1,
                oppositeEnterBonus: 20,
                spinPower: SpinPowerNormal.Instance,
                feverSpinPower: new SpinPowerDouble(SpinPowerNormal.Instance, 2));
    }

    /// <summary>
    /// 回転力を細かく定義できるやつ
    /// インデクサで出目に対する回転力を返すこと
    /// </summary>
    public interface ISpinPower
    {
        int this[int index] { get; }
    }


    /// <summary>
    /// 固定で標準値を返すやつ
    /// </summary>
    public class SpinPowerNormal : ISpinPower
    {
        public int this[int index] => 
            index switch
            {
                2 => 6,
                3 => 5,
                4 => 5,
                5 => 4,
                6 => 4,
                7 => 3,
                8 => 3,
                9 => 2,
                10 => 2,
                11 => 1,
                12 => 1,
                _ => 0
            };

        // 標準設定
        public static readonly SpinPowerNormal Instance = new SpinPowerNormal(); 
    }

    /// <summary>
    /// 倍にするやつ
    /// </summary>
    public class SpinPowerDouble : ISpinPower
    {
        ISpinPower basePower;
        int multiplier;

        public SpinPowerDouble(ISpinPower basePower, int multiplier)
        {
            this.basePower = basePower;
            this.multiplier = multiplier;
        }
        public int this[int index] => basePower[index] * multiplier;
    }
}
