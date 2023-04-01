using System;
using System.Collections.Generic;
using Swift;
using Swift.Math;

namespace TowerDefance
{
    /// <summary>
    /// 扩展 Unit 的 AI 行为
    /// </summary>
    public static partial class AIUnitExt
    {
        public interface IUnit
        {
            string UID { get; }
            Vec2 Pos { get; set; }
            Fix64 Dir { get; set; }
        }

        // 保持单一状态
        public static StateMachine SimpleState(this IUnit u, Action<State, Fix64> onRun = null)
        {
            var sm = new StateMachine(u.UID);
            sm.NewState("default").AsDefault().Run(onRun);
            sm.Trans().From("default").To("default").When((st) => false);
            return sm;
        }

        // 等待一段时间
        public static StateMachine StartWaiting4Time(this IUnit u, Fix64 time, Action onEnd, Action<Fix64> onTimeElapsed = null)
        {
            var sm = new StateMachine(u.UID);
            sm.NewState("waiting").Run((st, dt) =>
            {
                time -= dt;
                if (time > 0)
                    onTimeElapsed.SC(dt);
                else
                    onEnd.SC();
            }).AsDefault();
            sm.NewState("ended").Run(null);
            sm.Trans().From("waiting").To("ended").When((st) => time <= 0);
            sm.Trans().From("ended").To("ended").When(null);
            return sm;
        }
    }
}
