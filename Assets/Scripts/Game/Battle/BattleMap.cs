using Swift;
using Swift.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GalPanic
{
    public class BattleMap : ITimeDriven
    {
        public Action<BattleUnit> OnUnitAdded = null;
        public Action<BattleUnit> OnAbortToAddUnit = null;
        public Action<BattleUnit> OnUnitRemoved = null;
        public Action<BattleUnit> OnAbortToRemoveUnit = null;
        public Action<int> OnCompletionChanged = null;
        public Action<int, int, int, int, bool> OnObstaclesChanged = null;

        public enum GridType
        {
            Covered,
            Uncovered,

            TmpUncover1,
            TmpUncover2,
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public float Completion => (float)completionCounter / (Width * Height);
        public Battle Battle { get; private set; }

        private int completionCounter = 0;
        private readonly Dictionary<string, BattleUnit> units = new();
        private readonly GridType[,] grids;
        private readonly bool[,] obLayer;

        public BattleMap(Battle bt, int width, int height)
        {
            Battle = bt;
            Width = width;
            Height = height;
            grids = new GridType[width, height];
            obLayer = new bool[width, height];
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
        public GridType this[Vec2 pos] { get => grids[(int)pos.x, (int)pos.y]; }
        public bool InMapArea(Vec2 pos) => InMapArea((int)pos.x, (int) pos.y);
        public bool InMapArea(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

        public bool IsBlocked(Vec2 pos) => IsBlocked((int)pos.x, (int)pos.y);
        public bool IsBlocked(int x, int y) => x < 0 || x >= Width || y < 0 || y >= Height || grids[x, y] == GridType.Uncovered || obLayer[x, y];

        public bool IsObstacle(int x, int y) => obLayer[x, y];

        public int FillObstacleArea(int left, int width, int top, int height, bool isObstacle = true, Func<int, int, bool> filter = null)
        {
            var n = 0;
            FC.For2(left, left + width, top, top + height, (x, y) =>
            {
                if (InMapArea(x, y) && (filter == null || filter(x, y)))
                {
                    obLayer[x, y] = isObstacle;
                    n++;
                }
            });

            OnObstaclesChanged?.Invoke(left, width, top, height, isObstacle);
            return n;
        }

        public int FillArea(int left, int width, int top, int height, GridType fillType, Func<GridType, bool> filter = null)
        {
            var n = 0;
            FC.For2(left, left + width, top, top + height, (x, y) =>
            {
                if (InMapArea(x, y) && (filter == null || filter(grids[x, y])))
                {
                    var oldValue = grids[x, y];
                    grids[x, y] = fillType;

                    if (oldValue == GridType.Covered && fillType == GridType.Uncovered)
                        n++;
                    else if (oldValue == GridType.Uncovered && fillType == GridType.Covered)
                        n--;
                }
            });

            completionCounter += n;
            OnCompletionChanged?.Invoke(n);
            return n;
        }

        public int FillPts(IEnumerable<Vec2> pts, GridType fillType)
        {
            var n = 0;
            pts.Travel(pt =>
            {
                grids[(int)pt.x, (int)pt.y] = fillType;
                n++;
            });

            completionCounter += n;
            OnCompletionChanged?.Invoke(n);
            return n;
        }

        public int CompeteFilling(List<KeyValuePair<int, int>> seedsLeft, List<KeyValuePair<int, int>> seedsRight)
        {
            bool fillable(int x, int y) => InMapArea(x, y) && grids[x, y] == GridType.Covered;
            KeyValuePair<int, int>[] neighbours(int cx, int cy)
            {
                return new KeyValuePair<int, int>[]
                {
                    new KeyValuePair<int, int>(cx - 1, cy),
                    new KeyValuePair<int, int>(cx + 1, cy),
                    new KeyValuePair<int, int>(cx, cy - 1),
                    new KeyValuePair<int, int>(cx, cy + 1)
                };
            }

            var cancelled = false;

            void fill1(int x, int y) => grids[x, y] = GridType.TmpUncover1;
            void fill2(int x, int y) => grids[x, y] = GridType.TmpUncover2;

            bool cancelCheck1(int x, int y)
            {
                if (InMapArea(x, y) && grids[x, y] == GridType.TmpUncover2)
                    cancelled = true;

                return cancelled;
            }

            bool cancelCheck2(int x, int y)
            {
                if (InMapArea(x, y) && grids[x, y] == GridType.TmpUncover1)
                    cancelled = true;

                return cancelled;
            }

            var filler1 = GeoUtils.Fill2DAreaCoroutine(seedsLeft, fillable, cancelCheck1, neighbours, fill1);
            var filler2 = GeoUtils.Fill2DAreaCoroutine(seedsRight, fillable, cancelCheck2, neighbours, fill2);

            while (filler1.MoveNext() && filler2.MoveNext())
                ;

            var n = 0;
            if (cancelled)
            {
                filler1.Current?.Travel(pt => grids[pt.Key, pt.Value] = GridType.Covered);
                filler2.Current?.Travel(pt => grids[pt.Key, pt.Value] = GridType.Covered);
            }
            else
            {
                var toRevert = filler1.MoveNext() ? filler1.Current : filler2.Current;
                var toComplete = toRevert == filler1.Current ? filler2.Current : filler1.Current;

                toRevert?.Travel(pt => grids[pt.Key, pt.Value] = GridType.Covered);
                toComplete?.Travel(pt => grids[pt.Key, pt.Value] = GridType.Uncovered);

                if (toComplete != null)
                {
                    completionCounter += toComplete.Count;
                    n = toComplete.Count;
                }
            }

            OnCompletionChanged?.Invoke(n);
            return n;
        }

        public bool IsAroundBy(Vec2 pos, GridType type) => IsAroundBy((int)pos.x, (int)pos.y, type);
        public bool IsAroundBy(int x, int y, GridType type)
        {
            var notAroundBy = false;
            FC.For2(-1, 2, -1, 2, (offsetX, offsetY) =>
            {
                if (offsetX == 0 && offsetY == 0)
                    return;

                var tx = x + offsetX;
                var ty = y + offsetY;

                notAroundBy = InMapArea(tx, ty) && this[tx, ty] != type;
            }, () => !notAroundBy);

            return !notAroundBy;
        }

        public void OnTimeElapsed(Fix64 te)
        {
            AllUnits.ToList().Travel(u =>
            {
                // the unit may be removed in this process
                if (units.ContainsKey(u.UID))
                    u.OnTimeElapsed(te);
            });
        }
    }
}
