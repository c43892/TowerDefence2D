using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Swift;
using Swift.Math;

namespace TowerDefance.Game
{

    // for a tower defence battle instance including the map, the towers, the enemies, the player, the game state, etc.
    public class TowerDefanceBattle : ITimeDriven
    {
        public BattleMap Map { get; protected set; }

        public ISpawningPoint SpawningPoint { get; protected set; }

        public virtual void AddUnitAt(BattleMapUnit unit, Vec2 pos)
        {
            Map.AddUnitAt(unit, pos);
        }

        public virtual void OnTimeElapsed(Fix64 te)
        {
            if (!Running)
                return;

            SpawningPoint.OnTimeElapsed(te);
            Map.OnTimeElapsed(te);
            Map.AllUnits.Where(u => u.Hp <= 0).ToArray().Travel(Map.RemoveUnit);

            CheckEnding();
        }

        public bool Running { get; protected set; }
        public event Action<TowerDefanceBattle> OnWon = null;
        public event Action<TowerDefanceBattle> OnFailed = null;

        public void Start()
        {
            Running = true;
        }

        public void Stop()
        {
            Running = false;
        }

        void CheckEnding()
        {
            if (!Map.AllUnits.Any(u => u is TowerBase && u.Hp > 0))
            {
                OnFailed?.Invoke(this);
                Stop();
            }
            else if (SpawningPoint.Done && !Map.AllUnits.Any(u => u is Enemy))
            {
                OnWon?.Invoke(this);
                Stop();
            }
        }
    }
}
