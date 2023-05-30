using Newtonsoft.Json;
using Swift.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalPanic
{
    public class BattleUnitConfig
    {
        [JsonProperty("type")]
        public string type;

        [JsonProperty("ai")]
        public Dictionary<string, Dictionary<string, object>> ai;
    }

    public static class BattleUnitConfigExt
    {        
        public static float GetFloat(this Dictionary<string, object> args, string aiKey) => Convert.ToSingle(args[aiKey]);
        public static float GetInt(this Dictionary<string, object> args, string aiKey) => Convert.ToInt32(args[aiKey]);
        public static string GetString(this Dictionary<string, object> args, string aiKey) => Convert.ToString(args[aiKey]);
        public static Vec2 GetV2(this Dictionary<string, object> args, string keyX, string keyY) => new Vec2(args.GetFloat(keyX), args.GetFloat(keyY));
    }
}
