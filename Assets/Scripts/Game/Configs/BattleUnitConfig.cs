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
        public string[] ai = null;

        [JsonProperty("r")]
        public float radius;

        [JsonProperty("vx")]
        public float speedX = 0;

        [JsonProperty("vy")]
        public float speedY = 0;
    }
}
