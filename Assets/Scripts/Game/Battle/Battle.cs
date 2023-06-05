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
        public event Action OnWon = null;
        public event Action OnLost = null;
        public event Action<int> OnCursorHurt = null;
        public event Action<int> OnCursorHpChanged = null;
        public event Action<int> OnCompletionChanged = null;
        public event Action OnTraceLineCompleted = null;

        public BattleMap Map { get; private set; }
        public float WinPrecentage { get; private set; } = 1;
        public bool WinOnAllKeyUnitsDead { get; private set; } = false;
        public bool Ended { get; private set; } = false;
        public Cursor Cursor;

        public Battle(int mapWidth, int mapHeight, float winPrecent, BattleConfig.CursorConfig cursorCfg )
        {
            Map = new(this, mapWidth, mapHeight);
            Cursor = new(cursorCfg.hp, cursorCfg.armor, cursorCfg.armorDec, cursorCfg.armorHurtReset, cursorCfg.armorCompletionBonus[0], cursorCfg.armorCompletionBonus[1]);
            WinPrecentage = winPrecent;
            Ended = false;

            Map.OnCompletionChanged += (n) => OnCompletionChanged?.Invoke(n);
        }

        private Action UnitsLoader = null;
        public static Battle Create(BattleConfig cfg)
        {
            var bt = new Battle(cfg.width, cfg.height, cfg.winPercent, cfg.cursor)
            {
                WinOnAllKeyUnitsDead = cfg.units.Any(u => u.isKeyUnit)
            };

            bt.UnitsLoader = () => cfg.units.Travel(u => bt.AddUnitAt(u.name, new Vec2(u.x, u.y), u.isKeyUnit));
            
            return bt;
        }

        public void Load()
        {
            UnitsLoader?.Invoke();
            OnCursorHpChanged?.Invoke(0);
            OnCompletionChanged?.Invoke(0);
        }

        public void SetbackCursor()
        {
            if (Cursor.TraceLine.Count > 0)
                Cursor.StepBack();
        }

        public bool TryMoveCursor(int dx, int dy, bool forceUnsafe)
        {
            var x = Cursor.X + dx;
            var y = Cursor.Y + dy;
            var n = Cursor.TraceLine.IndexOf(new(x, y));

            if (n >= 0 && n == Cursor.TraceLine.Count - 1)
            {
                Cursor.StepBack();
                return true;
            }
            else if (n < 0)
            {
                var inMoving = CanMoveCursorTo(dx, dy, out int tx, out int ty, forceUnsafe);
                if (inMoving)
                {
                    if (Cursor.TraceLine.Count == 0)
                        Cursor.StartPos = Cursor.Pos;

                    Cursor.SetPos(tx, ty);
                    if (Map[x, y] == BattleMap.GridType.Covered)
                        Cursor.AddTracePos(tx, ty);
                }

                if ((!inMoving || Map[x, y] == BattleMap.GridType.Uncovered) && forceUnsafe && Cursor.TraceLine.Count > 0)
                    DoTraceLineSplite();

                return inMoving;
            }

            return false;
        }

        public bool CanMoveCursorTo(int dx, int dy, out int toX, out int toY, bool forceUnsafe = false)
        {
            var tx = toX = Cursor.X + dx;
            var ty = toY = Cursor.Y + dy;
            if (tx < 0 || tx >= Map.Width || ty < 0 || ty >= Map.Height)
                return false;

            if (!forceUnsafe && Map[tx, ty] != BattleMap.GridType.Uncovered)
                return false;

            // not on the inside of uncoverd areas
            var onEdge = tx == 0 || tx == Map.Width - 1 || ty == 0 || ty == Map.Height - 1;

            // can move to the target position
            var canMoveTo = onEdge || !Map.IsAroundBy(tx, ty, BattleMap.GridType.Uncovered);
            return canMoveTo;
        }

        public void OnTimeElapsed(Fix64 te)
        {
            if (Ended)
                return;

            Map.OnTimeElapsed(te);
            Cursor.OnTimeElapsed(te);

            ResetCursorWhenSuspended();

            CheckingEnding();
        }

        private void CheckingEnding()
        {
            if (Ended)
                return;

            if (Map.Completion >= WinPrecentage || (WinOnAllKeyUnitsDead && !Map.AllUnits.Any(u => u.IsKeyUnit)))
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
            u.Kill();
            RemoveUnit(u);
        }

        public void RemoveUnit(BattleUnit u)
        {
            Map.RemoveUnit(u);
        }

        public void ResetCursorWhenSuspended()
        {
            if (Cursor.TraceLine.Count == 0 &&
                Map.InMapArea(Cursor.Pos)
                && Map[Cursor.Pos] == BattleMap.GridType.Covered
                && Map.IsAroundBy(Cursor.Pos, BattleMap.GridType.Covered))
            {
                var r = Map.FindNearestPoint(Cursor.Pos, 100,
                    (x, y) => Map[x, y] == BattleMap.GridType.Uncovered
                    && !Map.IsAroundBy(x, y, BattleMap.GridType.Uncovered));

                if (r.Key)
                    Cursor.SetPos(r.Value, true);
                else
                    Cursor.SetPos(Vec2.Zero, true);
            }
        }

        public bool IsCursorSafe => (Map.IsBlocked(Cursor.Pos) && Cursor.Armor > 0) || Cursor.CoolDown > 0;

        public void CursorHurt(int dhp = -1)
        {
            Cursor.CursorHurt(dhp);
            Cursor.Reset2StartPos();

            OnCursorHurt?.Invoke(dhp);
            OnCursorHpChanged?.Invoke(dhp);

            if (Cursor.Hp > 0)
                Cursor.CoolDown = 1;
        }

        public void DoTraceLineSplite()
        {
            var traceLine = Cursor.TraceLine;

            var n = Map.FillPts(traceLine, BattleMap.GridType.Uncovered);

            var setLeft = new List<KeyValuePair<int, int>>();
            var setRight = new List<KeyValuePair<int, int>>();

            FC.For(Cursor.TraceLine.Count(), (i) =>
            {
                var ptA = i == 0 ? Cursor.StartPos : Cursor.TraceLine[i - 1];
                var ptB = Cursor.TraceLine[i];

                int ax = (int)ptA.x;
                int ay = (int)ptA.y;
                int bx = (int)ptB.x;
                int by = (int)ptB.y;

                // Calculate the vector from a to b
                var vLeft = new Vec2(by - ay, ax - bx);
                var vRight = new Vec2(ay - by, bx- ax);

                var ptLeft = ptB + vLeft;
                var ptRight = ptB + vRight;

                if (Map.InMapArea(ptLeft) && Map[ptLeft] == BattleMap.GridType.Covered)
                    setLeft.Add(new((int)ptLeft.x, (int)ptLeft.y));

                if (Map.InMapArea(ptRight) && Map[ptRight] == BattleMap.GridType.Covered)
                    setRight.Add(new((int)ptRight.x, (int)ptRight.y));
            });

            OnTraceLineCompleted?.Invoke();
            Cursor.TraceLine.Clear();

            Map.CompeteFilling(setLeft, setRight);

            Map.AllUnits.ToList().Travel(u =>
            {
                if (Map[u.Pos] == BattleMap.GridType.Uncovered)
                    KillUnit(u);
            });

            Cursor.OnCompletion((Fix64)n / (Map.Width * Map.Height));
        }
    }
}
