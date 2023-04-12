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

    public class TowerBase : BattleMapUnit, ITarget
    {
        public TowerBase(string uid, Fix64 phyDefence, Fix64 magDefence, Fix64 maxHp)
            : base(uid, 0, maxHp)
        {
            PhyDefence = phyDefence;
            MagDefence = magDefence;
        }

        public Fix64 PhyDefence { get; set; }
        public Fix64 MagDefence { get; set; }
    }
}
