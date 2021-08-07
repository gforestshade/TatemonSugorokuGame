using System.Collections.Generic;
using UnityEngine;

namespace TatemonSugoroku.Scripts.Akio
{
    public class ScoreModel : IModel
    {
        public void Initialize( AllModelManager manager ) {
        }

        public void InitializeGame(int numberOfPlayers)
        {
            Debug.Log("Number of Players is" + numberOfPlayers);
        }
        
        public void Dispose()
        {
            
        }

        // FieldCellsとPlayerIdを入力して、盤面上のスコアを返す静的メソッドです。
        public static int CalculateFieldScore(FieldCell[] fieldCells, int playerId)
        {
            List<List<int>> graph = new List<List<int>>();
            List<bool> seen = new List<bool>();

            for (int i = 0; i < fieldCells.Length; i++)
            {
                graph.Add(new List<int>());
                seen.Add(false);
            }

            for (int i = 0; i < fieldCells.Length; i++)
            {
                if (fieldCells[i].DomainPlayerId == playerId)
                {
                    if (i % 8 < 7)
                    {
                        int j = i + 1;
                        if (fieldCells[i].DomainPlayerId == fieldCells[j].DomainPlayerId)
                        {
                            graph[i].Add(j);
                            graph[j].Add(i);
                        }
                    }

                    if (i / 8 < 7)
                    {
                        int j = i + 8;
                        if (fieldCells[i].DomainPlayerId == fieldCells[j].DomainPlayerId)
                        {
                            graph[i].Add(j);
                            graph[j].Add(i);
                        }
                    }
                }
                else
                {
                    seen[i] = true;
                }
            }

            int r = 0;
            for (int i = 0; i < fieldCells.Length; i++)
            {
                if (seen[i])
                {
                    continue;
                }

                HashSet<int> indexes = new HashSet<int>();
                Queue<int> queue = new Queue<int>();
                seen[i] = true;
                queue.Enqueue(i);
                indexes.Add(i);
                
                while (queue.Count > 0)
                {
                    int v = queue.Dequeue();
                    foreach (int nv in graph[v])
                    {
                        if (seen[nv])
                        {
                            continue;
                        }

                        seen[nv] = true;
                        indexes.Add(nv);
                        queue.Enqueue(nv);
                    }
                }

                int r1 = indexes.Count;
                int r2 = 0;
                foreach (int index in indexes)
                {
                    if (fieldCells[index].TatemonPlayerId == playerId)
                    {
                        r2 += fieldCells[index].TatemonSpinPower;
                    }
                }

                r += r1 * r2;
            }

            return r;
        }
    }
}
