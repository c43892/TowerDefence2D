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

    public class Enemy : BattleUnit, ITarget, IAttacker, IAttackUnit, IFrameDrived
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


        public StateMachine CreateAI(List<Vec2> movingPath) => this.MoveAndSelfExplode(movingPath, MaxSpeed);

        public void Attack() => Skill.Attack();

        bool IsValidTargetType(Type type) => ValidTargetTypes == null || ValidTargetTypes.FirstIndexOf(type) >= 0;

        public ITarget[] AllTargets => Map.AllUnits.Where((u) => u is ITarget && IsValidTargetType(u.GetType())).Select((u) => u as ITarget).ToArray();

        public bool CanAttack() => Skill.CanAttack();

        public void OnTimeElapsed(int dt)
        {
            Skill?.OnTimeElapsed(dt);
        }
    }
}
