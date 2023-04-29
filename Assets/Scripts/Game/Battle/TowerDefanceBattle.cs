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
            SpawningPoint.Start();
            Running = true;
        }

        public void Stop()
        {
            SpawningPoint.Stop();
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

        public static TowerDefanceBattle Create(BattleConfig cfg)
        {
            var bt = new TowerDefanceBattle();

            var path = cfg.path.Select(p => new Vec2(p.x, p.y)).ToArray();

            bt.Map = new BattleMap(cfg.size.x, cfg.size.y, (x, y) => BattleMap.GridType.None);

            bt.AddUnitAt(new TowerBase(BattleMapUnit.IdGen("TowerBase"), "TowerBase", cfg.baseInfo.phyDefence, cfg.baseInfo.magDefence, cfg.baseInfo.maxHp), new Vec2(cfg.baseInfo.pos.x, cfg.baseInfo.pos.y));

            var enemies = cfg.enemies.Select(cfg => Enemy.Create(ConfigManager.GetEnemyConfig(cfg), path)).ToArray();
            var spawningPt = new SpawningPointSequential(bt, cfg.spawningPos.ToVec2(), enemies, cfg.spawningInterval);
            bt.SpawningPoint = spawningPt;

            return bt;
        }
    }
}
