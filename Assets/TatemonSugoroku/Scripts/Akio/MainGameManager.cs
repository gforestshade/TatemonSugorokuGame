using System.Collections.Generic;
using System.Linq;
using SubmarineMirage.Base;
using SubmarineMirage.Debug;
using SubmarineMirage.Service;
using SubmarineMirage.Setting;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;


namespace TatemonSugoroku.Scripts.Akio
{
    class PlayerInternalModel
    {
        public int Id;
        public string Name;
        public int Score;
        public int OppositeEnterBonus;
        public int Tatemon;
        public int MaxTatemon;
        public int TileId;
    }

    // Gfshade: 大改編
    public class MainGameManager : SMStandardMonoBehaviour
    {
        [SerializeField]
        private UICanvas _UI;

        [SerializeField]
        private DiceManagerView _DiceManager;

        [SerializeField]
        private TileManagerView _TileManager;

        [SerializeField]
        private PieceManagerView _PieceManager;

        [SerializeField]
        private MoveArrowManagerView _MoveArrowManager;

        [SerializeField]
        private TatemonManagerView _TatemonManager;


        private readonly int[] _SpinPowersOfTatemon = { 0, 0, 6, 5, 5, 4, 4, 3, 3, 2, 2, 1, 1 };
        private readonly int _OppositeEnterBonusOne = 20;

        private FieldModel fieldModel;
        private MotionModel motionModel;
        SMInputManager inputManager;
        PlayerInternalModel[] playerModels;

        private async UniTask DoGame()
        {
            InitTestPlayerInternalModels();
            inputManager = SMServiceLocator.Resolve<SMInputManager>();

            fieldModel = new FieldModel();
            fieldModel.InitializeGame(playerModels.Length);

            motionModel = new MotionModel();
            fieldModel.InitializeGame(playerModels.Length);

            for (int i = 0; i < 2; i++)
            {
                _TileManager.ChangeArea(playerModels[i].TileId, i);
            }

            InitUI();
            System.TimeSpan wait02 = System.TimeSpan.FromSeconds(0.2);
            System.TimeSpan wait10 = System.TimeSpan.FromSeconds(1.0);

            // げーむがはじまるよ
            await GameStart();

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    bool turnResult = await DoPlayerTurn(i, j);
                    if (!turnResult) goto GameEnd;
                }

                // 「2人のたてもんが置かれたとき、さらに効果発動！
                // 　スコアが再計算されるッ！」
                await UpdateScore();

                // じかんがすすむよ
                // Day.UpdateHour();

                // スコア更新時の時間調整だよ
                await UniTask.Delay(wait10);
            }

