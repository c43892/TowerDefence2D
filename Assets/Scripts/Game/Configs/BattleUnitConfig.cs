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
        public static float GetFloat(this Dictionary<string, object> args, string key) => args.ContainsKey(key) ? Convert.ToSingle(args[key]) : 0f;
        public static float GetInt(this Dictionary<string, object> args, string key) => args.ContainsKey(key) ? Convert.ToInt32(args[key]) : 0;
        public static bool GetBool(this Dictionary<string, object> args, string key) => args.ContainsKey(key) ? Convert.ToBoolean(args[key]) : false;
        public static string GetString(this Dictionary<string, object> args, string key) => Convert.ToString(args[key]);
        public static Vec2 GetV2(this Dictionary<string, object> args, string keyX, string keyY) => new(args.GetFloat(keyX), args.GetFloat(keyY));
    }
}
