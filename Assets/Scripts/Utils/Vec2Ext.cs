using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Swift.Math;

public static class Ext 
{
    public static Vector3 ToVec3(this Vec2 vec)
    {
        return new Vector3((float)vec.x, (float)vec.y, 0);
    }
}
