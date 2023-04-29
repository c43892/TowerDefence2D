using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefance
{
    public class EnemyConfig
    {
        [JsonProperty("type")]
        public string type;

        [JsonProperty("maxSpeed")]
        public float maxSpeed;

        [JsonProperty("maxHp")]
        public int maxHp;

        [JsonProperty("phyDefence")]
        public int phyDefence;

        [JsonProperty("magDefence")]
        public int magDefence;

        [JsonProperty("phyPower")]
        public int phyPower;

        [JsonProperty("magPower")]
        public int magPower;
    }
}
