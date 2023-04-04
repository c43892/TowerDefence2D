using Swift;
using Swift.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TowerDefance.Game
{
    using IAttacker = ISkillAttacking.IAttacker;
    using ITarget = ISkillAttacking.ITarget;
    using IUnit = AIUnitExt.IUnit;
    using IAttackerUnit = AIUnitExt.IAttackerUnit;

    public class Tower : BattleUnit, IAttacker, IAttackerUnit
    {
        public Tower(string id, Fix64 phyPower, Fix64 magPower, Fix64 attackingSpeed)
            : base(id, 0, 1)
        {
            PhyPower = phyPower;
            MagPower = magPower;
            AttackingSpeed = attackingSpeed;
        }

        public ISkillAttacking Skill { get; set; }

        public Fix64 PhyPower { get; private set; }
        public Fix64 MagPower { get; private set; }

        public Fix64 AttackingSpeed { get; private set; }

        public Fix64 Cooldown { get => 1 / AttackingSpeed; }

        public void Attack(IUnit[] targets)
        {
            if (targets.Length == 0)
                return;

            Skill.Attack(targets.Select(t => t as ITarget).ToArray());
        }

        public IUnit[] FindTargets()
        {
            var targets = Map.AllUnits
                .Where(u => u is ITarget && u is IUnit)
                .Select(u => u as ITarget).ToArray();

            return Skill.FindTargets(targets).Select(t => t as IUnit).ToArray();
        }

        public StateMachine CreateAI()
        {
            return this.AIAttackInPlace();
        }
    }
}
