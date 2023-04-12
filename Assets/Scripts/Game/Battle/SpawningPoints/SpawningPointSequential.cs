using Swift;
using Swift.Math;

namespace TowerDefance.Game
{
    public class SpawningPointSequential : ISpawningPoint
    {
        Fix64 SpawningInterval { get; set; }
        TowerDefanceBattle Bt { get; set; }
        Vec2 StartPos { get; set; }
        public BattleMapUnit[] Units { get; private set; }

        public SpawningPointSequential(TowerDefanceBattle bt, Vec2 startPos, BattleMapUnit[] units, Fix64 spawningInterval)
        {
            SpawningInterval = spawningInterval;
            Bt = bt;
            Units = units;
            StartPos = startPos;
            timeElapsed = SpawningInterval;
            nextUnitIndex = 0;
        }

        Fix64 timeElapsed = 0;
        bool started = false;
        int nextUnitIndex = 0;

        public void OnTimeElapsed(Fix64 te)
        {
            if (!started || Done)
                return;

            timeElapsed -= te;
            if (timeElapsed <= 0)
            {
                timeElapsed = SpawningInterval;
                var unit = Units[nextUnitIndex++];
                Bt.AddUnitAt(unit, StartPos);
            }
        }

        public void Start()
        {
            started = true;
        }

        public void Stop()
        {
            started = false;
        }

        public bool Done { get => nextUnitIndex >= Units.Length; }
    }
}
