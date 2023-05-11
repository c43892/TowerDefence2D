using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using System;

namespace GalPanic
{
    public class BattleMap
    {
        public enum GridType
        {
            Covered,
            Uncovered,
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private readonly GridType[,] grids;

        public BattleMap(int width, int height)
        {
            Width = width;
            Height = height;
            grids = new GridType[width, height];

            // fill borders
            FC.For(Width, (x) => grids[x, 0] = grids[x, height - 1] = GridType.Uncovered);
            FC.For(Height, (y) => grids[0, y] = grids[width - 1, y] = GridType.Uncovered);
        }

        public GridType this[int x, int y] { get => grids[x, y]; }

        public void FillArea(int left, int width, int top, int height, GridType fillType)
        {
            FC.For2(left, left + width, top, top + height, (x, y) => grids[x, y] = fillType);
        }

        public void FillPts(IEnumerable<KeyValuePair<int, int>> pts, GridType fillType)
        {
            foreach (var pt in pts)
                grids[pt.Key, pt.Value] = fillType;
        }

        public void CompeteFilling(int cx1, int cy1, int cx2, int cy2)
        {
            bool fillable(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height && grids[x, y] == GridType.Covered;
            KeyValuePair<int, int>[] neighbours(int cx, int cy)
            {
                var ns = new List<KeyValuePair<int, int>>();
                if (fillable(cx - 1, cy)) ns.Add(new KeyValuePair<int, int>(cx - 1, cy));
                if (fillable(cx + 1, cy)) ns.Add(new KeyValuePair<int, int>(cx + 1, cy));
                if (fillable(cx, cy - 1)) ns.Add(new KeyValuePair<int, int>(cx, cy - 1));
                if (fillable(cx, cy + 1)) ns.Add(new KeyValuePair<int, int>(cx, cy + 1));

                return ns.ToArray();
            }
            void fill(int x, int y) => grids[x, y] = GridType.Uncovered;

            var filler1 = GeoUtils.Fill2DAreaCoroutine(cx1, cy1, fillable, neighbours, fill);
            var filler2 = GeoUtils.Fill2DAreaCoroutine(cx2, cy2, fillable, neighbours, fill);

            while (filler1.MoveNext() && filler2.MoveNext())
                ;

            var toRevert = filler1.MoveNext() ? filler1.Current : filler2.Current;
            foreach (var pt in toRevert)
                grids[pt.Key, pt.Value] = GridType.Covered;
        }
    }
}
