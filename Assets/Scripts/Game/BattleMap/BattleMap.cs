using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TowerDefance.Game
{

    // the only map instance in the tower defence battle
    public partial class BattleMap
    {
        // the size of the map
        readonly int w;
        readonly int h;

        public BattleMap(int w, int h, Func<int, int, GridType> walkable)
        {
            this.w = w;
            this.h = h;

            InitMap(walkable);
            InitUnits();
        }
    }
}
