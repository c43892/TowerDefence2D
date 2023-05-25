using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using System;

namespace GalPanic
{
    public class BattleMap : ITimeDriven
    {
        public Action<BattleUnit> OnUnitAdded = null;
        public Action<BattleUnit> OnUnitRemoved = null;
        public Action OnCompletionChanged = null;

        public enum GridType
        {
            Covered,
            Uncovered,
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public float Completion => (float)completionCounter / (Width * Height);
        public Battle Battle { get; private set; }

        private int completionCounter = 0;
        private readonly Dictionary<string, BattleUnit> units = new();
        private readonly GridType[,] grids;

        public BattleMap(Battle bt, int width, int height)
        {
            Battle = bt;
            Width = width;
            Height = height;
            grids = new GridType[width, height];

            // fill borders
            // FC.For(Width, (x) => grids[x, 0] = grids[x, height - 1] = GridType.Uncovered);
            // FC.For(Height, (y) => grids[0, y] = grids[width - 1, y] = GridType.Uncovered);
        }

        public IEnumerable<BattleUnit> AllUnits => units.Values;

        public void AddUnit(BattleUnit u)
        {
            if (units.ContainsKey(u.UID))
                throw new Exception("unit already exists");

            units.Add(u.UID, u);
            OnUnitAdded?.Invoke(u);
        }

        public void RemoveUnit(BattleUnit u)
        {
            if (!units.ContainsKey(u.UID))
                throw new Exception("unit doesn't exist");

            units.Remove(u.UID);
            OnUnitRemoved?.Invoke(u);
        }

        public GridType this[int x, int y] { get => grids[x, y]; }

        public bool IsBlocked(Vec2 pos) => IsBlocked((int)pos.x, (int)pos.y);
        public bool IsBlocked(int x, int y) => x < 0 || x >= Width || y < 0 || y >= Height || grids[x, y] == GridType.Uncovered;

        public void FillArea(int left, int width, int top, int height, GridType fillType)
        {
            FC.For2(left, left + width, top, top + height, (x, y) => grids[x, y] = fillType);
        }

        public void FillPts(IEnumerable<Vec2> pts, GridType fillType)
        {
            foreach (var pt in pts)
                grids[(int)pt.x, (int)pt.y] = fillType;
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
            var toComplete = toRevert == filler1 ? filler2 : filler1;

            foreach (var pt in toRevert)
                grids[pt.Key, pt.Value] = GridType.Covered;

            completionCounter += toComplete.Current.Count;

            OnCompletionChanged?.Invoke();
        }

        public void OnTimeElapsed(Fix64 te)
        {
            AllUnits.Travel(u => u.OnTimeElapsed(te));
        }
    }
}
