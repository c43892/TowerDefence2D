using Swift.Math;
using System.Collections;
using System.Collections.Generic;

namespace TowerDefance.Game
{
    // a tower instance in the tower defence battle
    public partial class BattleUnit : BattleMap.IUnit
    {
        public BattleMap Map { get; set; }

        public string UID { get; private set; }


        public Vec2 Pos { get; set; }

        public Fix64 Dir { get; set; }

        public string Type { get; set; } = null;
    }
}
