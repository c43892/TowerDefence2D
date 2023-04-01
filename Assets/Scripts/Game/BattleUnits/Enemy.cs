using Swift.Math;
using System.Collections;
using System.Collections.Generic;

namespace TowerDefance.Game
{
    // a tower instance in the tower defence battle
    public partial class Enemy : BattleMap.IUnit, AIUnitExt.IMovingUnit
    {
        public Enemy(string id, Fix64 maxSpeed)
        {
            UID = id;
            MaxSpeed = maxSpeed;
        }
    }
}
