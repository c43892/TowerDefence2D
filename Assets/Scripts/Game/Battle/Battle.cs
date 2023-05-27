using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using System;
using System.Linq;
using Unity.VisualScripting;
using System.Diagnostics;

namespace GalPanic
{
    public class Battle : ITimeDriven
    {
        public Action OnWon = null;
        public Action OnLost = null;
        public Action OnCursorHurt = null;
        public Action OnCompletionChanged = null;
        public Action OnTraceLineCompleted = null;

        public BattleMap Map { get; private set; }
        public float WinPrecentage { get; private set; } = 1;
        public bool Ended { get; private set; } = false;
        public Cursor Cursor;

        public Battle(int mapWidth, int mapHeight, int cursorHp = 1, float winPrecent = 0.5f)
        {
            Map = new BattleMap(this, mapWidth, mapHeight);
            Cursor = new(cursorHp);
            WinPrecentage = winPrecent;
            Ended = false;

            Map.OnCompletionChanged += () => OnCompletionChanged();
        }

        public void Load() => UnitsLoader?.Invoke();
        private Action UnitsLoader = null;
        public static Battle Create(BattleConfig cfg)
        {
            var bt = new Battle(cfg.width, cfg.height, cfg.cursorHp, cfg.winPercent);
            bt.UnitsLoader = () => cfg.units.Travel(u => bt.AddUnitAt(u.type, new Vec2(u.x, u.y), u.isKeyUnit));
            return bt;
        }

        public bool TryMovingCursor(int dx, int dy, out int toX, out int toY, bool forceUnsafe = false)
        {
            var tx = toX = Cursor.X + dx;
            var ty = toY = Cursor.Y + dy;
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

            // can move to the target position
            if (!insideUncovered)
            {
                toX = tx;
                toY = ty;
            }

            return !insideUncovered;
        }

        public void OnTimeElapsed(Fix64 te)
        {
            if (Ended)
                return;

            Map.OnTimeElapsed(te);
            Cursor.OnTimeElapsed(te);

            CheckingEnding();
        }

        private void CheckingEnding()
        {
            if (Ended)
                return;

            if (Map.Completion >= WinPrecentage || !Map.AllUnits.Any(u => u.IsKeyUnit))
            {
                Ended = true;
                OnWon?.Invoke();
            }
            else if (Cursor.Hp <= 0)
            {
                Ended = true;
                OnLost?.Invoke();
            }
        }

        public void KillUnit(BattleUnit u)
        {
            Map.RemoveUnit(u);
        }

        public bool IsCursorSafe => Map.IsBlocked(Cursor.Pos) || Cursor.CoolDown > 0;

        public void CursorHurt(int dhp = -1)
        {
            Cursor.CursorHurt(dhp);
            Cursor.Reset2StartPos();
            OnCursorHurt?.Invoke();

            if (Cursor.Hp > 0)
                Cursor.CoolDown = 1;
        }

        public void DoTraceLineSplite()
        {
            var traceLine = Cursor.TraceLine;

            // find a straight line at least having 3 pts
            var pt0 = traceLine.Count > 1 ? traceLine[^2] : Cursor.StartPos;
            var pt1 = traceLine[^1];

            // check the changes on x&y directions
            var onX = (int)(pt1.x - pt0.x);
            var onY = (int)(pt1.y - pt0.y);

            Map.FillPts(traceLine, BattleMap.GridType.Uncovered);

            OnTraceLineCompleted?.Invoke();
            Cursor.TraceLine.Clear();

            if (onX != 0)
                Map.CompeteFilling((int)pt1.x, (int)pt1.y - 1, (int)pt1.x, (int)pt1.y + 1);
            else if (onY != 0)
                Map.CompeteFilling((int)pt1.x - 1, (int)pt1.y, (int)pt1.x + 1, (int)pt1.y);

            Map.AllUnits.ToList().Travel(u =>
            {
                if (Map[u.Pos] == BattleMap.GridType.Uncovered)
                    KillUnit(u);
            });
        }
    }
}
