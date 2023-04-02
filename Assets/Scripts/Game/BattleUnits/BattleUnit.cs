using Swift.Math;
using System.Collections;
using System.Collections.Generic;

namespace TowerDefance.Game
{
    public partial class BattleUnit : BattleMap.IUnit, AIUnitExt.IUnit
    {
        public BattleUnit(string id, Fix64 maxSpeed, Fix64 maxHp)
        {
            UID = id;
            MaxSpeed = maxSpeed;
            MaxHp = maxHp;
            Hp = maxHp;
        }

        public Fix64 MaxHp { get; private set; }

        public Fix64 Hp { get; set; }

        public Fix64 MaxSpeed { get; set; }
    }
}
