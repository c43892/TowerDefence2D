using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using System;
using System.Linq;

namespace GalPanic
{
    public class Battle : ITimeDriven
    {
        public Action OnWon = null;
        public Action OnLost = null;
        public Action OnCompletionChanged = null;

        public BattleMap Map { get; private set; }

        public Vec2 CursorPos => new(CursorX, CursorY);
        public int CursorX { get; private set; } = 0;
        public int CursorY { get; private set; } = 0;
        public float WinPrecentage { get; private set; } = 1;
        public bool Ended { get; private set; } = false;


        public Battle(int mapWidth, int mapHeight, float winPrecent = 0.5f)
        {
            Map = new BattleMap(this, mapWidth, mapHeight);
            CursorX = CursorY = 0;
            WinPrecentage = winPrecent;
            Ended = false;

            Map.OnCompletionChanged += () => OnCompletionChanged();
        }

        public void Load() => UnitsLoader?.Invoke();
        private Action UnitsLoader = null;
        public static Battle Create(string cfgName) => Create(ConfigManager.GetBattleConfig(cfgName));
        public static Battle Create(BattleConfig cfg)
        {
            var bt = new Battle(cfg.width, cfg.height, cfg.winPercent);
            bt.UnitsLoader = () => cfg.units.Travel(u => bt.AddUnitAt(u.type, new Vec2(u.x, u.y)));
            return bt;
        }

        public void ForceCursor(KeyValuePair<int, int> pt) => ForceCursor(pt.Key, pt.Value);
        public void ForceCursor(int x, int y)
        {
            CursorX = x;
            CursorY = y;
        }

        public bool TryMovingCursor(int dx, int dy, bool forceUnsafe = false)
        {
            var tx = CursorX + dx;
            var ty = CursorY + dy;
            if (tx < 0 || tx >= Map.Width || ty < 0 || ty >= Map.Height)
                return false;

            if (!forceUnsafe && Map[tx, ty] != BattleMap.GridType.Uncovered)
                return false;

            // not on the inside of uncoverd areas
            var onEdge = tx == 0 || tx == Map.Width - 1 || ty == 0 || ty == Map.Height - 1;

            var insideUncovered = !onEdge;
            FC.For2(-1, 2, -1, 2, (offsetX, offsetY) =>
            {
                var x = tx + offsetX;
                var y = ty + offsetY;
                if (offsetX == 0 && offsetY == 0)
                    return;

                insideUncovered = Map[x, y] == BattleMap.GridType.Uncovered;
            }, () => insideUncovered);

            // can move to the target positionc
            if (!insideUncovered)
            {
                CursorX = tx;
                CursorY = ty;
            }

            return !insideUncovered;
        }

        public void OnTimeElapsed(Fix64 te)
        {
            if (Ended)
                return;

            Map.OnTimeElapsed(te);

            var deadUnits = Map.AllUnits.Where(u => Map.IsBlocked(u.Pos));
            deadUnits.ToArray().Travel(Map.RemoveUnit);

            if (Map.Completion >= WinPrecentage)
            {
                Ended = true;
                OnWon?.Invoke();
            }
        }

        public bool IsCursorSafe => Map.IsBlocked(CursorPos);

        public void KillCursor()
        {
            Ended = true;
            OnLost?.Invoke();
        }
    }
}
