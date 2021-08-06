using System;
using System.Collections;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Setting;
using UniRx;
using UnityEngine;

namespace TatemonSugoroku.Scripts.Akio
{
    public class MainGameManager : SMStandardMonoBehaviour
    {
        private bool _isAllowedInputMotion;
        
        private Subject<Unit> _movingPlayerTimer = new Subject<Unit>();
        protected override void StartAfterInitialize()
        {
            SMInputManager inputManager = SMServiceLocator.Resolve<SMInputManager>();
            AllModelManager allModelManager = AllModelManager.s_instance;
            MainGameManagementModel mainGameManagementModel = allModelManager.Get<MainGameManagementModel>();
            MotionModel motionModel = allModelManager.Get<MotionModel>();
            
            mainGameManagementModel.InitializeGame(2);

            mainGameManagementModel.GamePhase.Subscribe(phase =>
            {
                switch (phase)
                {
                    case MainGamePhase.WhileShowingTurnCall:
                        Debug.Log("ターン進行！ 1秒待機");
                        Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe(_ =>
                        {
                            mainGameManagementModel.NotifyShowingTurnCallFinished();
                        });
                        break;
                    case MainGamePhase.WaitingMovingPlayer:
                        break;
                    case MainGamePhase.PlayerCannotMove:
                        Debug.Log("プレイヤーはどのルートにも動けません");
                        break;
                    case MainGamePhase.WhileMovingPlayer:
                        Debug.Log("プレイヤー移動フェイズ");
                        mainGameManagementModel.MovePlayer();
                        StartCoroutine(CoroutineMovingPlayer());
                        break;
                    case MainGamePhase.WaitingPuttingTatemon:
                        Debug.Log("たてもんの選択フェイズ");
                        Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe(_ =>
                        {
                            mainGameManagementModel.NotifyInputPuttingTatemon();
                        });
                        break;
                    case MainGamePhase.WhilePuttingTatemon:
                        Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe(_ =>
                        {
                            mainGameManagementModel.NotifyPuttingTatemonFinished();
                        });
                        break;
                    case MainGamePhase.WhileCalculatingScore:
                        Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe(_ =>
                        {
                            mainGameManagementModel.NotifyCalculatingScoreFinished();
                        });
                        break;
                }

                if (phase == MainGamePhase.WaitingMovingPlayer)
                {
                    Debug.Log("移動可能");
                    _isAllowedInputMotion = true;
                }
                else
                {
                    _isAllowedInputMotion = false;
                }
            });

            _movingPlayerTimer.Subscribe(_ =>
            {
                if (mainGameManagementModel.HasDeterminedMotionPosition())
                {
                    mainGameManagementModel.MovePlayer();
                    StartCoroutine(CoroutineMovingPlayer());
                }
                else
                {
                    Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe(__ =>
                    {
                        mainGameManagementModel.NotifyMovingPlayerFinished();
                    });
                }
            });

            motionModel.NumberOfMovableCells.Subscribe(num =>
            {
                Debug.Log("移動できる回数：" + num);
            });
            
            inputManager._touchTileID
                .Where(_ => _isAllowedInputMotion)
                .Subscribe(cellId =>
                {
                    mainGameManagementModel.InputPosition(cellId);
                });
            
            Observable.Timer(TimeSpan.FromSeconds(5.0f)).Subscribe(_ =>
            {
                mainGameManagementModel.StartGame();
            });
        }

        IEnumerator CoroutineMovingPlayer()
        {
            yield return new WaitForSeconds(1.2f);
            _movingPlayerTimer.OnNext(Unit.Default);
        }
    }
}
