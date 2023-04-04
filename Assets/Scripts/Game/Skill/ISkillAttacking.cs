using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Swift.Math;

namespace TowerDefance
{
    public interface ISkillAttacking : ISkill
    {
        public interface IAttacker
        {
            Vec2 Pos { get; }
            Fix64 PhyPower { get; }
            Fix64 MagPower { get; }
        }

        public interface ITarget
        {
            Vec2 Pos { get; }
            Fix64 Hp { get; set; }

            Fix64 PhyDefence { get; }
            Fix64 MagDefence { get; }
        }

        public class AttackingResult
        {
            public Fix64 PhyDamage { get; set; }
            public Fix64 MagDamage { get; set; }
            public Fix64 DHp { get; set; }
        }

        Fix64 Range { get; }

        IAttacker Owner { get; }

        ITarget[] FindTargets(ITarget[] targers);

        void Attack(ITarget[] targets);
    }
}
