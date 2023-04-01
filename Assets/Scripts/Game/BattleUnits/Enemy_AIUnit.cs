using Swift.Math;
using System.Collections;
using System.Collections.Generic;

namespace TowerDefance.Game
{
    // a tower instance in the tower defence battle
    public partial class Enemy
    {
        public string UID { get; private set; }

        public Fix64 ForwardDir { get; set; }

        public Fix64 MaxSpeed { get; set; }

        public Vec2 PreferredVelocity { get; set; }

        public Fix64 Dir { get; set; }
    }
}
