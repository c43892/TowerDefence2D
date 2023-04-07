using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Swift;
using Swift.Math;

namespace TowerDefance.Game
{

    // for a tower defence battle instance including the map, the towers, the enemies, the player, the game state, etc.
    public class TowerDefanceBattle : IFrameDrived
    {
        public BattleMap Map { get; protected set; }

        public ISpawningPoint SpawningPoint { get; protected set; }

        protected readonly StateMachineManager smm = new();

        public virtual void AddUnitAt(BattleUnit unit, Vec2 pos)
        {
            Map.AddUnitAt(unit, pos);
            smm.Add(unit.CreateAI());
        }

        public virtual void OnTimeElapsed(int te)
        {
            if (!Running)
                return;

            SpawningPoint.OnTimeElapsed(te);
            smm.OnTimeElapsed(te);
            Map.OnTimeElapsed(te);

            var toRemove = Map.AllUnits.Where(u => u.Hp <= 0).ToArray();
            toRemove.Travel((u) =>
            {
                Map.RemoveUnit(u);
                smm.Del(u.UID);
            });

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
