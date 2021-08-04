using UniRx;
using UnityEngine;

namespace TatemonSugoroku.Scripts
{
    public enum MainGamePhase
    {
        Idle,
        Start,
        WhileWaitingRollingDice,
        WhileRollingDice,
        WhileMoving
    }

    public class MainGameManagementModel : IModel

    {
        private FieldModel _fieldModel;

        private MotionModel _motionModel;
        // ScoreModel _scoreModel;
        // DiceModel _diceModel;


        private readonly ReactiveProperty<MainGamePhase> _mainGamePhase =
            new ReactiveProperty<MainGamePhase>(MainGamePhase.Idle);

        private readonly ReactiveProperty<int> _currentPlayerId = new ReactiveProperty<int>(-1);
        private ReactiveProperty<bool> _isAllowedToPushDiceButton = new ReactiveProperty<bool>(false);

        public IReadOnlyReactiveProperty<int> CurrentPlayerId => _currentPlayerId;

        public void SetFieldModel(FieldModel fieldModel)
        {
            _fieldModel = fieldModel;
        }

        public void SetMotionModel(MotionModel motionModel)
        {
            _motionModel = motionModel;
        }

        public void SetScoreModel()
        {

        }

        public void SetDiceModel()
        {

        }

        public void NotifyGameStart()
        {

        }

        public void NotifyRollingDicesEnd()
        {
            if (_mainGamePhase.Value == MainGamePhase.WhileRollingDice)
            {
                int numberOfDice = 7; // todo DiceModel.GetNumberOfDice() に変更する。
                if (_fieldModel == null)
                {
                    Debug.Log("FieldModelがアタッチされていません。");
                }
                else
                {
                    if (_fieldModel.InspectPlayerCanMove(_currentPlayerId.Value, numberOfDice))
                    {

                    }
                    else
                    {

                    }
                }
            }
            else
            {
                Debug.LogError("ダイスロール中以外の場面でこのメソッドを呼び出さないでください。");
            }
        }

        public void Dispose()
        {

        }
    }
}
