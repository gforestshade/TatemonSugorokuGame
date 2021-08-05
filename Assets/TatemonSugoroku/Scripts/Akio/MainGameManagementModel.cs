using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace TatemonSugoroku.Scripts.Akio
{
    public enum MainGamePhase
    {
        Idle,
        WaitingRollingDice,
        WhileRollingDice,
        WaitingMovingPlayer,
        WhileMovingPlayer,
        Finish,
    }
    public class MainGameManagementModel : IModel
    {
        private MotionModel _motionModel;
        private FieldModel _fieldModel;
        private ScoreModel _scoreModel;

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


        public void SetUpMotionModel(MotionModel motionModel)
        {
            _motionModel = motionModel;
        }

        public void SetUpFieldModel(FieldModel fieldModel)
        {
            _fieldModel = fieldModel;
        }

        public void SetUpScoreModel(ScoreModel scoreModel)
        {
            _scoreModel = scoreModel;
        }

        public void InitializeGame(int numberOfPlayers)
        {
            _scoreModel.InitializeGame(numberOfPlayers);
            _motionModel.InitializeGame();

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
        }

        public void StartPhase()
        {
            _gamePhase.Value = MainGamePhase.WaitingRollingDice;
        }
        

        // サイコロを振り始めて欲しい時に呼び出して欲しいメソッド
        public void InputDicesRoll()
        {
            _gamePhase.Value = MainGamePhase.WhileRollingDice;
        }

        // サイコロが振り終われば呼び出して欲しいメソッド
        public void NotifyDiceRollingFinished()
        {
            _decidedNumberOfDice.Value = 7; // todo Use DiceModel
            _motionModel.SetCurrentPlayerId(_currentPlayerId.Value);
            _motionModel.SetNumberOfDice(_decidedNumberOfDice.Value);
            _motionModel.SetCurrentPosition(_fieldModel.GetCurrentPositionByPlayerId(_currentPlayerId.Value));
            _motionModel.SetFieldCellsAsMovableField(_fieldModel.GetFieldCells());
            _motionModel.ClearInformation();
            
            if (_motionModel.InspectPlayerCanMove())
            {
                _gamePhase.Value = MainGamePhase.WaitingMovingPlayer;
            }
            else
            {
                _gamePhase.Value = MainGamePhase.Finish;
            }
        }
        
        public void InputPosition(int position)
        {
            _motionModel.PushMotionPosition(position);
            if (_motionModel.InspectInputtingMotionsFinished())
            {
                _determinedMotionPosition = _motionModel.GetMotionsAsQueue();
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

                _fieldModel.MovePlayer(_currentPlayerId.Value, position);
            }
        }

        // プレイヤーの移動が終われば呼び出して欲しいメソッド
        public void NotifyMovingPlayerFinished()
        {
            
        }

        public void PutTatemonAtCurrentPosition()
        {
            _fieldModel.PutTatemonAtCurrentPosition(_currentPlayerId.Value,
                _spinPowersOfTatemon[_decidedNumberOfDice.Value]);
        }

        // たてもんの配置が終われば呼び出して欲しいメソッド
        public void NotifyPuttingTatemonFinished()
        {
            
        }

        public void NextPhase()
        {
            _currentPlayerId.Value = _nextPlayerId[_currentPlayerId.Value];
            StartPhase();
        }
        
        public void Dispose()
        {
            
        }
    }
}
