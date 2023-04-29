using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefance
{
    public class TowerConfig
    {
        [JsonProperty("type")]
        public string type;

        [JsonProperty("phyPower")]
        public int phyPower;

        [JsonProperty("magPower")]
        public int magPower;

        [JsonProperty("attackingInterval")]
        public float attackingInterval;
    }
}
