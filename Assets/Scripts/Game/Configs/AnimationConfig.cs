using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalPanic
{
    public class AnimationConfig
    {
        [JsonProperty("scale")]
        public float scale = 1;

        [JsonProperty("label")]
        public string label;

        [JsonProperty("interval")]
        public float interval;

        [JsonProperty("loop")]
        public bool loop = false;
    }
}
