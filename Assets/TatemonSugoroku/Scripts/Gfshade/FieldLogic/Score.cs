using System.Collections.Generic;


namespace TatemonSugoroku.Scripts.FieldLogic
{
    /// <summary>
    /// Disjoint Union Sets
    /// </summary>
    public class DSU
    {
        private readonly int[] parentOrSize;

        public DSU(int n)
        {
            parentOrSize = new int[n];
            for (int i = 0; i < n; i++)
            {
                parentOrSize[i] = -1;
            }
        }

        public int Merge(int a, int b)
        {
            int x = Leader(a);
            int y = Leader(b);
            
            if (x == y) return x;
            
            if (-parentOrSize[x] < -parentOrSize[y]) (x, y) = (y, x);
            parentOrSize[x] += parentOrSize[y];
            parentOrSize[y] = x;
            
            return x;
        }

        public bool Same(int a, int b)
        {
            return Leader(a) == Leader(b);
        }

        public int Leader(int a)
        {
            // 自分が根
            if (parentOrSize[a] < 0) return a;
            
            // サイズを見て平衡にしていく
            while (parentOrSize[parentOrSize[a]] >= 0)
            {
                (a, parentOrSize[a]) = (parentOrSize[a], parentOrSize[parentOrSize[a]]);
            }
            
            return parentOrSize[a];
        }


        public int Size(int a)
        {
            return -parentOrSize[Leader(a)];
        }

        public List<List<int>> Groups()
        {
            int N = parentOrSize.Length;
            int[] leaderBuf = new int[N];
            int[] id = new int[N];
            
            var result = new List<List<int>>(N);
            
            for (int i = 0; i < N; i++)
            {
                leaderBuf[i] = Leader(i);
                if (leaderBuf[i] == i)
                {
                    id[i] = result.Count;
                    result.Add(new List<int>());
                }
            }

            for (int i = 0; i < N; i++)
            {
                var leaderID = id[leaderBuf[i]];
                result[leaderID].Add(i);
            }
            return result;
        }
    }

    public class Score
    {
        public static int[] CalculateScore(Field field)
        {
            int cellCount = field.maxX * field.maxY;
            DSU dsu = new DSU(cellCount);

            for (int i = 0; i < cellCount; i++)
            {
                if (field.Domain(i) < 0) continue;

                // 自分と右が同じ色なら辺を張る
                int right = CellVector2.IdMove(i, MoveDirection.Right, field.maxX, field.maxY);
                if (right >= 0)
                {
                    if (field.Domain(i) == field.Domain(right))
                    {
                        dsu.Merge(i, right);
                    }
                }

                // 自分と下が両方通行可能なら辺を張る
                int down = CellVector2.IdMove(i, MoveDirection.Down, field.maxX, field.maxY);
                if (down >= 0)
                {
                    if (field.Domain(i) == field.Domain(down))
                    {
                        dsu.Merge(i, down);
                    }
                }
            }

            int[] scores = new int[field.PlayerCount];
            for (int i = 0; i < field.PlayerCount; i++)
            {
                scores[i] = 0;
            }

            var gruops = dsu.Groups();
            foreach (var g in gruops)
            {
                int domain = field.Domain(g[0]);
                if (domain < 0) continue;

                int spinPowerSum = 0;
                foreach (var i in g)
                {
                    spinPowerSum += field.SpinPower(i);    
                }

                scores[domain] += spinPowerSum * g.Count;
            }
            return scores;
        }

        // FieldCellsとPlayerIdを入力して、盤面上のスコアを返す静的メソッドです。
        public static int CalculateScore(Cell[] fieldCells, int playerId, int maxX, int maxY)
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
                    if (i % maxX < maxX - 1)
                    {
                        int j = i + 1;
                        if (fieldCells[i].DomainPlayerId == fieldCells[j].DomainPlayerId)
                        {
                            graph[i].Add(j);
                            graph[j].Add(i);
                        }
                    }

                    if (i / maxX < maxY - 1)
                    {
                        int j = i + maxX;
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