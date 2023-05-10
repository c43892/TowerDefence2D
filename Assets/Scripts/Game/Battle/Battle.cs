using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using System;

namespace GalPanic
{
    public class Battle
    {
        public BattleMap Map { get; private set; }

        public int CursorX { get; private set; } = 0;
        public int CursorY { get; private set; } = 0;

        public Battle(int mapWidth, int mapHeight)
        {
            Map = new BattleMap(mapWidth, mapHeight);
            CursorX = CursorY = 0;
        }


        public bool TryMovingCursor(int dx, int dy)
        {
            var tx = CursorX + dx;
            var ty = CursorY + dy;
            if (tx < 0 || tx >= Map.Width || ty < 0 || ty >= Map.Height)
                return false;

            // not on the inside of uncoverd areas
            var insideUncovered = true;
            FC.For2(-1, 2, -1, 2, (offsetX, offsetY) =>
            {
                var x = tx + offsetX;
                var y = ty + offsetY;
                insideUncovered = x == 0 || x == Map.Width || y == 0 || y == Map.Height || Map[x, y] == BattleMap.GridType.Covered;
            }, () => !insideUncovered);

            // can move to the target position
            if (!insideUncovered)
            {
                CursorX = tx;
                CursorY = ty;
            }

            return !insideUncovered;
        }
    }
}
