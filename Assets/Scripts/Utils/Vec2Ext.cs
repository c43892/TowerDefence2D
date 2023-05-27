using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Swift.Math;

public static class Ext 
{
    public static Vector3 ToV3(this Vec2 vec, float z = 0)
    {
        return new Vector3((float)vec.x, (float)vec.y, z);
    }

    public static float ToArc(this Vec2 vec)
    {
        return Mathf.Atan2((float)vec.y, (float)vec.x);
    }

    public static float ToAngle(this Vec2 vec)
    {
        return vec.ToArc() * Mathf.Rad2Deg;
    }

    public static Vec2 ToV2Dir(this Fix64 angle)
    {
        return new Vec2(MathEx.Cos(angle * Mathf.Deg2Rad), MathEx.Sin(angle * Mathf.Deg2Rad));
    }
}
