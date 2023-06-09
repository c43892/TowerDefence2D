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
            [JsonProperty("name")]
            public string name;

            [JsonProperty("x")]
            public int x;

            [JsonProperty("y")]
            public int y;

            [JsonProperty("isKeyUnit")]
            public bool isKeyUnit = false;
        }

        public class CursorConfig
        {
            [JsonProperty("hp")]
            public int hp;

            [JsonProperty("speed")]
            public float speed;

            [JsonProperty("backSpeed")]
            public float backSpeed;

            [JsonProperty("armor")]
            public int armor;

            [JsonProperty("armorDec")]
            public float armorDec;

            [JsonProperty("armorHurtReset")]
            public int armorHurtReset;

            [JsonProperty("armorCompletionBonus")]
            public float[] armorCompletionBonus;
        }

        [JsonProperty("width")]
        public int width;

        [JsonProperty("height")]
        public int height;

        [JsonProperty("frontAni")]
        public string frontAni;

        [JsonProperty("backAni")]
        public string backAni;

        [JsonProperty("winPercent")]
        public float winPercent;

        [JsonProperty("units")]
        public BattleConfigUnit[] units;

        [JsonProperty("cursor")]
        public CursorConfig cursor;
    }
}
