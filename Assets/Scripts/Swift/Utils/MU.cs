using System;
using System.Collections.Generic;
using Swift.Math;

namespace Swift
{
    /// <summary>
    /// Math related Utils
    /// </summary>
    public static class MU
    {
        // 截断 (min, max)
        public static int ClampLR(this int v, int min, int max)
        {
            return v.Clamp(min + 1, max - 1);
        }

        // 截断 (min, max]
        public static int ClampR(this int v, int min, int max)
        {
            return v.Clamp(min + 1, max);
        }

        // 截断 [min, max)
        public static int ClampL(this int v, int min, int max)
        {
            return v.Clamp(min, max - 1);
        }

        // 截断 [min, max]
        public static int Clamp(this int v, int min, int max)
        {
            if (v < min)
                return min;
            else if (v > max)
                return max - 1;
            else
                return v;
        }

        // 根据给定向量方向计算对应角度
        public static Fix64 v2Degree(Fix64 y, Fix64 x)
        {
            var arc = Fix64.Atan2(y, x);
            return arc * 180 / Fix64.Pi;
        }

        // 判断给定扇形是否与指定圆形相交
        public static bool IsFunOverlappedCircle(Vec2 circleCenter, Fix64 circleR, Vec2 fanCenter, Fix64 fanR, Fix64 fanDir, Fix64 fanAngle)
        {
            // 先判断圆心距离
            var dc = circleCenter - fanCenter;
            var r = dc.Length;
            if (r > circleR + fanR)
                return false;

            // 在判断方向角度
            var dir = MU.v2Degree(dc.x, dc.y);
            var dd = (dir - fanDir).RangeIn180();
            return Fix64.Abs(dd) <= fanAngle / 2;
        }

        // 对给定的 Vec2 在指定范围内做镜像
        public static void MirroClamp(this Vec2 v, Vec2 min, Vec2 max)
        {
            if (v.x < min.x)
                v.x = 2 * min.x - v.x;
            else if (v.x > max.x)
                v.x = 2 * max.x - v.x;

            if (v.y < min.y)
                v.y = 2 * min.y - v.y;
            else if (v.y > max.y)
                v.y = 2 * max.y - v.y;
        }

        // 对给定的 Vec2 在指定范围内做镜像
        public static void MirroClamp(this Vec2 v, Vec2 max)
        {
            v.MirroClamp(Vec2.Zero, max);
        }

        // 判断给定左边是否在指定矩形范围内
        public static bool InRect(this Vec2 v, Vec2 min, Vec2 max)
        {
            return v.x >= min.x && v.x <= max.x && v.y >= min.y && v.y <= max.y;
        }

        // 判断给定左边是否在指定矩形范围内
        public static bool InRect(this Vec2 v, Vec2 max)
        {
            return v.InRect(Vec2.Zero, max);
        }

        // 求 3x3 矩阵行列式
        public static Fix64 Determinant(Fix64[, ] matrix)
        {
            return matrix[0, 0] * (matrix[1, 1] * matrix[2, 2] - matrix[2, 1] * matrix[1, 2])
                 - matrix[0, 1] * (matrix[1, 0] * matrix[2, 2] - matrix[1, 2] * matrix[2, 0])
                 + matrix[0, 2] * (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0]);
        }

        // 求解经过指定 p1, p2, p3 的一元二次方程参数 Ax^2+Bx+c=0;
        public static bool SolveQuadraticEquation(Vec2 p1, Vec2 p2, Vec2 p3, out Fix64 a, out Fix64 b, out Fix64 c)
        {
            var x1 = p1.x;
            var y1 = p1.y;
            var x2 = p2.x;
            var y2 = p2.y;
            var x3 = p3.x;
            var y3 = p3.y;

            Fix64[, ] A = new[,]
            {
                { x1 * x1, x1, 1 },
                { x2 * x2, x2, 1 },
                { x3 * x3, x3, 1 }
            };

            Fix64[] B = new[] { y1, y2, y3 };

            var detA = Determinant(A);

            if (detA == Fix64.Zero)
            {
                Console.WriteLine("Matrix A is singular and cannot be solved.");
                a = b = c = 0;
                return false;
            }

            var Ax = (Fix64[,])A.Clone();
            Ax[0, 0] = B[0];
            Ax[1, 0] = B[1];
            Ax[2, 0] = B[2];

            var Ay = (Fix64[,])A.Clone();
            Ay[0, 1] = B[0];
            Ay[1, 1] = B[1];
            Ay[2, 1] = B[2];

            var Az = (Fix64[,])A.Clone();
            Az[0, 2] = B[0];
            Az[1, 2] = B[1];
            Az[2, 2] = B[2];

            a = Determinant(Ax) / detA;
            b = Determinant(Ay) / detA;
            c = Determinant(Az) / detA;

            return true;
        }
    }
}