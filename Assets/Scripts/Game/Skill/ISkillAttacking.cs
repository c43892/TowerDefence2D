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
            Fix64 Power { get; }
        }

        public interface ITarget
        {
            Vec2 Pos { get; }
            Fix64 Defence { get; }
            Fix64 Hp { get; set; }
        }

        Fix64 Range { get; }

        IAttacker Owner { get; }

        ITarget[] FindTargets(ITarget[] targers);

        void Attack(ITarget[] targets);
    }
}
