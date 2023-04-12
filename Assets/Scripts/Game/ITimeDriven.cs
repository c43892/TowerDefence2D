using Swift.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefance
{
    public interface ITimeDriven
    {
        void OnTimeElapsed(Fix64 te);
    }
}
