using Swift;
using Swift.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TowerDefance.Game
{
    using ITarget = ISkillAttacking.ITarget;

    public class Enemy : Attacker, ITarget
    {
        public Enemy(string id, string type, Fix64 maxSpeed, Fix64 maxHp, Fix64 phyDefence, Fix64 magDefence, Fix64 phyPower, Fix64 magPower, IEnumerable<Vec2> movingPath)
            : base(id, type, maxSpeed, maxHp, phyPower, magPower, 0)
        {
            Type = type;
            PhyDefence = phyDefence;
            MagDefence = magDefence;
            MovingPath = movingPath;
        }

        public Fix64 PhyDefence { get; set; }
        public Fix64 MagDefence { get; set; }

        public IEnumerable<Vec2> MovingPath { get; private set; }

        public override StateMachine CreateAI() => this.MoveAndSelfExplode(MovingPath, () => Speed);

        public static Enemy Create(EnemyConfig cfg, IEnumerable<Vec2> movingPath)
        {
            return new Enemy(IdGen("enemy"), cfg.type, cfg.maxSpeed, cfg.maxHp, cfg.phyDefence, cfg.magDefence, cfg.phyPower, cfg.magPower, movingPath);
        }
    }
}
