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
    using IArea = ISkillAttacking.IArea;

    public partial interface ISkillAttacking : ISkill
    {
        public class Circle : IArea
        {
            public Fix64 Radius { get; private set; }
            public Circle(Fix64 radius) { Radius = radius; }
            public bool IsInArea(Vec2 center, Vec2 target) => (target - center).Length <= Radius;
        }
    }
}
