using System.Collections.Generic;
using System.Linq;
using SubmarineMirage.Base;
using SubmarineMirage.Debug;
using SubmarineMirage.Service;
using SubmarineMirage.Audio;
using SubmarineMirage.Setting;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace TatemonSugoroku.Scripts
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

    static partial class Utility
    {
        public static int MaxIndex<T>(IList<T> list)
            where T : System.IComparable<T>
        {
            if (list.Count == 0) return -1;

            int max_index = 0;
            T max_element = list[0];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].CompareTo(max_element) > 0)
                {
                    max_index = i;
                    max_element = list[i];
                }
            }
            return max_index;
        }
    }

    // Gfshade: 大改編
    public class MainGameManager : MonoBehaviour
    {
        [SerializeField]
        private UICanvas _UI;

        [SerializeField]
        private UITurn _TurnUI;

        [SerializeField]
        private DiceCanvas _DiceUI;

        [SerializeField]
        private UIGameEnd _GameEndUI;

        [SerializeField]
        private ResultCanvas _ResultUI;

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

        [SerializeField]
        private DayView _DayManager;

        [SerializeField]
        private GameCameraView _CameraManager;


        private FieldLogic.Field fieldModel;
        SMInputManager inputManager;
        PlayerInternalModel[] playerModels;
        Difficulty difficulty;

        private void InitTestPlayerInternalModels(Difficulty diff)
        {
            playerModels = new PlayerInternalModel[]
            {
                new PlayerInternalModel
                {
                    Id = 0,
                    Name = "プレイヤー1",
                    Score = 0,
                    OppositeEnterBonus = 0,
                    Tatemon = diff.maxTurn,
                    MaxTatemon = diff.maxTurn,
                    TileId = 0,
                },
                new PlayerInternalModel
                {
                    Id = 1,
                    Name = "プレイヤー2",
                    Score = 0,
                    OppositeEnterBonus = 0,
                    Tatemon = diff.maxTurn,
                    MaxTatemon = diff.maxTurn,
                    TileId = diff.maxX * diff.maxY - 1,
                },
            };
        }

        public async UniTask DoGame(CancellationToken ct)
        {
            DifficultyManager difficultyManager = Singleton<DifficultyManager>.s_instance;
            if (difficultyManager.Diff == null) difficultyManager.Diff = Difficulty.Normal;
            difficulty = difficultyManager.Diff;

            InitTestPlayerInternalModels(difficulty);
            inputManager = SMServiceLocator.Resolve<SMInputManager>();

            fieldModel = new FieldLogic.Field(difficulty.maxX, difficulty.maxY, playerModels.Select(m => m.TileId).ToArray());

            for (int i = 0; i < 2; i++)
            {
                _TileManager.ChangeArea(playerModels[i].TileId, i);
            }

            InitUI();
            System.TimeSpan wait02 = System.TimeSpan.FromSeconds(0.2);
            System.TimeSpan wait10 = System.TimeSpan.FromSeconds(1.0);

            // げーむがはじまるよ
            await GameStart(ct);

            var audioManager = await SMServiceLocator.WaitResolve<SMAudioManager>();
            await audioManager.Play( SMJingle.Start1 );
            await audioManager.Play( SMJingle.Start2 );

            for (int i = 0; i < difficulty.maxTurn; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    bool turnResult = await DoPlayerTurn(i, j, ct);
                    if (!turnResult) goto GameEnd;

                    // たてもんが置かれるたび、それのスコアは再計算される。
                    await UpdateScore(ct);
                }

                // じかんがすすむよ
                _DayManager.UpdateHour();

                // スコア更新時の時間調整だよ
                await UniTask.Delay(wait10, cancellationToken: ct);
            }

            /// 3歩あるいたあとはスコア更新しなくてもいい気がするかなあ

            GameEnd:
            // げーむがおわったよ
            await GameEnd(ct);

            // 「そして、オレが勝者となる」
            List<int> scores = playerModels.Select(p => p.Score).ToList();
            int winner = Utility.MaxIndex(scores);
            await _ResultUI.WaitForClick(scores, winner, ct);
        }

        private async UniTask<bool> DoPlayerTurn(int turnIndex, int playerId, CancellationToken ct)
        {
            System.TimeSpan wait02 = System.TimeSpan.FromSeconds(0.2);
            System.TimeSpan wait10 = System.TimeSpan.FromSeconds(1.0);

            PlayerInternalModel pTurn = playerModels[playerId];

            // 「たておのターン！」
            await TurnStart(pTurn, ct);

            // 「ドロー！」
            int dice = await DoDice(playerId, ct);

            // 「オレは、12を召喚！」
            _UI.SetWalkRemaining(dice);
            await UniTask.Delay(wait02, cancellationToken: ct);

            // 「フィールドを、好きな方向に12回まで塗りつぶすことができるッ！」
            await InputAndMove(playerId, dice, ct);

            await UniTask.Delay(wait02, cancellationToken: ct);

            // 「さらに！　塗りつぶしが終了したとき、効果発動！オレの最終位置に、設置魔法「たてもん」が発動するぜ！」
            int spinPower;
            if (turnIndex < difficulty.feverTurn)
            {
                spinPower = difficulty.spinPower[dice];
            }
            else
            {
                spinPower = difficulty.feverSpinPower[dice];
            }

            // 「モンスター「たてもん」が召喚される！」
            fieldModel.PutTatemonAtCurrentPosition(playerId, spinPower);
            _TatemonManager.Place(playerId, pTurn.TileId, spinPower);
            await UniTask.Delay(wait02, cancellationToken: ct);
            await _UI.ChangeTatemon(playerId, pTurn.Tatemon, pTurn.Tatemon - 1);
            playerModels[playerId].Tatemon -= 1;

            // ターン終了時の時間調整だよ
            await UniTask.Delay(wait10, cancellationToken: ct);

            return true;
        }

        private void InitUI()
        {
            for (int i = 0; i < playerModels.Length; i++)
            {
                _UI.SetScore(i, playerModels[i].Score);
                _UI.SetTatemon(i, playerModels[i].Tatemon);
                _UI.SetMaxTatemon(i, playerModels[i].MaxTatemon);
            }
        }

        private async UniTask GameStart(CancellationToken ct)
        {
            SMLog.Debug($"ゲーム開始", SMLogTag.Scene);
            //await UniTask.Delay(System.TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// ゲーム終了時に演出をするぞ！！！！！！！！！！！！！！！！！！！！！！
        /// </summary>
        private async UniTask GameEnd(CancellationToken ct)
        {
            SMLog.Debug($"ゲーム終了", SMLogTag.Scene);
            _UI.gameObject.SetActive(false);
            _TurnUI.ChangeTurn( null );
            _GameEndUI.SetActive( true );
            _CameraManager.SetResultCamera();
            await UniTask.Delay(System.TimeSpan.FromSeconds(1), cancellationToken: ct);
        }

        private async UniTask TurnStart(PlayerInternalModel model, CancellationToken ct)
        {
            SMLog.Debug($"{model.Name}のターン", SMLogTag.Scene);
            //_UI.SetCurrentPlayerName(model.Name);
            await UniTask.Delay(System.TimeSpan.FromSeconds(1), cancellationToken: ct);
            _TurnUI.ChangeTurn( model.Id );
        }

        private async UniTask<int> DoDice(int playerId, CancellationToken ct)
        {
            SMLog.Debug($"サイコロボタンを押してください", SMLogTag.Scene);
            await _DiceManager.ChangeState( DiceState.Rotate );
            await _DiceUI.WaitForClick(playerId);
            int dice = await _DiceManager.Roll();
            
            SMLog.Debug($"{dice}が出ました", SMLogTag.Scene);
            return dice;
        }

        private async UniTask<bool> InputAndMove(int playerId, int dice, CancellationToken ct)
        {
            PlayerInternalModel pTurn = playerModels[playerId];

            FieldLogic.Motion motionModel = fieldModel.CreateMotion(playerId, dice);

            // 初期化時、囲まれてて移動できないならゲーム終了
            if (!motionModel.CanMoveAnyRoute())
            {
                return false;
            }

            Queue<int> motions;

            // 例外が飛んだ場合は購読していたという事実は無かったことになる
            using (CompositeDisposable movementDisposables = new CompositeDisposable())
            {
                // 移動ルート受け取り用のSubjectを用意
                Subject<Queue<int>> motionQueueSubject = new Subject<Queue<int>>();

                // 触った場所を購読して、MotionModelに流す
                // 最終結果は移動ルート受け取り用Subjectに向かってぷっしゅしてもらう
                inputManager
                    ._touchTileID
                    .Subscribe(tileId => InputPosition(motionQueueSubject, motionModel, tileId))
                    .AddTo(movementDisposables);

                // コマ移動
                /*
                fieldModel
                    .PlayerPositions
                    .Select(pp => pp[playerId])
                    .Subscribe(tileId => GoastPieceMove(playerId, tileId))
                    .AddTo(movementDisposables);
                */

                // まず(矢印方向, 矢印状態)のぺあにおいて変化が全部送られてくるストリームをつくる
                var arrowUp = motionModel.MotionStatusUp.Select(status => new KeyValuePair<MoveArrowType, FieldLogic.MotionStatus>(MoveArrowType.Up, status));
                var arrowRight = motionModel.MotionStatusRight.Select(status => new KeyValuePair<MoveArrowType, FieldLogic.MotionStatus>(MoveArrowType.Right, status));
                var arrowDown = motionModel.MotionStatusDown.Select(status => new KeyValuePair<MoveArrowType, FieldLogic.MotionStatus>(MoveArrowType.Down, status));
                var arrowLeft = motionModel.MotionStatusLeft.Select(status => new KeyValuePair<MoveArrowType, FieldLogic.MotionStatus>(MoveArrowType.Left, status));
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
                motions = await motionQueueSubject.ToUniTask(cancellationToken: ct);
                pTurn.TileId = motions.Last();
                HideArrows();
            }

            // ルートを塗る
            while (motions.Count > 0)
            {
                int tileId = motions.Dequeue();
                var moveResult = fieldModel.MovePlayer(playerId, tileId);

                // 「罠カードオープン！　おまえがオレの領域(テリトリー)を踏んだ時、オレは20点のスコアを得る！」
                if (moveResult.IsOppositeEnter)
                {
                    playerModels[moveResult.oppositePlayerId].OppositeEnterBonus += difficulty.oppositeEnterBonus;
                }

                // 塗る
                _TileManager.ChangeArea(tileId, playerId);

                await _PieceManager.Move(playerId, pTurn.TileId);
            }

            return true;
        }

        private void HideArrows()
        {
            _MoveArrowManager.Hide();
            _UI.HideWalkRemaining();
        }

        private async UniTask UpdateScore(CancellationToken ct)
        {
            List<UniTask> changeScore = new List<UniTask>();
            int[] newScore = new int[playerModels.Length];

            for (int i = 0; i < 2; i++)
            {
                newScore[i] = FieldLogic.ScoreModel.CalculateFieldScore(fieldModel.GetFieldCells(), i, difficulty.maxX, difficulty.maxY) + playerModels[i].OppositeEnterBonus;
                changeScore.Add(_UI.ChangeScore(i, playerModels[i].Score, newScore[i]));
            }

            await UniTask.WhenAll(changeScore);
            
            for (int i = 0; i < 2; i++)
            {
                playerModels[i].Score = newScore[i];
            }
        }


        private void InputPosition(System.IObserver<Queue<int>> onMoveComplete, FieldLogic.Motion model, int tileId)
        {
            model.PushMotionPosition(tileId);
            if (model.IsFinished())
            {
                var motionQueue = model.GetMotionsAsQueue();
                onMoveComplete.OnNext(motionQueue);
                onMoveComplete.OnCompleted();
            }
        }

    }
}
