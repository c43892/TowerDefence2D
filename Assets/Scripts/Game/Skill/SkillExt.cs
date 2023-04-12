using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Swift.Math;
using static TowerDefance.Game.ISkillAttacking;

namespace TowerDefance
{
    public static class SkillExt
    {
        public static ITarget[] FindClosestTargets(this IAttacker attacker, ITarget[] candidates, Func<ITarget, bool> filter = null, int maxNum = 1)
        {
            var targets = new List<ITarget>();

            foreach (var t in candidates)
            {
                if (t == null || (filter != null && !filter(t)))
                    continue;

                targets.Add(t);
                targets.OrderBy((tar) => (t.Pos - attacker.Pos).Length);

                if (targets.Count > maxNum)
                    targets.RemoveRange(maxNum, targets.Count - maxNum);
            }

            return targets.ToArray();
        }
    }
}
