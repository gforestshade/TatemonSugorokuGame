using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace TatemonSugoroku.Scripts
{
    public class DiceCanvas : MonoBehaviour
    {
        [SerializeField]
        Image _PlayerName;

        [SerializeField]
        Sprite[] _PlayerNameSprites;

        [SerializeField]
        Button _Button;

        private void Awake()
        {
            //gameObject.SetActive(false);
        }

        public async UniTask WaitForClick(int playerId, CancellationToken ct = default)
        {
            try
            {
                gameObject.SetActive(true);
                _PlayerName.sprite = _PlayerNameSprites[playerId];
                await _Button.OnClickAsync(ct);
            }
            finally
            {
                gameObject.SetActive(false);
            }
        }
    }
}
