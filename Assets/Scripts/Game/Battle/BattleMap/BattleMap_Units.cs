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
        readonly List<BattleMapUnit> units = new();

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

        public BattleMapUnit[] GetUnitsAt(Vec2 pos)
        {
            var lst = new List<BattleMapUnit>();
            foreach (var u in units)
            {
                if (u.Pos == pos)
                    lst.Add(u);
            }

            return lst.ToArray();
        }

        public void AddUnitAt(BattleMapUnit unit, int x, int y) => AddUnitAt(unit, new Vec2(x, y));
        public void AddUnitAt(BattleMapUnit unit, Vec2 pos)
        {
            if (!Walkable(pos))
                throw new Exception($"it's not empty at {pos}");

            if (units.Contains(unit))
                throw new Exception($"this unit is already on the map");

            unit.Map = this;
            unit.Pos = pos;
            units.Add(unit);

            unit.StartAI();

            OnUnitAdded?.Invoke(unit);
            OnMapUnitAdded?.Invoke(this, unit);
        }

        public void RemoveUnit(BattleMapUnit unit)
        {
            if (!units.Contains(unit))
                throw new Exception($"unit {unit.UID} is not on the map. can not remove it");

            units.Remove(unit);
            OnUnitRemoved?.Invoke(unit);
            OnMapUnitRemoved?.Invoke(this, unit);
        }

        public BattleMapUnit[] AllUnits { get => units.ToArray(); }

        public static event Action<BattleMap, BattleMapUnit> OnMapUnitAdded = null;
        public static event Action<BattleMap, BattleMapUnit> OnMapUnitRemoved = null;

        public event Action<BattleMapUnit> OnUnitAdded = null;
        public event Action<BattleMapUnit> OnUnitRemoved = null;

        public void UpdateAllUnits(Fix64 te)
        {
            units.Where(u => u is ITimeDriven).Cast<ITimeDriven>().Travel(u => u.OnTimeElapsed(te));
        }
    }
}
