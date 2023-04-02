using System;
using System.Collections;
using System.Collections.Generic;
using Swift;
using Swift.Math;

namespace TowerDefance.Game
{

    // for a tower defence battle instance including the map, the towers, the enemies, the player, the game state, etc.
    public class TestBattle : TowerDefanceBattle
    {
        public void Init()
        {
            Map = new BattleMap(21, 11, (x, y) => BattleMap.GridType.None);
            var enemyMovingPath = new List<Vec2>()
            {
                new Vec2(0, 5), new Vec2(5, 5),
                new Vec2(5, 8), new Vec2(15, 8),
                new Vec2(15, 5), new Vec2(20, 5),
            };

            // the enemy
            var e = new Enemy(RandomUtils.GetRandomString("enemy_"), 2, 10, 0);
            Map.AddUnitAt(e, 0, 5);
            smm.Add(e.CreateAI(enemyMovingPath));

            // the tower
            var s = new SkillAttackingSingleTargetPhysical("PhysicalSingleAttack", 8);
            var t = new Tower(RandomUtils.GetRandomString("tower_"), 3, 1);
            s.Owner = t;
            t.Skill = s;
            Map.AddUnitAt(t, 10, 5);
            smm.Add(t.CreateAI());
        }

        public override void OnTimeElapsed(int te)
        {
            base.OnTimeElapsed(te);
        }
    }
}
