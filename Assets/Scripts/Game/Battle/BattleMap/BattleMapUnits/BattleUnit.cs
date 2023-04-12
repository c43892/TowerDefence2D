using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using static TowerDefance.Game.BuffSpeedReduction;

namespace TowerDefance.Game
{
    public partial class BattleMapUnit : AIUnitExt.IUnit, ITimeDriven, IBuffSpeedReductionTarget
    {
        public BattleMapUnit(string id, Fix64 maxSpeed, Fix64 maxHp)
        {
            UID = id;
            MaxSpeed = maxSpeed;
            Speed = maxSpeed;
            MaxHp = maxHp;
            Hp = maxHp;
        }

        public BattleMap Map { get; set; }

        public string UID { get; private set; }

        public Vec2 Pos { get; set; }

        public Fix64 Dir { get; set; }

        public string Type { get; set; } = null;

        public Fix64 MaxHp { get; private set; }

        public Fix64 Hp { get; set; }

        public Fix64 Speed { get; set; }

        public Fix64 MaxSpeed { get; set; }

        public virtual void OnTimeElapsed(Fix64 te)
        {
            ProcessBuff(te);

            if (AI != null)
                AI.Run(te);
        }
    }
}
