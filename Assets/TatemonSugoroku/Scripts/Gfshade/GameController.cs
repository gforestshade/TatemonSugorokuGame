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

            await StartEffect();

            int score0 = 0;
            int score1 = 0;
            int tatemon0 = 7;
            int tatemon1 = 7;

            for (int i = 0; i < 7; i++)
            {
                _UI.SetCurrentPlayerName(playerNames[0]);
                await UniTask.Delay(wait1);

                await _UI.ChangeTatemon(0, tatemon0, tatemon0 - 1);
                tatemon0--;
                await UniTask.Delay(wait1);

                _UI.SetCurrentPlayerName(playerNames[1]);
                await UniTask.Delay(wait1);

                await _UI.ChangeTatemon(1, tatemon1, tatemon1 - 1);
                tatemon1--;
                await UniTask.Delay(wait1);

                var changeScore0 = _UI.ChangeScore(0, score0, score0 + 10);
                var changeScore1 = _UI.ChangeScore(1, score1, score1 + 10);
                await UniTask.WhenAll(changeScore0, changeScore1);
                score0 += 10;
                score1 += 10;
                await UniTask.Delay(wait1);
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
            await UniTask.Delay(System.TimeSpan.FromSeconds(1));
        }

        private async UniTask EndEffect()
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(1));
        }

    }
}


