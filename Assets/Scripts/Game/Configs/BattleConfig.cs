using Newtonsoft.Json;
using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalPanic
{
    public class BattleConfig
    {
        public class BattleConfigUnit
        {
            [JsonProperty("type")]
            public string type;

            [JsonProperty("x")]
            public int x;

            [JsonProperty("y")]
            public int y;
        }

        [JsonProperty("width")]
        public int width;

        [JsonProperty("height")]
        public int height;

        [JsonProperty("winPercent")]
        public float winPercent;

        [JsonProperty("units")]
        public BattleConfigUnit[] units;
    }
}
