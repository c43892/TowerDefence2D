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

            var e = new Enemy(RandomUtils.GetRandomString("enemy_"), 1);
            Map.AddUnitAt(e, 0, 5);

            var enemyPath = new List<Vec2>() { new Vec2(0, 5), new Vec2(21, 5) };
            smm.Add(e.AIMoving(enemyPath));
        }

        public override void OnTimeElapsed(int te)
        {
            base.OnTimeElapsed(te);
        }
    }
}
