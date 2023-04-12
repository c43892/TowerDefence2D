using System;
using System.Collections.Generic;
using System.Linq;
using Swift;
using Swift.Math;

namespace TowerDefance.Game
{
    using IAttacker = ISkillAttacking.IAttacker;
    using ITarget = ISkillAttacking.ITarget;

    public abstract class SkillAttacking : ISkillAttacking
    {
        public string ID { get; private set; }

        public Fix64 Range { get; private set; }

        public int MaxAttacks { get; private set; }

        public IAttacker Owner { get; set; }

        public SkillAttacking(string id, Fix64 range, int maxAttacks = 1)
        {
            ID = id;
            Range = range;
            MaxAttacks = maxAttacks;
        }

        public static readonly Fix64 ATTACKING_DEPLAY = 0.2; // secs
        protected readonly List<Func<Fix64, bool>> onGoingAttackingWrappers = new();
        protected readonly List<Func<Fix64, bool>> warpperToRemove = new();

        public abstract Action AttackImpl(ITarget[] candidates);

        public virtual bool CanAttack()
        {
            return Owner.AllTargets.Any(t => (t.Pos - Owner.Pos).Length <= Range);
        }

        public void Attack()
        {
            var attacking = AttackImpl(Owner.AllTargets);
            if (attacking == null)
                return;

            Fix64 timeLeft = ATTACKING_DEPLAY;
            onGoingAttackingWrappers.Add((te) =>
            {
                timeLeft -= te;
                if (timeLeft > 0)
                    return false;

                attacking();
                return true;
            });
        }

        public virtual void OnTimeElapsed(Fix64 te)
        {
            FC.ForEach(onGoingAttackingWrappers, (i, w) =>
            {
                if (w(te))
                    warpperToRemove.Add(w);
            });

            warpperToRemove.Travel(w => onGoingAttackingWrappers.Remove(w));
            warpperToRemove.Clear();
        }
    }
}
