using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace TatemonSugoroku.Scripts
{
    public class ResultCanvas : MonoBehaviour
    {
        [SerializeField]
        SwitchableImage _Winner;

        [SerializeField]
        ResultScorePanel[] _ResultScores;

        [SerializeField]
        Button _Button;


        public async UniTask WaitForClick(IList<int> scores, int winner, CancellationToken ct = default)
        {
            try
            {
                gameObject.SetActive(true);
                _Winner.Switch(winner);
                for (int i = 0; i < scores.Count; i++)
                {
                    _ResultScores[i].Image.Switch(i);
                    _ResultScores[i].Score.SetText("{0}", scores[i]);
                }
                await _Button.OnClickAsync(ct);
            }
            finally
            {
                gameObject.SetActive(false);
            }
        }
    }
}
