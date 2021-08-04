using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using TatemonSugoroku.Gfshade.Extension;


namespace TatemonSugoroku.Scripts
{
    public class UICanvas : MonoBehaviour
    {
        [SerializeField]
        TMPro.TextMeshProUGUI _PlayerName;
        public TMPro.TextMeshProUGUI PlayerName => _PlayerName;

        [SerializeField]
        TMPro.TextMeshProUGUI _WalkRemaining;
        public TMPro.TextMeshProUGUI WalkRemaining => _WalkRemaining;

        [SerializeField]
        PlayerStatusPanel[] _PlayerStatusPanels;
        public PlayerStatusPanel[] PlayerStatusPanels => _PlayerStatusPanels;


        public void Initalize(IList<string> playerNames)
        {
            if (playerNames.Count != PlayerStatusPanels.Length)
            {
                throw new System.ArgumentException();
            }

            for (int i = 0; i < PlayerStatusPanels.Length; i++)
            {
                SetString(PlayerStatusPanels[i].Name, playerNames[i]);
            }
        }

        private void SetString(TMPro.TextMeshProUGUI tm, string str)
        {
            tm.SetText(str);
        }
        private void SetInt(TMPro.TextMeshProUGUI tm, int num)
        {
            tm.SetText("{0}", num);
        }

        public void SetCurrentPlayerName(string name)
        {
            PlayerName.SetText(name);
        }

        public void SetName(int playerId, string str)
        {
            SetString(PlayerStatusPanels[playerId].Name, str);
        }

        public void SetScore(int playerId, int num)
        {
            SetInt(PlayerStatusPanels[playerId].Score, num);
        }

        public void SetTatemon(int playerId, int num)
        {
            SetInt(PlayerStatusPanels[playerId].Tatemon, num);
        }


        public async UniTask ChangeScore(TMPro.TextMeshProUGUI tm, int oldScore, int newScore, CancellationToken ct = default)
        {
            await DOVirtual.Float(oldScore, newScore, 0.5f, num => tm.SetText("{0}", (int)num)).Play().WithCancellation(ct);
        }
        public UniTask ChangeScore(int playerId, int oldScore, int newScore, CancellationToken ct = default)
        {
            return ChangeScore(PlayerStatusPanels[playerId].Score, oldScore, newScore, ct);
        }

        public async UniTask ChangeTatemon(TMPro.TextMeshProUGUI tm, int oldTatemon, int newTatemon, CancellationToken ct = default)
        {
            Sequence seq = DOTween.Sequence()
                .Append(tm.transform.DOLocalMoveX(30f, 0.1f).SetEase(Ease.OutSine).SetRelative())
                .AppendCallback(() => tm.SetText("{0}", newTatemon))
                .Append(tm.transform.DOLocalMoveX(-30f, 0.1f).SetEase(Ease.InSine).SetRelative());
            await seq.Play().WithCancellation(ct);
        }
        public UniTask ChangeTatemon(int playerId, int oldTatemon, int newTatemon, CancellationToken ct = default)
        {
            return ChangeTatemon(PlayerStatusPanels[playerId].Tatemon, oldTatemon, newTatemon, ct);
        }
    }
}