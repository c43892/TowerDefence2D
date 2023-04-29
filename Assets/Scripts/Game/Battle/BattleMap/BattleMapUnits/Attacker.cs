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
    using IAttackUnit = AIUnitExt.IAttackerUnit;

    public class Attacker : BattleMapUnit, IAttacker, IAttackUnit
    {
        public Attacker(string id, string type, Fix64 maxSpeed, Fix64 maxHp, Fix64 phyPower, Fix64 magPower, Fix64 attackingInterval)
            : base(id, type, maxSpeed, maxHp)
        {
            PhyPower = phyPower;
            MagPower = magPower;
            AttackingInterval = attackingInterval;
        }

        public Fix64 PhyPower { get; set; }
        public Fix64 MagPower { get; set; }
        public Fix64 AttackingInterval { get; set; }

        public Type[] ValidTargetTypes { get; set; } = null;

        public ISkillAttacking Skill
        {
            get => skill;
            set
            {
                if (skill != null)
                    skill.Owner = null;

                skill = value;
                skill.Owner = this;
            }
        } ISkillAttacking skill = null;

        public void Attack() => Skill?.Attack();

        bool IsValidTargetType(Type type) => ValidTargetTypes == null || ValidTargetTypes.FirstIndexOf(type) >= 0;

        public ITarget[] AllTargets => Map.AllUnits.Where((u) => u is ITarget && IsValidTargetType(u.GetType())).Select((u) => u as ITarget).ToArray();

        public bool CanAttack() => Skill != null && Skill.CanAttack();

        public override void OnTimeElapsed(Fix64 te)
        {
            Skill?.OnTimeElapsed(te);
            base.OnTimeElapsed(te);
        }
    }
}
