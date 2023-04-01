using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Swift;

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

        public Func<int, int, GridType> GridTypeAt { get; private set; }
        GridType[,] grids = null;


        void InitGrids(Func<int, int, GridType> gridTypeAt)
        {
            grids = new GridType[Width, Height];
            GridTypeAt = gridTypeAt;

            // use filler fill all grids
            FC.For2(Width, Height, (x, y) => grids[x, y] = gridTypeAt(x, y));
        }

        public bool Walkable(int x, int y)
        {
            return x >= 0 && x < grids.GetLength(0)
                && y >= 0 && y < grids.GetLength(1)
                && GridTypeAt(x, y) == GridType.None;
        }
    }
}
