using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        Func<int, int, GridType> walkable = null;
        GridType[,] grids = null;


        void InitMap(Func<int, int, GridType> walkable)
        {
            grids = new GridType[w, h];
            this.walkable = walkable;

            // use filler fill all grids
            for (int x = 0; x < w; ++x)
                for (int y = 0; y < h; ++y)
                    grids[x, y] = walkable(x, y);
        }

        public bool Walkable(int x, int y) => grids[x, y] == GridType.None;
    }
}
