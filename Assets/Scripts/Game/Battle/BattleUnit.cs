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

        public Vec2 Pos { get; set; }

        private readonly StateMachine[] sms = null;

        public BattleUnit(BattleMap map, string type, Dictionary<string, string> aiTypes)
        {
            Type = type;
            Map = map;
            UID = IDGen(IDGen($"unit_{type}_"));

            sms = aiTypes.Select(ai => CreateAI(ai.Key, ai.Value)).ToArray();
            sms.Travel(sm => sm.Start());
        }

        StateMachine CreateAI(string aiType, string args)
        {
            return aiType switch
            {
                "MoveAndReflect" => this.AIMoveAndReflect(args),
                "KillUnsafeCursorOnCollision" => this.AIKillUnsafeCursorOnCollision(args),
                _ => null
            }; ;
        }

        public void OnTimeElapsed(Fix64 te)
        {
            sms.Travel(sm => sm.Run(te));
        }
    }
}
