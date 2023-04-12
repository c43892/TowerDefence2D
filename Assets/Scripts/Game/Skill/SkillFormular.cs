using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Swift.Math;
using static TowerDefance.Game.ISkillAttacking;

namespace TowerDefance
{
    public static class SkillFomular
    {
        /*
            for an IAttacker: Pos, PhyPower, MagPower
            for an ITarget: Pos, Hp, PhyDefence, MagDefence
        */

        public static Fix64 PhyDamage(IAttacker attacker, ITarget target)
        {
            return attacker.PhyPower - target.PhyDefence;
        }

        public static Fix64 MagDamage(IAttacker attacker, ITarget target)
        {
            return attacker.MagPower - target.MagDefence;
        }
    }
}
