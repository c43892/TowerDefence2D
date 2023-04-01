using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Swift.Math;

namespace TowerDefance
{
    public static partial class AIUnitExt
    {
        public interface IMovingUnit : IUnit
        {
            Fix64 ForwardDir { get; set; }
            Fix64 MaxSpeed { get; }
        }

        public static StateMachine AIMoving(this IMovingUnit u, List<Vec2> path)
        {
            var sm = new StateMachine(u.UID);

            // 沿给定路径移动
            Func<Fix64, bool> move = Move(u, path, u.MaxSpeed);
            sm.NewState("moving").Run((st, te) => move(te)).AsDefault();

            return sm;
        }

        // 沿路径移动
        static Func<Fix64, bool> Move(IMovingUnit u, List<Vec2> path, Fix64 v)
        {
            Vec2 nowPos = u.Pos;
            return (dt) =>
            {
                // 计算当前位置
                var maxDist = v * dt;
                var ps = Vec2.Zero;
                var pe = Vec2.Zero;
                nowPos = OnPath(nowPos, path, maxDist, out ps, out pe);
                var dir = pe - ps;
                var dist = dir.Length;
                u.Dir = dist > 0 ? dir.Dir() : u.ForwardDir;

                // 更新位置
                u.Pos = nowPos;

                // 是否还有剩余距离可继续移动
                return path.Count > 0;
            };
        }

        // 沿路径移动，返回下一个目标路径点
        static Vec2 OnPath(Vec2 from, List<Vec2> path, Fix64 dist, out Vec2 ps, out Vec2 pe)
        {
            var dst = path.Count > 0 ? path[path.Count - 1] : from;
            ps = from;
            pe = from;

            while (dist > 0 && path.Count > 0)
            {
                pe = path[0];
                var d = (pe - ps).Length;
                if (dist >= d)
                {
                    dist -= d;
                    ps = pe;
                    path.RemoveAt(0);
                }
                else
                    return ps + (pe - ps) * dist / d;
            }

            return dst;
        }
    }
}