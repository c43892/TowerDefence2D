using Swift;
using Swift.Math;

namespace TowerDefance.Game
{
    public interface ISpawningPoint : IFrameDrived
    {
        void Start();

        void Stop();

        bool Done { get; }
    }
}
