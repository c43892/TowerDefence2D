using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using System;
using static GalPanic.AIUnitExt;

namespace GalPanic
{
    public class BattleUnit : IUnit, ITimeDriven
    {
        public static Func<string, string> IDGen = null;

        public BattleMap Map { get; private set; }

        public string Type { get; private set; }

        public string UID { get; private set; }

        public int Hp { get; set; }

        public Vec2 Pos { get; set; }

        public Vec2 Dir { get; set; }

        public string CurrentAIStatus => sm?.CurrentState;

        private readonly StateMachine sm = null;

        public BattleUnit(BattleMap map, string type, string aiType, Vec2 pos, Vec2 dir)
        {
            Type = type;
            Map = map;
            UID = IDGen(IDGen($"unit_{type}_"));
            Pos = pos;
            Dir = dir;
            Hp = 1;

            sm = CreateAI(aiType);
            sm.Start();
        }

        StateMachine CreateAI(string aiType)
        {
            return aiType switch
            {
                "MoveAndReflect" => this.MoveAndReflect(Dir, (x, y) => !Map.IsBlocked(x, y)),
                "SpawnUnitIntervally" => this.RunInternally(1, () =>
                {
                    // var u = new BattleUnit(map, "Slime", )
                }),
                _ => null
            };
        }

        public void OnTimeElapsed(Fix64 te)
        {
            sm.Run(te);
        }
    }
}
