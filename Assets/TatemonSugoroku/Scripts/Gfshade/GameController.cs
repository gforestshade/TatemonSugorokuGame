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


        private readonly string[] playerNames = new string[] {"たてお", "たてこ"};

        private async UniTask Start()
        {
            InitUI();
            System.TimeSpan wait1 = System.TimeSpan.FromSeconds(0.2);
            System.TimeSpan wait2 = System.TimeSpan.FromSeconds(1.0);

            // フェードをoptputする方法はないのだろうか
            await StartEffect();

            int[] scores = new int[] {0, 0};
            int[] tatemons = new int[] {7, 7};

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    _UI.SetCurrentPlayerName(playerNames[j]);
                    await UniTask.Delay(wait1);

                    int dice = Random.Range(1, 6) + Random.Range(1, 6);

                    for (; dice >= 0; dice--)
                    {
                        _UI.SetWalkRemaining(dice);
                        await UniTask.Delay(wait1);
                    }

                    await _UI.ChangeTatemon(j, tatemons[j], tatemons[j] - 1);
                    tatemons[j]--;
                    await UniTask.Delay(wait1);
                }

                List<UniTask> changeScore = new List<UniTask>();
                for (int j = 0; j < 2; j++)
                {
                    changeScore.Add(_UI.ChangeScore(j, scores[j], scores[j] + 10));
                }
                await UniTask.WhenAll(changeScore);
                for (int j = 0; j < 2; j++)
                {
                    scores[j] += 10;
                }
                await UniTask.Delay(wait2);
            }

            await EndEffect();
        }

        private void InitUI()
        {
            _UI.Initalize(playerNames);
            _UI.SetScore(0, 0);
            _UI.SetScore(1, 0);
            _UI.SetTatemon(0, 7);
            _UI.SetTatemon(1, 7);
        }

        private async UniTask StartEffect()
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(5));
        }

        private async UniTask EndEffect()
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(1));
        }

    }
}


