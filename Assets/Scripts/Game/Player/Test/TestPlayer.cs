using Swift;
using Swift.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Diagnostics;

namespace TowerDefance.Game
{
    public class TestPlayer : ITowerProvider
    {
        public event Action<string[]> ValidTowersUpdated;

        private readonly List<string> ValidTower = new();

        public TestPlayer(int maxNumTowers)
        {
            FC.For(maxNumTowers, i => ValidTower.Add("Tower01"));
        }

        public string[] AllValidTowerTypes { get => ValidTower.ToArray(); }

        public Tower CreateTower(string type)
        {
            if (!ValidTower.Contains(type))
                return null;

            ValidTower.Remove(type);
            return new Tower(RandomUtils.GetRandomString("tower_" + type), 1, 0, 1)
            {
                Skill = new SkillAttackingTargets("PhysicalSingleAttack", 6, 1),
                ValidTargetTypes = new Type[] { typeof(Enemy) },
                Type = "tower01"
            };
        }
    }
}
