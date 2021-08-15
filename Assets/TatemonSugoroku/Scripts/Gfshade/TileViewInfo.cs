
using UnityEngine;

namespace TatemonSugoroku.Scripts.Gfshade
{
    public class TileViewInfo
    {
        Difficulty diff;
        public TileViewInfo(Difficulty difficulty)
        {
            diff = difficulty;
        }

        public Vector2Int MaxSize => new Vector2Int(diff.maxX, diff.maxY);
    }
}
