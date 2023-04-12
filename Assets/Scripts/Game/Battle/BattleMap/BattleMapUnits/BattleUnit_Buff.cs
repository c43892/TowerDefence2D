using Swift;
using Swift.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using static TowerDefance.Game.BuffSpeedReduction;

namespace TowerDefance.Game
{
    // a tower instance in the tower defence battle
    public partial class BattleMapUnit : IBuffSpeedReductionTarget
    {
        private readonly List<IBuff> buffs = new();

        public void AddBuff(IBuff buff)
        {
            if (buffs.Contains(buff))
                throw new Exception("already have the buff");

            buffs.Add(buff);
            buff.Owner = this;
        }

        public void RemoveBuff(IBuff buff)
        {
            if (!buffs.Contains(buff))
                throw new Exception("doesn't have the buff");

            buff.Owner = null;
            buffs.Remove(buff);
        }

        void ProcessBuff(Fix64 te)
        {
            buffs.ToArray().Travel(buff =>
            {
                if (buff is ITimeDriven timeBasedBuff)
                    timeBasedBuff.OnTimeElapsed(te);
            });
        }
    }
}
