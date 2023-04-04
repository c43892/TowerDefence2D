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
    using static UnityEngine.GraphicsBuffer;
    using IAttacker = ISkillAttacking.IAttacker;
    using ITarget = ISkillAttacking.ITarget;

    public class SkillAttackingTargetPhysical : ISkillAttacking
    {

        #region events

        public static Action<SkillAttackingTargetPhysical, IAttacker, ITarget[]> OnTargetSelected = null;
        public static Action<SkillAttackingTargetPhysical, IAttacker, ITarget[], Fix64[]> OnAttacking = null;
        
        #endregion

        public string ID { get; private set; }

        public Fix64 Range { get; private set; }

        public int MaxAttacks { get; private set; }

        public IAttacker Owner { get; set; }

        public SkillAttackingTargetPhysical(string id, Fix64 range, int maxAttacks = 1)
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
            var dhpArr = new Fix64[targets.Length];
            FC.ForEach(targets, (i, t) =>
            {
                var damange = Owner.Power - t.Defence;

                if (damange < 0)
                    damange = 0;

                var dHp = t.Hp < damange ? -t.Hp : -damange;
                t.Hp += dHp;
                dhpArr[i] = dHp;
            });

            OnAttacking?.Invoke(this, Owner, targets, dhpArr);
        }
    }
}
