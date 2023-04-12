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
        public Tower(string id, Fix64 phyPower, Fix64 magPower, Fix64 attackingInterval)
            : base(id, 0, 1, phyPower, magPower, attackingInterval)
        {
            PhyPower = phyPower;
            MagPower = magPower;
        }

        public override StateMachine CreateAI() => this.AIAttackInPlace();
    }
}
