using Swift;
using Swift.Math;

namespace TowerDefance.Game
{
    public interface ISpawningPoint : ITimeDriven
    {
        void Start();

        void Stop();

        bool Done { get; }
    }
}
