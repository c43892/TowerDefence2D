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
}
