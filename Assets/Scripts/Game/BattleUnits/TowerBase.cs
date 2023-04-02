using Swift;
using Swift.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TowerDefance.Game
{
    using ITarget = ISkillAttacking.ITarget;
    using IUnit = AIUnitExt.IUnit;

    public class TowerBase : BattleUnit, ITarget
    {
        public TowerBase(string uid, Fix64 defence, Fix64 maxHp)
            : base(uid, 0, maxHp)
        {
            Defence = defence;
        }

        public Fix64 Defence { get; set; }
    }
}
