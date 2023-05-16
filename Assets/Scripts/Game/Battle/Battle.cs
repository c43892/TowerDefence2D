using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using System;

namespace GalPanic
{
    public class Battle : ITimeDriven
    {
        public BattleMap Map { get; private set; }

        public int CursorX { get; private set; } = 0;
        public int CursorY { get; private set; } = 0;

        public Battle(int mapWidth, int mapHeight)
        {
            Map = new BattleMap(mapWidth, mapHeight);
            CursorX = CursorY = 0;
        }

        public void ForceCursor(KeyValuePair<int, int> pt) => ForceCursor(pt.Key, pt.Value);
        public void ForceCursor(int x, int y)
        {
            CursorX = x;
            CursorY = y;
        }

        public bool TryMovingCursor(int dx, int dy)
        {
            var tx = CursorX + dx;
            var ty = CursorY + dy;
            if (tx < 0 || tx >= Map.Width || ty < 0 || ty >= Map.Height)
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
            Map.OnTimeElapsed(te);
        }
    }
}
