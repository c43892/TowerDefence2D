using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Swift.Math;
using static TowerDefance.Game.IBuff;

namespace TowerDefance.Game
{
    public abstract class BuffOnce : IBuff, ITimeDriven
    {
        public BuffOnce(Fix64 waitingTime)
        {
            waitingLeft = waitingTime;
        }

        public string ID { get; set; }

        public bool Done { get => waitingLeft < 0; }

        public IBuffOwner Owner { get; set; }

        protected abstract void OnEffect();

        protected Fix64 waitingLeft = 0;

        public void OnTimeElapsed(Fix64 te)
        {
            if (Done)
                return;

            waitingLeft -= te;
            if (waitingLeft < 0)
            {
                OnEffect();
                Owner.RemoveBuff(this);
            }
        }
    }
}
