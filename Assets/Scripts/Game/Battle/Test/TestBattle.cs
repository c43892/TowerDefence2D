using System;
using System.Collections;
using System.Collections.Generic;
using Swift;
using Swift.Math;
using static TowerDefance.ISkillAttacking;

namespace TowerDefance.Game
{

    // for a tower defence battle instance including the map, the towers, the enemies, the player, the game state, etc.
    public class TestBattle : TowerDefanceBattle
    {
        public override void Init()
        {
            Map = new BattleMap(21, 11, (x, y) => BattleMap.GridType.None);
            var enemyMovingPath = new List<Vec2>()
            {
                new Vec2(0, 5), new Vec2(5, 5),
                new Vec2(5, 8), new Vec2(15, 8),
                new Vec2(15, 5), new Vec2(20, 5),
            };

            // the enemy
            var e100 = new Enemy(RandomUtils.GetRandomString("enemy_"), 2, 100, 0, 0, 1, 0);
            var s100 = new SkillAttackingTargets("PhysicalSingleAttack", 1);
            e100.Skill = s100;
            s100.Owner = e100;
            e100.ValidTargetTypes = new Type[] { typeof(TowerBase) };
            Map.AddUnitAt(e100, 0, 5);
            smm.Add(e100.CreateAI(enemyMovingPath));

            var e3 = new Enemy(RandomUtils.GetRandomString("enemy_"), 3, 3, 0, 0, 1, 0);
            var s3 = new SkillAttackingTargets("SkillAttackingTargets", 1);
            e3.Skill = s3;
            s3.Owner = e3;
            e3.ValidTargetTypes = new Type[] { typeof(TowerBase) };
            Map.AddUnitAt(e3, 0, 5);
            smm.Add(e3.CreateAI(enemyMovingPath));

            // the tower
            var s7 = new SkillAttackingTargets("SkillAttackingTargets", 7, 2);
            // var s7a = new SkillAttackingArea("SkillAttackingArea", 7, new Circle(5), 10);
            var t = new Tower(RandomUtils.GetRandomString("tower_"), 1, 0, 1);
            s7.Owner = t;
            t.Skill = s7;
            Map.AddUnitAt(t, 10, 5);
            smm.Add(t.CreateAI());

            // the tower base
            var b = new TowerBase(RandomUtils.GetRandomString("towerbase_"), 0, 0, 5);
            Map.AddUnitAt(b, 20, 5);

            base.Init();
        }

        public override void OnTimeElapsed(int te)
        {
            base.OnTimeElapsed(te);
        }
    }
}
