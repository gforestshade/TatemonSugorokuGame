using System.Collections.Generic;


namespace TatemonSugoroku.Scripts.FieldLogic
{
    public class MovingGraph
    {
        /// <summary>
        /// DFSするためのやつ
        /// </summary>
        class DFSData
        {
            public int V;
            public int BeforeV;
            public int Depth;
        }

        private readonly List<List<int>> graph;
        private readonly int initialPosition;
        private readonly int cellCount;

        public MovingGraph(Field field, int playerId)
        {
            cellCount = field.maxX * field.maxY;
            initialPosition = field.GetCurrentPositionByPlayerId(playerId);

            // 隣接リスト形式でグラフを構築したい
            graph = new List<List<int>>();
            for (int i = 0; i < cellCount; i++)
            {
                graph.Add(new List<int>());
            }

            // 全セルに対して
            for (int i = 0; i < cellCount; i++)
            {
                // 自分が通行不可なら何もしない
                if (!field.CanMove(playerId, i))
                {
                    continue;
                }

                // 自分と右が両方通行可能なら辺を張る
                int right = CellVector2.IdMove(i, MoveDirection.Right, field.maxX, field.maxY);
                if (right >= 0)
                {
                    if (field.CanMove(playerId, right))
                    {
                        graph[i].Add(right);
                        graph[right].Add(i);
                    }
                }

                // 自分と下が両方通行可能なら辺を張る
                int down = CellVector2.IdMove(i, MoveDirection.Down, field.maxX, field.maxY);
                if (down >= 0)
                {
                    if (field.CanMove(playerId, down))
                    {
                        graph[i].Add(down);
                        graph[down].Add(i);
                    }
                }
            }
        }

        /// <summary>
        /// どんなルートにせよ、出目の分だけ移動することが可能ならばtrue, どんなルートを通っても無理ならfalse
        /// </summary>
        /// <returns></returns>
        public bool CanMoveAnyRoute(int numberOfDice)
        {
            // DFSをしたいのでスタックを用意する           
            Stack<DFSData> stack = new Stack<DFSData>();

            // 原点は深さ0
            stack.Push(new DFSData
            {
                V = initialPosition,
                BeforeV = -1,
                Depth = 0
            });

            // すべてのルートを周りきるまで
            while (stack.Count > 0)
            {
                DFSData v = stack.Pop();

                if (v.Depth >= numberOfDice)
                {
                    // 出目の分だけ移動できるルートがあった！
                    // 3頂点以上でループしている可能性があるので、
                    // 1つでもあれば即early return
                    return true;
                }

                // 自分との間に辺が張られているすべての頂点に対して
                foreach (int nv in graph[v.V])
                {
                    // 来た道は除外しつつ(2頂点間のループは考えない)
                    if (nv == v.BeforeV)
                    {
                        continue;
                    }

                    // 次の頂点をスタックに積む(深さを1増やす)
                    stack.Push(new DFSData
                    {
                        V = nv,
                        BeforeV = v.V,
                        Depth = v.Depth + 1
                    });
                }
            }

            return false;
        }


        public IEnumerable<List<int>> EnumerateAllRoutes(int numberOfDice)
        {
            List<int> thisRoute = new List<int>();

            // DFSをしたいのでスタックを用意する           
            Stack<DFSData> stack = new Stack<DFSData>();

            // 原点は深さ0
            stack.Push(new DFSData
            {
                V = initialPosition,
                BeforeV = -1,
                Depth = 0
            });

            // すべてのルートを周りきるまで
            while (stack.Count > 0)
            {
                DFSData v = stack.Pop();
                if (thisRoute.Count > v.Depth)
                {
                    thisRoute.RemoveRange(v.Depth, thisRoute.Count - v.Depth);
                }
                thisRoute.Add(v.V);

                if (v.Depth >= numberOfDice)
                {
                    // 出目の分だけ移動できるルートがあった！
                    yield return new List<int>(thisRoute);
                    continue;
                }

                // 自分との間に辺が張られているすべての頂点に対して
                foreach (int nv in graph[v.V])
                {
                    // 来た道は除外しつつ(2頂点間のループは考えない)
                    if (nv == v.BeforeV)
                    {
                        continue;
                    }

                    // 次の頂点をスタックに積む(深さを1増やす)
                    stack.Push(new DFSData
                    {
                        V = nv,
                        BeforeV = v.V,
                        Depth = v.Depth + 1
                    });
                }
            }

        }

    }

}
