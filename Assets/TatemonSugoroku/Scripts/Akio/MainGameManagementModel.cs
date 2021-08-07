using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace TatemonSugoroku.Scripts.Akio
{
    public enum MainGamePhase
    {
        Idle,
        Prepare,
        WhileShowingTurnCall,
        WaitingRollingDice,
        WhileRollingDice,
        WaitingMovingPlayer,
        WhileMovingPlayer,
        WaitingPuttingTatemon,
        WhilePuttingTatemon,
        WhileCalculatingScore,
        PlayerCannotMove,
        Finish,
    }
    public class MainGameManagementModel : IModel
    {
        private MotionModel _motionModel;
        private FieldModel _fieldModel;
        private ScoreModel _scoreModel;
//        private DiceModel _diceModel;

        private readonly ReactiveProperty<MainGamePhase>
            _gamePhase = new ReactiveProperty<MainGamePhase>(MainGamePhase.Idle);
        public IReadOnlyReactiveProperty<MainGamePhase> GamePhase => _gamePhase;

        private readonly ReactiveProperty<int> _currentPlayerId = new ReactiveProperty<int>(0);
        public IReadOnlyReactiveProperty<int> CurrentPlayerId => _currentPlayerId;

        private readonly ReactiveProperty<int> _decidedNumberOfDice = new ReactiveProperty<int>(0);
        public IReadOnlyReactiveProperty<int> DecidedNumberOfDice => _decidedNumberOfDice;

        private int[] _nextPlayerId;

        private Queue<int> _determinedMotionPosition = new Queue<int>();

        private readonly int[] _spinPowersOfTatemon = {0, 0, 6, 5, 5, 4, 4, 3, 3, 2, 2, 1, 1};


        public void Initialize( AllModelManager manager ) {
            _motionModel = manager.Get<MotionModel>();
            _fieldModel = manager.Get<FieldModel>();
            _scoreModel = manager.Get<ScoreModel>();
//            _diceModel = manager.Get<DiceModel>();
        }

        public void InitializeGame(int numberOfPlayers)
        {
            _scoreModel.InitializeGame(numberOfPlayers);
            _motionModel.InitializeGame();
            _fieldModel.InitializeGame(numberOfPlayers);

            switch (numberOfPlayers)
            {
                case 1:
                    _nextPlayerId = new[] {0};
                    break;
                case 2:
                    _nextPlayerId = new[] {1, 0};
                    break;
                case 3:
                    _nextPlayerId = new[] {1, 2, 0};
                    break;
            }

            _gamePhase.Value = MainGamePhase.Prepare;
        }

        // ゲーム開始時に呼び出して欲しいメソッド
        public void StartGame()
        {
            _currentPlayerId.Value = 0; // todo 後ほどランダムに変更
            _gamePhase.Value = MainGamePhase.WhileShowingTurnCall;
        }

        // ターン表示が終われば呼び出して欲しいメソッド（サイコロフェーズに入る）
        public void NotifyShowingTurnCallFinished()
        {
            _gamePhase.Value = MainGamePhase.WaitingRollingDice;
//            _diceModel.ChangeState(DiceState.Rotate);
        }

        // サイコロを振り始めて欲しい時に呼び出して欲しいメソッド
        public void InputDicesRoll()
        {
            _gamePhase.Value = MainGamePhase.WhileRollingDice;
/*
            _diceModel.SetPower(
                new Vector3(
                    UnityEngine.Random.Range(-1.0f, 1.0f),
                    UnityEngine.Random.Range(-1.0f, 1.0f),
                    UnityEngine.Random.Range(-1.0f, 1.0f)
                ).normalized * 10.0f
            );
            _diceModel.ChangeState(DiceState.Roll);
*/
        }

        // サイコロが振り終われば呼び出して欲しいメソッド
        public void NotifyDiceRollingFinished(int numberOfDice)
        {
            _decidedNumberOfDice.Value = numberOfDice;
            _motionModel.SetCurrentPlayerId(_currentPlayerId.Value);
            _motionModel.SetNumberOfDice(_decidedNumberOfDice.Value);
            _motionModel.SetCurrentPosition(_fieldModel.GetCurrentPositionByPlayerId(_currentPlayerId.Value));
            _motionModel.SetFieldCellsAsMovableField(_fieldModel.GetFieldCells());
            _motionModel.ClearInformation();

//            _diceModel.ChangeState(DiceState.Hide);
            if (_motionModel.InspectPlayerCanMove())
            {
                _gamePhase.Value = MainGamePhase.WaitingMovingPlayer;
            }
            else
            {
                _gamePhase.Value = MainGamePhase.PlayerCannotMove;
            }
        }
        
        public void InputPosition(int position)
        {
            _motionModel.PushMotionPosition(position);
            if (_motionModel.InspectInputtingMotionsFinished())
            {
                _determinedMotionPosition = _motionModel.GetMotionsAsQueue();
                _motionModel.ClearMotion();
                _gamePhase.Value = MainGamePhase.WhileMovingPlayer;
            }
        }

        public bool HasDeterminedMotionPosition()
        {
            return _determinedMotionPosition.Count > 0;
        }

        public void MovePlayer()
        {
            if (_determinedMotionPosition.Count > 0)
            {
                int position = _determinedMotionPosition.Dequeue();
                /*
                switch (_fieldModel.MovePlayer(_currentPlayerId.Value, position))
                {
                    case MoveResult.OppositeCellEntered:
                        
                        // todo 相手にスコア加算
                        
                        break;
                }
                */
            }
        }

        // プレイヤーの移動が終われば呼び出して欲しいメソッド
        public void NotifyMovingPlayerFinished()
        {
            _gamePhase.Value = MainGamePhase.WaitingPuttingTatemon;
        }

        public void NotifyInputPuttingTatemon()
        {
            _gamePhase.Value = MainGamePhase.WhilePuttingTatemon;
        }
        public void PutTatemonAtCurrentPosition()
        {
            _fieldModel.PutTatemonAtCurrentPosition(_currentPlayerId.Value,
                _spinPowersOfTatemon[_decidedNumberOfDice.Value]);
        }

        // たてもんの配置が終われば呼び出して欲しいメソッド（スコア計算フェーズに入る）
        public void NotifyPuttingTatemonFinished()
        {
            _gamePhase.Value = MainGamePhase.WhileCalculatingScore;
        }

        //スコアの計算演出が終われば呼び出して欲しいメソッド（次のターンに進む）
        public void NotifyCalculatingScoreFinished()
        {
            if (true) // たてもんの存在判定を確認し、存在するならば、次のターンへ
            {
                _currentPlayerId.Value = _nextPlayerId[_currentPlayerId.Value];
                _gamePhase.Value = MainGamePhase.WhileShowingTurnCall;
            }
            else //存在しないならば、ゲーム終了フェーズへ
            {
                _gamePhase.Value = MainGamePhase.Finish;
            }
        }

        // 移動不可能の警告が出されたら、承認したら呼び出して欲しいメソッド（結果フェーズに入る）
        public void NotifyShowingAlertThatPlayerCannotMove()
        {
            _gamePhase.Value = MainGamePhase.Finish;
        }

        public void Dispose()
        {
            
        }
    }
}
