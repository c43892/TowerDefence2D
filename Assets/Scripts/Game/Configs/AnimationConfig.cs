using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefance
{
    public class AnimationConfig
    {
        [JsonProperty("label")]
        public string label;

        [JsonProperty("interval")]
        public float interval;

        [JsonProperty("loop")]
        public bool loop;
    }
}
