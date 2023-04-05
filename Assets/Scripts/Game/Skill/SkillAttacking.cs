using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Swift.Math;

namespace TowerDefance
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

        public static readonly int ATTACKING_DEPLAY = 500; // ms
        protected readonly List<Func<int, bool>> onGoingAttackingWrappers = new();
        protected readonly List<Func<int, bool>> warpperToRemove = new();

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

            var timeLeft = ATTACKING_DEPLAY;
            onGoingAttackingWrappers.Add((te) =>
            {
                timeLeft -= te;
                if (timeLeft > 0)
                    return false;

                attacking();
                return true;
            });
        }

        public virtual void OnTimeElapsed(int te)
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
