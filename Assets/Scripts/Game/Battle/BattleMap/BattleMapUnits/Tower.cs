using Swift;
using Swift.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TowerDefance.Game
{
    public class Tower : Attacker
    {
        public Tower(string id, string type, Fix64 phyPower, Fix64 magPower, Fix64 attackingInterval)
            : base(id, type, 0, 1, phyPower, magPower, attackingInterval)
        {
            PhyPower = phyPower;
            MagPower = magPower;
        }

        public override StateMachine CreateAI() => this.AIAttackInPlace();

        public static Tower Create(TowerConfig cfg)
        {
            return new Tower(IdGen("tower"), cfg.type, cfg.phyPower, cfg.magPower, cfg.attackingInterval);
        }
    }
}
