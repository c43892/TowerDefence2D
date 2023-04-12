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
    public abstract class BuffIntervalBase : IBuff, ITimeDriven
    {
        public string ID { get; set; }

        public abstract Fix64 Interval { get; } // in secs

        public abstract Fix64 Duration { get; } // in secs

        public IBuffOwner Owner { get; set; }

        public bool Done { get => life < 0; }

        protected abstract void OnEffect();

        protected bool running = false;
        protected Fix64 timeInterval = 0;
        protected Fix64 life = 0;

        public virtual void Start()
        {
            running = true;
            timeInterval = Interval;
            life = Duration;
        }

        public void OnTimeElapsed(Fix64 te)
        {
            if (!running)
                return;

            timeInterval -= te;
            while (timeInterval <= 0)
            {
                OnEffect();
                timeInterval += Interval;
            }

            life -= te;
            if (life < 0)
                running = false;
        }
    }
}
