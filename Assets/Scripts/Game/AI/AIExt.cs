using System;
using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json.Serialization;
using Swift;
using Swift.Math;

namespace GalPanic
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
            Vec2 Dir { get; set; }
            Fix64 Radius { get; }
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

        public static StateMachine MoveAndReflect(this IUnit u, Vec2 initDir, Func<int, int, bool> validPos)
        {
            var dir = initDir;
            return u.SimpleState((st, te) =>
            {
                var newPos = u.Pos + dir * te;
                var x = (int)newPos.x;
                var y = (int)newPos.y;

                if (validPos(x, y))
                    u.Pos = newPos;
                else
                {
                    // check reflecting direction

                    var h = !validPos(x - 1, y) && !validPos(x + 1, y);
                    var v = !validPos(x, y - 1) && !validPos(x, y + 1);

                    if (h && !v)
                        dir = new Vec2(dir.x, -dir.y);
                    else if (!h && v)
                        dir = new Vec2(-dir.x, dir.y);
                    else
                        dir = new Vec2(-dir.x, -dir.y);
                }
            });
        }

        public static StateMachine RunPeriodically(this IUnit u, Fix64 interval, Action run)
        {
            var sm = new StateMachine(u.UID);

            var waiting = interval;
            sm.NewState("waiting")
                .Run((_, te) => waiting -= te);

            sm.NewState("run")
                .Run((_, te) =>
                {
                    run();
                    waiting = interval;
                });

            sm.Trans().From("waiting").To("run").When((_) => waiting <= 0);
            sm.Trans().From("run").To("waiting").When((_) => waiting > 0);

            return sm;
        }

        public static StateMachine OnCollide(this IUnit u, Func<Vec2> checkPos, Action onCollide)
        {
            return u.SimpleState((st, te) =>
            {
                var pos = u.Pos;
                var x = (int)pos.x;
                var y = (int)pos.y;

                var targetPos = checkPos();
                var l = (int)(targetPos.x - u.Radius);
                var r = (int)(targetPos.x + u.Radius);
                var t = (int)(targetPos.y - u.Radius);
                var b = (int)(targetPos.y + u.Radius);

                if (x >= l && x <= r && y >= t && y <= b)
                    onCollide?.Invoke();
            });
        }
    }
}
