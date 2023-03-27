using System.Collections;
using System.Collections.Generic;

namespace TowerDefance.Game
{
    // a tower instance in the tower defence battle
    public partial class Tower : BattleMap.IUnit
    {
        public BattleMap Map { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
