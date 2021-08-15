using UnityEngine;

namespace TatemonSugoroku.Scripts.Akio
{
    using FieldLogic;

    public class AkioModelChecker : MonoBehaviour
    {
        private readonly Field _fieldModel = new Field(8, 8, new int[] {0, 63});
        
        void Start()
        {
            Cell[] fieldCells = new Cell[64];

            for (int i = 0; i < fieldCells.Length; i++)
            {
                fieldCells[i] = new Cell
                {
                    TatemonSpinPower = 0,
                    TatemonPlayerId = -1,
                    DomainPlayerId = -1
                };
            }

            fieldCells[0] = new Cell
            {
                TatemonSpinPower = 1,
                TatemonPlayerId = 0,
                DomainPlayerId = 0
            };
            fieldCells[1] = new Cell
            {
                TatemonSpinPower = 2,
                TatemonPlayerId = 0,
                DomainPlayerId = 0
            };
            fieldCells[2] = new Cell
            {
                TatemonSpinPower = 0,
                TatemonPlayerId = -1,
                DomainPlayerId = 0
            };
            fieldCells[3] = new Cell
            {
                TatemonSpinPower = 3,
                TatemonPlayerId = 0,
                DomainPlayerId = 0
            };

            Debug.Log("FIELD SCORE: " + ScoreModel.CalculateFieldScore(fieldCells, 0, 8, 8));
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
