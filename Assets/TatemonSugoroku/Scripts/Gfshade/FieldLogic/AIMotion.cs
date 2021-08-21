using System;
using System.Collections.Generic;
using UniRx;
using System.Linq;

namespace TatemonSugoroku.Scripts.FieldLogic
{
    public class AIMotion
    {
        private readonly int playerId;

        private readonly int numberOfDice;

        private readonly int turnIndex;

        private readonly int playerCount;

        private readonly Field field;

        private readonly Difficulty difficulty;
        
        private int[] initialScores;

        private int[] initialBonuses;

        /// <summary>
        /// DFSするためのやつ
        /// </summary>
        class DFSData
        {
            public int V;
            public int BeforeV;
            public int Depth;
        }

        public AIMotion(Field field, Difficulty difficulty, PlayerInternalModel[] playerModels, int turnIndex, int playerId, int numberOfDice)
        {
            this.playerId = playerId;
            this.numberOfDice = numberOfDice;
            this.field = new Field(field);
            this.difficulty = difficulty;
            this.initialScores = playerModels.Select(p => p.Score).ToArray();
            this.initialBonuses = playerModels.Select(p => p.OppositeEnterBonus).ToArray();
            this.turnIndex = turnIndex;
            playerCount = playerModels.Length;
        }

        public List<int> ChooseRoute()
        {
            MovingGraph graph = new MovingGraph(field, playerId);

            int maxHyouka = -9999;
            List<int> maxHyoukaRoute = null;
            foreach (List<int> route in graph.EnumerateAllRoutes(numberOfDice))
            {
                Field nextField = new Field(field);
                int[] bonuses = new int[playerCount];

                foreach (int i in route)
                {
                    var result = nextField.MovePlayer(playerId, i);
                    if (result.IsOppositeEnter)
                    {
                        bonuses[result.oppositePlayerId] += difficulty.oppositeEnterBonus;
                    }
                }
                int spinPower = difficulty.GetSpinPower(turnIndex, numberOfDice);
                nextField.PutTatemonAtCurrentPosition(playerId, spinPower);

                int[] thisScore = Score.CalculateScore(nextField);
                int hyouka = 0;
                for (int i = 0; i < playerCount; i++)
                {
                    thisScore[i] += initialBonuses[i] + bonuses[i];

                    if (i == playerId)
                    {
                        hyouka += thisScore[i] * (playerCount - 1);
                    }
                    else
                    {
                        hyouka -= thisScore[i];
                    }
                }

                if (hyouka > maxHyouka)
                {
                    maxHyouka = hyouka;
                    maxHyoukaRoute = route;
                }
            }

            return maxHyoukaRoute;
        }
    }
}
