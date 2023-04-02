using Swift;
using Swift.Math;
using System.Collections;
using System.Collections.Generic;

namespace TowerDefance.Game
{
    using ITarget = ISkillAttacking.ITarget;

    public class Enemy : BattleUnit, ITarget
    {
        public Enemy(string id, Fix64 maxSpeed, Fix64 maxHp, Fix64 defence)
            : base(id, maxSpeed)
        {
            Defence = defence;
            MaxHp = maxHp;
            Hp = maxHp;
        }

        public Fix64 Defence { get; set; }

        public Fix64 Hp { get; set; }

        public Fix64 MaxHp { get; private set; }

        public StateMachine CreateAI(List<Vec2> movingPath)
        {
            return this.AIMove(movingPath, MaxSpeed);
        }
    }
}
