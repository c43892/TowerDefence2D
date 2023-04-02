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

    public class Enemy : BattleUnit, ITarget, ISkillAttacking.IAttacker, AIUnitExt.IAttackerUnit
    {
        public Enemy(string id, Fix64 maxSpeed, Fix64 maxHp, Fix64 defence, Fix64 power)
            : base(id, maxSpeed, maxHp)
        {
            Defence = defence;
            Power = power;
        }

        public Fix64 Defence { get; set; }

        public Fix64 Power { get; set; }

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
