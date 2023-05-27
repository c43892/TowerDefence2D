using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Newtonsoft.Json.Serialization;
using Swift;
using Swift.Math;
using UnityEngine.UI;

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

        public static StateMachine MoveForward(this IUnit u, Func<Vec2> getDir, Func<int, int, bool> validPos, bool refectable, Action<Vec2> onReflection = null)
        {
            var move = u.MoveForwardStateRunner(getDir, validPos, refectable, onReflection);
            return u.SimpleState((_, te) => move(te));
        }

        public static Action<Fix64> MoveForwardStateRunner(this IUnit u, Func<Vec2> getDir, Func<int, int, bool> validPos, bool refectable = false, Action<Vec2> onReflection = null)
        {
            return (te) =>
            {
                var dir = getDir();
                var newPos = u.Pos + dir * te;
                var x = (int)newPos.x;
                var y = (int)newPos.y;

                if (validPos(x, y))
                    u.Pos = newPos;
                else if (refectable)
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

                    onReflection?.Invoke(dir);
                }
            };
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

        public static StateMachine MoveOnPtsList(this IUnit u, Func<List<Vec2>> ptsProvider, Func<Fix64> speed, Action onEnd)
        {
            var sm = new StateMachine(u.UID);

            var dist = Fix64.Zero;
            var ndx = ptsProvider().IndexOf(u.Pos);

            sm.NewState("moving").Run((st, te) =>
            {
                var pst = ptsProvider();
                dist += speed() * te;
                while (dist > 1 && ndx < pst.Count)
                {
                    dist -= 1;
                    ndx++;
                }

                u.Pos = ndx >= pst.Count ? pst[^1] : pst[ndx];
            }).AsDefault();

            sm.NewState("ended").OnRunIn((st) =>
            {
                onEnd?.Invoke();
            });

            sm.Trans().From("moving").To("ended").When((st) => ndx >= ptsProvider().Count);

            return sm;
        }
    }
}
