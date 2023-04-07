using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Swift;
using Swift.Math;

namespace TowerDefance.Game
{

    // battle map units related parts
    public partial class BattleMap
    {
        public enum GridType
        {
            None, // which can be used as a path and placed a tower
            Block, // which is a block and can't be used as a path
        }

        public Func<Vec2, GridType> GridTypeAt { get; private set; }
        GridType[,] grids = null;


        void InitGrids(Func<int, int, GridType> gridTypeAt)
        {
            grids = new GridType[Width, Height];
            GridTypeAt = (pos) => gridTypeAt((int)pos.x, (int)pos.y);

            // use filler fill all grids
            FC.For2(Width, Height, (x, y) => grids[x, y] = gridTypeAt(x, y));
        }

        public bool Walkable(Vec2 pos)
        {
            return pos.x >= 0 && pos.x < grids.GetLength(0)
                && pos.y >= 0 && pos.y < grids.GetLength(1)
                && GridTypeAt(pos) == GridType.None;
        }
    }
}
