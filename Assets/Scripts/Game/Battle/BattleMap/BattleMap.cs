using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Swift;
using Swift.Math;

namespace TowerDefance.Game
{

    // the only map instance in the tower defence battle
    public partial class BattleMap : ITimeDriven
    {
        // the size of the map
        public int Width { get; private set; }
        public int Height { get; private set; }

        public BattleMap(int width, int height, Func<int, int, GridType> gridType)
        {
            Width = width;
            Height = height;

            InitGrids(gridType);
            InitUnits();
        }

        public void OnTimeElapsed(Fix64 te)
        {
            UpdateAllUnits(te);
        }
    }
}
