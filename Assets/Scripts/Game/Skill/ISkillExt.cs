using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Swift.Math;
using static TowerDefance.ISkillAttacking;

namespace TowerDefance
{
    public static class ISkillExt
    {
        public static ITarget[] FindClosestTargets(this IAttacker attacker, ITarget[] candidates, Func<ITarget, bool> filter = null, int maxNum = 1)
        {
            var targets = new List<ITarget>();

            foreach (var t in candidates)
            {
                if (t == null || (filter != null && !filter(t)))
                    continue;

                targets.Add(t);
                targets.SwiftSort((tar) => (t.Pos - attacker.Pos).Length);

                if (targets.Count > maxNum)
                    targets.RemoveAt(targets.Count);
            }

            return targets.ToArray();
        }
    }
}
