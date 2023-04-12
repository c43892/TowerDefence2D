using System;
using System.Collections.Generic;
using System.Linq;
using Swift;
using Swift.Math;

namespace TowerDefance.Game
{
    using IAttacker = ISkillAttacking.IAttacker;
    using ITarget = ISkillAttacking.ITarget;

    public class SkillAttackingTargets : SkillAttacking
    {

        #region events

        public static Action<SkillAttackingTargets, IAttacker, ITarget[]> AboutToAttacking = null;
        public static Action<SkillAttackingTargets, IAttacker, Dictionary<ITarget, ISkillAttacking.AttackingResult>> OnAttackingDone = null;
        
        #endregion

        public SkillAttackingTargets(string id, Fix64 range, int maxAttacks = 1)
            : base(id, range, maxAttacks)
        {
        }

        public override Action AttackImpl(ITarget[] candidates)
        {
            var targets = Owner.FindClosestTargets(candidates, t => (Owner.Pos - t.Pos).Length <= Range, MaxAttacks);
            if (targets.Count() == 0)
                return null;

            AboutToAttacking?.Invoke(this, Owner, targets);

            return () =>
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

                OnAttackingDone?.Invoke(this, Owner, attackingResults);
            };
        }
    }
}
