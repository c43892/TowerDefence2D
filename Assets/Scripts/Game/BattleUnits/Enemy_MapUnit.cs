using Swift.Math;
using System.Collections;
using System.Collections.Generic;

namespace TowerDefance.Game
{
    // a tower instance in the tower defence battle
    public partial class Enemy
    {
        public BattleMap Map { get; set; }

        public Vec2 Pos { get; set; }
    }
}
