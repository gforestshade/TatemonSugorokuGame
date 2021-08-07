using UnityEngine;

namespace TatemonSugoroku.Scripts.Akio
{
    public class AkioModelChecker : MonoBehaviour
    {
        private readonly FieldModel _fieldModel = new FieldModel();
        
        void Start()
        {
            FieldCell[] fieldCells = new FieldCell[64];

            for (int i = 0; i < fieldCells.Length; i++)
            {
                fieldCells[i] = new FieldCell
                {
                    TatemonSpinPower = 0,
                    TatemonPlayerId = -1,
                    DomainPlayerId = -1
                };
            }

            fieldCells[0] = new FieldCell
            {
                TatemonSpinPower = 1,
                TatemonPlayerId = 0,
                DomainPlayerId = 0
            };
            fieldCells[1] = new FieldCell
            {
                TatemonSpinPower = 2,
                TatemonPlayerId = 0,
                DomainPlayerId = 0
            };
            fieldCells[2] = new FieldCell
            {
                TatemonSpinPower = 0,
                TatemonPlayerId = -1,
                DomainPlayerId = 0
            };
            fieldCells[3] = new FieldCell
            {
                TatemonSpinPower = 3,
                TatemonPlayerId = 0,
                DomainPlayerId = 0
            };

            Debug.Log("FIELD SCORE: " + ScoreModel.CalculateFieldScore(fieldCells, 0));
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
