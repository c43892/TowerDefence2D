using Newtonsoft.Json;
using Swift.Math;
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
        public Dictionary<string, string> ai;
    }
}
