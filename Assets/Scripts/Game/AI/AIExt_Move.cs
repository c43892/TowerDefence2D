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
        public static StateMachine AIMove(this IUnit u, IEnumerable<Vec2> path, Fix64 maxSpeed)
        {
            var sm = new StateMachine(u.UID);

            // 沿给定路径移动
            Func<Fix64, bool> move = Move(u, path, maxSpeed);
            sm.NewState("moving").Run((st, te) => move(te)).AsDefault();

            return sm;
        }

        // 沿路径移动
        static Func<Fix64, bool> Move(IUnit u, IEnumerable<Vec2> srcPath, Fix64 v)
        {
            var path = new List<Vec2>(srcPath);
            Vec2 nowPos = u.Pos;
            return (dt) =>
            {
                // 计算当前位置
                var maxDist = v * dt;
                var ps = Vec2.Zero;
                var pe = Vec2.Zero;
                nowPos = OnPath(nowPos, path, maxDist, out ps, out pe);

                // 更新位置和方向
                u.Pos = nowPos;
                var dir = pe - ps;
                var dist = dir.Length;
                if (dist > 0)
                    u.Dir = dir.Dir();

                // 是否还有剩余距离可继续移动
                return path.Count > 0;
            };
        }

        // 沿路径移动，返回下一个目标路径点
        static Vec2 OnPath(Vec2 from, List<Vec2> path, Fix64 dist, out Vec2 ps, out Vec2 pe)
        {
            var dst = path.Count > 0 ? path[^1] : from;
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