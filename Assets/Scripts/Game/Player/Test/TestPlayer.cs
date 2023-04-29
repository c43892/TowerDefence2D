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
        private readonly List<string> ValidTower = new();

        public TestPlayer(int maxNumTowers)
        {
            FC.For(maxNumTowers, i => ValidTower.Add("Archer"));
        }

        public string[] AllValidTowerTypes { get => ValidTower.ToArray(); }
    }
}
