
namespace TatemonSugoroku
{
    /// <summary>
    /// どうあってもどこかで定義せざるを得ないルール上の定数を置いておく場所
    /// </summary>
    public class DifficultyManager : System.IDisposable
    {
        public Difficulty Diff { get; set; }

        public void Dispose()
        { }
    }
}
