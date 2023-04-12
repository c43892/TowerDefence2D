using System;
using System.Collections.Generic;
using System.Linq;
using Swift;
using Swift.Math;
using static TowerDefance.Game.ISkillAttacking;

namespace TowerDefance.Game
{
    using IAttacker = ISkillAttacking.IAttacker;
    using ITarget = ISkillAttacking.ITarget;
    using IArea = ISkillAttacking.IArea;

    public class SkillAttackingArea : SkillAttacking
    {
        #region events

        public static Action<SkillAttackingArea, IAttacker, Vec2> AboutToAttacking = null;
        public static Action<SkillAttackingArea, IAttacker, Vec2, Dictionary<ITarget, ISkillAttacking.AttackingResult>> OnAttackingDone = null;
        
        #endregion

        public IArea Area { get; private set; }

        public SkillAttackingArea(string id, Fix64 range, IArea area, int maxAttacks = 1)
            : base(id, range, maxAttacks)
        {
            Area = area;
        }

        public override Action AttackImpl(ITarget[] candidates)
        {
            var targets = Owner.FindClosestTargets(candidates, (t) => (t.Pos - Owner.Pos).Length <= Range, 1);
            if (targets.Count() == 0)
                return null;

            var targetPos = targets[0].Pos;
            targets = Owner.FindClosestTargets(candidates, (t) => Area.IsInArea(targetPos, t.Pos), MaxAttacks);

            AboutToAttacking?.Invoke(this, Owner, targetPos);

            return () =>
            {
                var attackingResults = new Dictionary<ITarget, ISkillAttacking.AttackingResult>();
                FC.ForEach(targets, (i, t) =>
                {
                    var timeLeft = ATTACKING_DEPLAY;

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

                    OnAttackingDone?.Invoke(this, Owner, targetPos, attackingResults);
                });
            };
        }
    }
}
