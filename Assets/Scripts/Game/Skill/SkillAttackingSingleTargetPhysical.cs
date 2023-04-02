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

    public class SkillAttackingSingleTargetPhysical : ISkillAttacking
    {

        #region events

        public static Action<SkillAttackingSingleTargetPhysical, IAttacker, ITarget> OnTargetSelected = null;
        public static Action<SkillAttackingSingleTargetPhysical, IAttacker, ITarget, Fix64> OnAttacking = null;
        
        #endregion

        public string ID { get; private set; }

        public Fix64 Range { get; private set; }

        public IAttacker Owner { get; set; }

        public SkillAttackingSingleTargetPhysical(string id, Fix64 range)
        {
            ID = id;
            Range = range;
        }

        public ITarget FindClosestTarget(ITarget[] targets)
        {
            var target = ISkillAttacking.FindClosestTarget(Owner, targets);

            target = (Owner.Pos - target.Pos).Length > Range ? null : target;
            
            if (target != null)
                OnTargetSelected?.Invoke(this, Owner, target);

            return target;
        }

        public ITarget[] FindTargets(ITarget[] targets)
        {
            var target = FindClosestTarget(targets);
            return target == null ? new ITarget[0] : new ITarget[] { target };
        }

        public void Attack(ITarget target)
        {
            var damange = Owner.Power - target.Defence;

            if (damange < 0)
                damange = 0;

            var dHp = target.Hp < damange ? -target.Hp : -damange;
            target.Hp += dHp;

            OnAttacking?.Invoke(this, Owner, target, dHp);
        }

        public virtual void Attack(ITarget[] targets)
        {
            foreach (var t in targets)
                Attack(t);
        }
    }
}
