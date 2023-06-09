﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Swift.Math;

namespace TowerDefance.Game
{
    public partial interface ISkillAttacking : ISkill
    {
        public interface IAttacker
        {
            Vec2 Pos { get; }
            Fix64 PhyPower { get; }
            Fix64 MagPower { get; }

            ITarget[] AllTargets { get; }
        }

        public interface ITarget
        {
            Vec2 Pos { get; }
            Fix64 Hp { get; set; }

            Fix64 PhyDefence { get; }
            Fix64 MagDefence { get; }
        }

        public interface IArea
        {
            public bool IsInArea(Vec2 center, Vec2 target);
        }

        public class AttackingResult
        {
            public Fix64 PhyDamage { get; set; }
            public Fix64 MagDamage { get; set; }
            public Fix64 DHp { get; set; }
        }

        Fix64 Range { get; }

        IAttacker Owner { get; set; }

        bool CanAttack();

        void Attack();
    }
}
