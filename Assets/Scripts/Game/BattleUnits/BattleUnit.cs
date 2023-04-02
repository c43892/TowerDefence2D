using Swift.Math;
using System.Collections;
using System.Collections.Generic;

namespace TowerDefance.Game
{
    public partial class BattleUnit : BattleMap.IUnit, AIUnitExt.IUnit
    {
        public BattleUnit(string id, Fix64 maxSpeed)
        {
            UID = id;
            MaxSpeed = maxSpeed;
        }

        public Fix64 MaxSpeed { get; set; }
    }
}
