using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalPanic
{
    public interface IGameDriver
    {
        void SetBattle(Battle battle);
        void SetCursorSpeed(Func<float> cursorSpeed, Func<float> cursorBackspeed);
    }
}
