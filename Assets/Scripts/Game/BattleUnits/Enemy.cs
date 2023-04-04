using Swift;
using Swift.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TowerDefance.Game
{
    using ITarget = ISkillAttacking.ITarget;
    using IAttacker = ISkillAttacking.IAttacker;
    using IUnit = AIUnitExt.IUnit;
    using IAttackUnit = AIUnitExt.IAttackerUnit;

    public class Enemy : BattleUnit, ITarget, IAttacker, IAttackUnit
    {
        public Enemy(string id, Fix64 maxSpeed, Fix64 maxHp, Fix64 phyDefence, Fix64 magDefence, Fix64 phyPower, Fix64 magPower)
            : base(id, maxSpeed, maxHp)
        {
            PhyDefence = phyDefence;
            MagDefence = magDefence;
            PhyPower = phyPower;
            MagPower = magPower;
        }

        public Fix64 PhyDefence { get; set; }
        public Fix64 MagDefence { get; set; }

        public Fix64 PhyPower { get; set; }
        public Fix64 MagPower { get; set; }

        public Fix64 Cooldown { get; set; }

        public Type[] ValidTargetTypes { get; set; } = null;

        public ISkillAttacking Skill { get; set; } = null;


        public StateMachine CreateAI(List<Vec2> movingPath)
        {
            return this.MoveAndSelfExplode(movingPath, MaxSpeed);
        }

        public void Attack(IUnit[] targets)
        {
            if (targets.Length == 0)
                return;

            Skill.Attack(targets.Select(t => t as ITarget).ToArray());
        }

        bool IsValidTargetType(Type type)
        {
            return ValidTargetTypes == null || ValidTargetTypes.FirstIndexOf(type) >= 0;
        }

        // 只攻击 base
        public AIUnitExt.IUnit[] FindTargets()
        {
            var targets = Map.AllUnits
                .Where(u => u is ITarget && IsValidTargetType(u.GetType()))
                .Select(u => u as ITarget).ToArray();

            return Skill.FindTargets(targets).Select(t => t as IUnit).ToArray();
        }
    }
}
