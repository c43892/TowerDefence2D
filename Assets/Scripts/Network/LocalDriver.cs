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
        private Battle bt;
        private Func<float> getCursorSpeed;
        private Func<float> getCursorBackSpeed;
        private Fix64 movingTimer = 0;
        private Fix64 backMovingTimer = 0;

        public void SetBattle(Battle battle)
        {
            bt = battle;
        }

        public void SetCursorSpeed(Func<float> cursorSpeed, Func<float> cursorBackspeed)
        {
            getCursorSpeed = cursorSpeed;
            getCursorBackSpeed = cursorBackspeed;
        }

        public void OnTimeElapsed(Fix64 te)
        {
            var cursorSpeed = getCursorSpeed == null ? 0 : getCursorSpeed();
            var cursorBackSpeed = getCursorBackSpeed == null ? 0 : getCursorBackSpeed();

            movingTimer += cursorSpeed * te;
            backMovingTimer += cursorBackSpeed * te;

            bt.OnTimeElapsed(te);
        }

        public void Input(int dx, int dy, bool forceUnsafe)
        {
            if (bt.Cursor.CoolDown > 0 || bt.Ended)
                return;

            if (movingTimer >= 1)
            {
                if (dx != 0 || dy != 0)
                    bt.TryMoveCursor(dx, dy, forceUnsafe);

                movingTimer %= 1;
            }

            if (backMovingTimer > 1)
            {
                if (dx == 0 && dy == 0)
                    bt.SetbackCursor();

                backMovingTimer %= 1;
            }
        }
    }
}