            GameEnd:
            // げーむがおわったよ
            await GameEnd();
        }

        private async UniTask<bool> DoPlayerTurn(int turnIndex, int playerId)
        {
            System.TimeSpan wait02 = System.TimeSpan.FromSeconds(0.2);
            System.TimeSpan wait10 = System.TimeSpan.FromSeconds(1.0);

            PlayerInternalModel pTurn = playerModels[playerId];

            // 「たておのターン！」
            await TurnStart(pTurn);

            // 「ドロー！」
            int dice = await DoDice();

            // 「オレは、12を召喚！」
            _UI.SetWalkRemaining(dice);
            await UniTask.Delay(wait02);


            // 「フィールドを、好きな方向に12回まで塗りつぶすことができるッ！」

            // MotionModelを初期化する
            if (!ResetMotionModel(motionModel, fieldModel, playerId, dice))
            {
                // 初期化時、囲まれてて移動できないならfalseが返る。
                // そうなったらゲーム終了
                return false;
            }

            await InputAndMove(playerId);

            // 「さらに！　塗りつぶしが終了したとき、効果発動！」
            _MoveArrowManager.Hide();
            _UI.HideWalkRemaining();
            await UniTask.Delay(wait02);

            // 「オレの最終位置に、設置魔法「たてもん」が発動するぜ！」
            fieldModel.PutTatemonAtCurrentPosition(playerId, _SpinPowersOfTatemon[dice]);
            _TatemonManager.Place(playerId, pTurn.TileId, _SpinPowersOfTatemon[dice]);
            await UniTask.Delay(wait02);
            await _UI.ChangeTatemon(playerId, pTurn.Tatemon, pTurn.Tatemon - 1);
            playerModels[playerId].Tatemon -= 1;

            // ターン終了時の時間調整だよ
            await UniTask.Delay(wait10);

            return true;
        }

        private void InitTestPlayerInternalModels()
        {
            playerModels = new PlayerInternalModel[]
            {
                new PlayerInternalModel
                {
                    Id = 0,
                    Name = "たてた",
                    Score = 0,
                    OppositeEnterBonus = 0,
                    Tatemon = 7,
                    MaxTatemon = 7,
                    TileId = 0,
                },
                new PlayerInternalModel
                {
                    Id = 1,
                    Name = "たてて",
                    Score = 0,
                    OppositeEnterBonus = 0,
                    Tatemon = 7,
                    MaxTatemon = 7,
                    TileId = 63,
                },
            };
        }

        private void InitUI()
        {
            _UI.Initalize(playerModels.Select(info => info.Name).ToList());
            for (int i = 0; i < playerModels.Length; i++)
            {
                _UI.SetScore(i, playerModels[i].Score);
                _UI.SetTatemon(i, playerModels[i].Tatemon);
            }
        }

        private async UniTask GameStart()
        {
            SMLog.Debug($"ゲーム開始", SMLogTag.Scene);
            //await UniTask.Delay(System.TimeSpan.FromSeconds(5));
        }

        private async UniTask GameEnd()
        {
            SMLog.Debug($"ゲーム終了", SMLogTag.Scene);
            await UniTask.Delay(System.TimeSpan.FromSeconds(1));
        }

        private async UniTask TurnStart(PlayerInternalModel model)
        {
            SMLog.Debug($"{model.Name}のターン", SMLogTag.Scene);
            _UI.SetCurrentPlayerName(model.Name);
            await UniTask.Delay(System.TimeSpan.FromSeconds(1));
        }

        private async UniTask<int> DoDice()
        {
            SMLog.Debug($"サイコロボタンを押してください", SMLogTag.Scene);
            int dice = await _DiceManager.Roll();
            
            SMLog.Debug($"{dice}が出ました", SMLogTag.Scene);
            return dice;
        }

        private async UniTask InputAndMove(int playerId)
        {
            PlayerInternalModel pTurn = playerModels[playerId];

            // 例外が飛んだ場合は購読していたという事実は無かったことになる
            using (CompositeDisposable movementDisposables = new CompositeDisposable())
            {
                // 移動ルート受け取り用のQueueとSubjectを用意
                Subject<Queue<int>> motionQueueSubject = new Subject<Queue<int>>();
                Queue<int> motions;

                // 触った場所を購読して、MotionModelに流す
                // 最終結果は移動ルート受け取り用Subjectに向かってぷっしゅしてもらう
                inputManager
                    ._touchTileID
                    .Subscribe(tileId => InputPosition(motionQueueSubject, motionModel, tileId))
                    .AddTo(movementDisposables);

                // コマ移動
                fieldModel
                    .PlayerPositions
                    .Select(pp => pp[playerId])
                    .Subscribe(tileId => GoastPieceMove(playerId, tileId))
                    .AddTo(movementDisposables);

                // まず(矢印方向, 矢印状態)のぺあにおいて変化が全部送られてくるストリームをつくる
                var arrowUp = motionModel.MotionStatusUp.Select(status => new KeyValuePair<MoveArrowType, MotionStatus>(MoveArrowType.Up, status));
                var arrowRight = motionModel.MotionStatusRight.Select(status => new KeyValuePair<MoveArrowType, MotionStatus>(MoveArrowType.Right, status));
                var arrowDown = motionModel.MotionStatusDown.Select(status => new KeyValuePair<MoveArrowType, MotionStatus>(MoveArrowType.Down, status));
                var arrowLeft = motionModel.MotionStatusLeft.Select(status => new KeyValuePair<MoveArrowType, MotionStatus>(MoveArrowType.Left, status));
                var arrow = Observable.CombineLatest(arrowUp, arrowRight, arrowDown, arrowLeft);

                // 現在の矢印取得
                arrow
                    .First()
                    .Subscribe(arrowInfos => _MoveArrowManager.Place(playerId, pTurn.TileId, arrowInfos));

                // コマ移動が起こったら、
                // コマ座標と最新矢印情報をまとめて矢印表示に流す
                motionModel
                    .PeekPosition
                    .WithLatestFrom(arrow, (peek, arrow) => (peek, arrow))
                    .Subscribe(p => _MoveArrowManager.Place(playerId, p.peek, p.arrow))
                    .AddTo(movementDisposables);

                // 残り歩数をUIに流す
                motionModel
                    .NumberOfMovableCells
                    .Subscribe(_UI.SetWalkRemaining)
                    .AddTo(movementDisposables);


                // 移動終了まで待つ
                motions = await motionQueueSubject;
                pTurn.TileId = motions.Last();

                // ルートを塗る
                while (motions.Count > 0)
                {
                    int tileId = motions.Dequeue();
                    var moveResult = fieldModel.MovePlayer(playerId, tileId);

                    // 「罠カードオープン！　おまえがオレの領域(テリトリー)を踏んだ時、オレは20点のスコアを得る！」
                    if (moveResult.IsOppositeEnter)
                    {
                        playerModels[moveResult.oppositePlayerId].OppositeEnterBonus += _OppositeEnterBonusOne;
                    }

                    // 塗る
                    _TileManager.ChangeArea(tileId, playerId);
                }
            }
        }

        private async void GoastPieceMove(int playerId, int tileId)
        {
            // await _PieceManager.Move(tileId, playerId);
        }

        private async UniTask UpdateScore()
        {
            List<UniTask> changeScore = new List<UniTask>();
            int[] newScore = new int[playerModels.Length];

            for (int i = 0; i < 2; i++)
            {
                newScore[i] = ScoreModel.CalculateFieldScore(fieldModel.GetFieldCells(), i) + playerModels[i].OppositeEnterBonus;
                changeScore.Add(_UI.ChangeScore(i, playerModels[i].Score, newScore[i]));
            }

            await UniTask.WhenAll(changeScore);
            
            for (int i = 0; i < 2; i++)
            {
                playerModels[i].Score = newScore[i];
            }
        }


        private void InputPosition(System.IObserver<Queue<int>> onMoveComplete, MotionModel model, int tileId)
        {
            model.PushMotionPosition(tileId);
            if (model.InspectInputtingMotionsFinished())
            {
                var motionQueue = model.GetMotionsAsQueue();
                model.ClearMotion();
                onMoveComplete.OnNext(motionQueue);
                onMoveComplete.OnCompleted();
            }
        }


        private bool ResetMotionModel(MotionModel model, FieldModel field, int currentPlayerId, int numberOfDice)
        {
            model.SetCurrentPlayerId(currentPlayerId);
            model.SetNumberOfDice(numberOfDice);
            model.SetCurrentPosition(field.GetCurrentPositionByPlayerId(currentPlayerId));
            model.SetFieldCellsAsMovableField(field.GetFieldCells());
            model.ClearInformation();

            return model.InspectPlayerCanMove();
        }

        protected override void StartAfterInitialize()
        {
            DoGame().Forget();
            /*
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
            */
        }

    }
}
