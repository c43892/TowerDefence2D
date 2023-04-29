using System.Collections;
using System.Collections.Generic;
using Swift;
using System.Linq;
using UnityEngine;
using System.IO;

[System.Serializable]
public class MultiAniData
{
    public Dictionary<string, AniData> Data = new();
}
