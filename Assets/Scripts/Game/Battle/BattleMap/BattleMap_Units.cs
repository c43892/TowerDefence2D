using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Swift;
using Swift.Math;

namespace TowerDefance.Game
{

    // battle map units related parts
    public partial class BattleMap
    {
        public interface IUnit
        {
            string UID { get; }
            public BattleMap Map { get; set; }
            public Vec2 Pos { get; set; }
            public Fix64 Dir { get; set; }
            public Fix64 Hp { get; set; }
            public string Type { get; }
        }

        readonly List<IUnit> units = new();

        void InitUnits()
        {
        }

        public bool IsOccupiedAt(Vec2 pos)
        {
            if (!Walkable(pos))
                return false;

            foreach (var u in units)
                if (u.Pos == pos)
                    return false;

            return true;
        }

        public IUnit[] GetUnitsAt(Vec2 pos)
        {
            var lst = new List<IUnit>();
            foreach (var u in units)
            {
                if (u.Pos == pos)
                    lst.Add(u);
            }

            return lst.ToArray();
        }

        public void AddUnitAt(IUnit unit, int x, int y) => AddUnitAt(unit, new Vec2(x, y));
        public void AddUnitAt(IUnit unit, Vec2 pos)
        {
            if (!Walkable(pos))
                throw new Exception($"it's not empty at {pos}");

            if (units.Contains(unit))
                throw new Exception($"this unit is already on the map");

            unit.Map = this;
            unit.Pos = pos;
            units.Add(unit);

            OnUnitAdded?.Invoke(unit);
            OnMapUnitAdded?.Invoke(this,unit);
        }

        public void RemoveUnit(IUnit unit)
        {
            if (!units.Contains(unit))
                throw new Exception($"unit {unit.UID} is not on the map. can not remove it");

            units.Remove(unit);
            OnUnitRemoved?.Invoke(unit);
            OnMapUnitRemoved?.Invoke(this, unit);
        }

        public void ForEachUnit(Action<IUnit> f)
        {
            foreach (var u in units)
                f(u);
        }

        public IUnit[] AllUnits { get => units.ToArray(); }

        public static event Action<BattleMap, IUnit> OnMapUnitAdded = null;
        public static event Action<BattleMap, IUnit> OnMapUnitRemoved = null;

        public event Action<IUnit> OnUnitAdded = null;
        public event Action<IUnit> OnUnitRemoved = null;

        public void UpdateAllUnits(int te)
        {
            units.Where(u => u is IFrameDrived).Cast<IFrameDrived>().Travel(u => u.OnTimeElapsed(te));
        }
    }
}
