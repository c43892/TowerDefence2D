using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Swift.Math;
using static TowerDefance.Game.IBuff;

namespace TowerDefance.Game
{
    public class BuffSpeedReduction : BuffOnce
    {
        public BuffSpeedReduction(Fix64 waitingTime)
            : base(waitingTime)
        {
        }

        public override string ID { get; } = "BuffSpeedReduction";

        public interface IBuffSpeedReductionTarget : IBuffOwner
        {
            public Fix64 Speed { get; set; }
            public Fix64 MaxSpeed { get; set; }
        }

        public Fix64 A { get; set; } = 1;
        public Fix64 B { get; set; } = 0;

        public Fix64 Duration { get; set; }

        protected override void OnEffect()
        {
            var owner = (Owner as IBuffSpeedReductionTarget);
            owner.Speed = owner.MaxSpeed * A + B;

            // add another buff to cancel the effect after a while
            owner.AddBuff(new BuffSpeedReductionEnd(Duration));
        }

        class BuffSpeedReductionEnd : BuffOnce
        {
            public BuffSpeedReductionEnd(Fix64 waitingTime)
                : base(waitingTime)
            {
            }

            public override string ID { get; } = "BuffSpeedReductionEnd";

            protected override void OnEffect()
            {
                var owner = (Owner as IBuffSpeedReductionTarget);
                owner.Speed = owner.MaxSpeed;
            }
        }
    }
}
