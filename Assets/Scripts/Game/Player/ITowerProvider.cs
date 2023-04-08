using Swift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefance.Game
{
    public interface ITowerProvider
    {
        public event Action<string[]> ValidTowerUpdated;
        public string[] AllValidTowerTypes { get; }
        public Tower CreateTower(string type);
    }
}
