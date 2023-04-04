using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Swift.Math;
using Unity.VisualScripting;

namespace TowerDefance
{
    using IAttacker = ISkillAttacking.IAttacker;
    using ITarget = ISkillAttacking.ITarget;

    public class SkillAttackingTargets : ISkillAttacking
    {

        #region events

        public static Action<SkillAttackingTargets, IAttacker, ITarget[]> OnTargetSelected = null;
        public static Action<SkillAttackingTargets, IAttacker, Dictionary<ITarget, ISkillAttacking.AttackingResult>> OnAttacking = null;
        
        #endregion

        public string ID { get; private set; }

        public Fix64 Range { get; private set; }

        public int MaxAttacks { get; private set; }

        public IAttacker Owner { get; set; }

        public SkillAttackingTargets(string id, Fix64 range, int maxAttacks = 1)
        {
            ID = id;
            Range = range;
            MaxAttacks = maxAttacks;
        }

        public ITarget[] FindTargets(ITarget[] candidates)
        {
            var targets = Owner.FindClosestTargets(candidates, t => (Owner.Pos - t.Pos).Length <= Range, MaxAttacks);
            if (targets.Count() > 0)
                OnTargetSelected?.Invoke(this, Owner, targets);

            return targets;
        }

        public virtual void Attack(ITarget[] targets)
        {
            var attackingResults = new Dictionary<ITarget, ISkillAttacking.AttackingResult>();
            FC.ForEach(targets, (i, t) =>
            {
                var phyDamage = SkillFomular.PhyDamage(Owner, t);
                var magDamage = SkillFomular.MagDamage(Owner, t);
                var damage = phyDamage + magDamage;
                var dhp = t.Hp < damage ? -t.Hp : -damage;

                attackingResults[t] = new ISkillAttacking.AttackingResult()
                {
                    PhyDamage = phyDamage,
                    MagDamage = magDamage,
                    DHp = dhp,
                };

                t.Hp += dhp;
            });

            OnAttacking?.Invoke(this, Owner, attackingResults);
        }
    }
}
