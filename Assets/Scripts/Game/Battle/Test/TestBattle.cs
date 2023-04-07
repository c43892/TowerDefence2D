using System;
using System.Collections;
using System.Collections.Generic;
using Swift;
using Swift.Math;
using UnityEditor.Experimental.GraphView;
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

            SpawningPoint = new SpawningPointSequential(this, new Vec2(0, 5), new BattleUnit[]
            {
                new Enemy(RandomUtils.GetRandomString("enemy_"), 2, 3, 0, 0, 1, 0, enemyMovingPath)
                {
                    Skill = new SkillAttackingTargets("PhysicalSingleAttack", 1),
                    ValidTargetTypes = new Type[] { typeof(TowerBase) }
                },

                new Enemy(RandomUtils.GetRandomString("enemy_"), 2, 3, 0, 0, 1, 0, enemyMovingPath)
                {
                    Skill = new SkillAttackingTargets("PhysicalSingleAttack", 1),
                    ValidTargetTypes = new Type[] { typeof(TowerBase) }
                },

                new Enemy(RandomUtils.GetRandomString("enemy_"), 1, 100, 0, 0, 1, 0, enemyMovingPath)
                {
                    Skill = new SkillAttackingTargets("PhysicalSingleAttack", 1),
                    ValidTargetTypes = new Type[] { typeof(TowerBase) }
                },

                new Enemy(RandomUtils.GetRandomString("enemy_"), 2, 3, 0, 0, 1, 0, enemyMovingPath)
                {
                    Skill = new SkillAttackingTargets("PhysicalSingleAttack", 1),
                    ValidTargetTypes = new Type[] { typeof(TowerBase) }
                },

                new Enemy(RandomUtils.GetRandomString("enemy_"), 2, 3, 0, 0, 1, 0, enemyMovingPath)
                {
                    Skill = new SkillAttackingTargets("PhysicalSingleAttack", 1),
                    ValidTargetTypes = new Type[] { typeof(TowerBase) }
                }
            }, 0.5f);

            // the tower
            var s7 = new SkillAttackingTargets("SkillAttackingTargets", 7, 2);
            var t = new Tower(RandomUtils.GetRandomString("tower_"), 1, 0, 1);
            t.Skill = s7;
            AddUnitAt(t, new Vec2(10, 5));

            // the tower base
            var b = new TowerBase(RandomUtils.GetRandomString("towerbase_"), 0, 0, 5);
            AddUnitAt(b, new Vec2(20, 5));

            base.Init();

            SpawningPoint.Start();
        }
    }
}
