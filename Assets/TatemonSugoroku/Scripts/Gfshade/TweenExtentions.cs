using DG.Tweening;

namespace TatemonSugoroku.Gfshade
{
    public static partial class Utilities
    {
        /// <summary>
        /// Tweenが実行中であれば中断する
        /// Tweenの実行が終わっていれば何もしない
        /// </summary>
        public static bool TweenKill(Tween tween, bool complete = false)
        {
            if (tween != null && tween.IsActive() && tween.IsPlaying())
            {
                tween.Kill(complete);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
