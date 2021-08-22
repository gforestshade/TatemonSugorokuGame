using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TatemonSugoroku.Scripts;



namespace TatemonSugoroku.Gfshade
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        UICanvas _UI;

        [SerializeField]
        DiceCanvas _DiceUI;

        [SerializeField]
        ResultCanvas _ResultUI;


        private readonly string[] playerNames = new string[] {"たてお", "たてこ"};

        readonly CancellationTokenSource _cancelSource = new CancellationTokenSource();

        private async UniTask Start()
        {
            var ct = _cancelSource.Token;

            InitUI();
            System.TimeSpan wait1 = System.TimeSpan.FromSeconds(0.2);
            System.TimeSpan wait2 = System.TimeSpan.FromSeconds(1.0);

            // フェードをoptputする方法はないのだろうか
            await StartEffect(ct);

            int[] scores = new int[] {0, 0};
            int[] tatemons = new int[] {7, 7};

            await _ResultUI.WaitForClick(new int[] {500, 700}, 1, ct);

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    await _DiceUI.WaitForClick(j, ct);
                    int dice = Random.Range(1, 6) + Random.Range(1, 6);

                    _UI.SetWalkRemaining(dice);
                    await UniTask.Delay(wait1, cancellationToken: ct);
                    for (dice = dice - 1; dice >= 0; dice--)
                    {
                        await _UI.ChangeWalkRemaining(dice+1, dice, 0.5f, ct);
                        await UniTask.Delay(wait1, cancellationToken: ct );
                    }

                    _UI.HideWalkRemaining();
                    await UniTask.Delay(wait1, cancellationToken: ct );

                    await _UI.ChangeTatemon(j, tatemons[j], tatemons[j] - 1, 0.1f, ct);
                    tatemons[j]--;
                    await UniTask.Delay(wait1, cancellationToken: ct );
                }

                List<UniTask> changeScore = new List<UniTask>();
                for (int j = 0; j < 2; j++)
                {
                    changeScore.Add(_UI.ChangeScore(j, scores[j], scores[j] + 10, 0.5f, ct));
                }
                await UniTask.WhenAll(changeScore);
                for (int j = 0; j < 2; j++)
                {
                    scores[j] += 10;
                }

                await UniTask.Delay(wait2, cancellationToken: ct );
            }
            await EndEffect(ct);
        }

        private void InitUI()
        {
            _UI.SetScore(0, 0);
            _UI.SetScore(1, 0);
            _UI.SetTatemon(0, 7);
            _UI.SetTatemon(1, 7);
        }

        private async UniTask StartEffect(CancellationToken ct)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(3), cancellationToken: ct);
        }

        private async UniTask EndEffect(CancellationToken ct)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(1), cancellationToken: ct);
        }


		private void OnDestroy() {
            _cancelSource.Cancel();
            _cancelSource.Dispose();
        }
	}
}


