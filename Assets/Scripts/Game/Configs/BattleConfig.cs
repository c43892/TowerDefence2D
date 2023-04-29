using Newtonsoft.Json;
using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefance
{
    public class BattleConfig
    {
        public class Pos
        {
            [JsonProperty("x")]
            public int x;

            [JsonProperty("y")]
            public int y;

            public Vec2 ToVec2() => new(x, y);
        }

        public class BaseInfo
        {
            [JsonProperty("pos")]
            public Pos pos;

            [JsonProperty("phyDefence")]
            public int phyDefence;

            [JsonProperty("magDefence")]
            public int magDefence;

            [JsonProperty("maxHp")]
            public int maxHp;
        }

        [JsonProperty("size")]
        public Pos size;

        [JsonProperty("path")]
        public Pos[] path;

        [JsonProperty("towers")]
        public string[] towers;

        [JsonProperty("baseInfo")]
        public BaseInfo baseInfo;

        [JsonProperty("spawningPos")]
        public Pos spawningPos;

        [JsonProperty("spawningInterval")]
        public float spawningInterval;

        [JsonProperty("enemies")]
        public string[] enemies;
    }
}
