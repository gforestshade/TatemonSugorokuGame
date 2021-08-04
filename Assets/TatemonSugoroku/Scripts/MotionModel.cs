using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace TatemonSugoroku.Scripts
{
    public class MotionModel: IModel
    {
        private FieldModel _fieldModel;

        private int _currentPlayerId;
        private int _numberOfDice;

        private readonly Queue<int> _queueDeterminedPositions = new Queue<int>();
        
        public void SetFieldModel(FieldModel fieldModel)
        {
            _fieldModel = fieldModel;
        }

        public void Initialize()
        {
            
        }
        
        public void SetCurrentPlayerId(int playerId)
        {
            _currentPlayerId = playerId;
        }
        public void SetNumberOfDice(int numberOfDice)
        {
            _numberOfDice = numberOfDice;
        }
        
        public void SetPosition(int position)
        {
            if (_fieldModel == null)
            {
                Debug.LogError("FieldModelがアタッチされていません");
            }
            else
            {
                FieldCell[] fieldCells = _fieldModel.GetFieldCells();
            }
        }

        public int PopDeterminedPositions()
        {
            if (_queueDeterminedPositions.Count > 0)
            {
                return _queueDeterminedPositions.Dequeue();
            }

            return -1;
        }

        public void Dispose()
        {
            
        }
    }
}
