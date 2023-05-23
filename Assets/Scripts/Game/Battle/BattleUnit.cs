using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using System;
using static GalPanic.AIUnitExt;
using System.Linq;
using static UnityEngine.EventSystems.EventTrigger;

namespace GalPanic
{
    public class BattleUnit : IUnit, ITimeDriven
    {
        public static Func<string, string> IDGen = null;

        public BattleMap Map { get; private set; }

        public Battle Battle => Map.Battle;

        public string Type { get; private set; }

        public string UID { get; private set; }

        public Fix64 Radius { get; private set; }

        public Vec2 Pos { get; set; }

        public Vec2 Dir { get; set; }

        private readonly StateMachine[] sms = null;

        public BattleUnit(BattleMap map, string type, string[] aiTypes, Fix64 radius, Vec2 pos, Vec2 dir)
        {
            Type = type;
            Map = map;
            UID = IDGen(IDGen($"unit_{type}_"));
            Radius = radius;
            Pos = pos;
            Dir = dir;

            sms = aiTypes.Select(ai => CreateAI(ai)).ToArray();
            sms.Travel(sm => sm.Start());
        }

        StateMachine CreateAI(string aiType)
        {
            return aiType switch
            {
                "MoveAndReflect" => this.MoveAndReflect(Dir, (x, y) => !Map.IsBlocked(x, y)),
                "KillUnsafeCursorOnCollision" => this.OnCollide(() => Map.Battle.CursorPos, () =>
                {
                    if (!Map.Battle.IsCursorSafe)
                        Map.Battle.CursorHurt();
                }),
                "SpawnUnitPeriodically" => this.RunPeriodically(1, () =>
                {
                    // var u = new BattleUnit(map, "Slime", )
                }),
                _ => null
            };
        }

        public void OnTimeElapsed(Fix64 te)
        {
            sms.Travel(sm => sm.Run(te));
        }
    }
}
