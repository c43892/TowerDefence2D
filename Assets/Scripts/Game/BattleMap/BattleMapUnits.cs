using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TowerDefance.Game
{

    // battle map units related parts
    public partial class BattleMap
    {
        public interface IUnit
        {
            public BattleMap Map { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }

        IUnit[,] units = null;

        void InitUnits()
        {
            units = new IUnit[w, h];
        }

        public IUnit UnitAt(int x, int y) => units[x, y];

        public void PlaceUnit(IUnit unit, int x, int y)
        {
            if (UnitAt(x, y) != null)
                throw new Exception($"({x}, {y}) is occupied");

            units[x, y] = unit;
        }
    }
}
