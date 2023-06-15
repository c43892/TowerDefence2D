using Swift.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalPanic
{
    public class LocalDriver : IGameDriver
    {
        public Battle Battle { get; private set; }
        public void SetBattle(Battle bt)
        {
            Battle = bt;
        }

        public void OnTimeElapsed(Fix64 te)
        {
            Battle.OnTimeElapsed(te);
        }
    }
}
