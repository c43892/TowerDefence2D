using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using System;
using System.Linq;
using static UnityEditor.PlayerSettings;

namespace GalPanic
{
    public static class BattleExt
    {
        public static BattleUnit AddUnitAt(this Battle battle, string unitType, Vec2 pos)
        {
            var cfg = ConfigManager.GetBattleUnitConfig(unitType);
            var unit = new BattleUnit(
                battle.Map,
                cfg.type,
                cfg.ai
            )
            {
                Pos = pos
            };

            battle.Map.AddUnit(unit);
            return unit;
        }

        public static StateMachine AIMoveAndReflect(this BattleUnit u, string args)
        {
            var vs = args.Split(',').Select(v => float.Parse(v)).ToArray();
            return u.MoveAndReflect(new Vec2(vs[0], vs[1]), (x, y) => !u.Map.IsBlocked(x, y));
        }

        public static StateMachine AIKillUnsafeCursorOnCollision(this BattleUnit u, string args)
        {
            var r = float.Parse(args);
            return u.SimpleState((st, te) =>
            {
                if (!u.Battle.IsCursorSafe && CheckCollision(u.Pos, u.Battle.Cursor.Pos, r))
                    u.Battle.CursorHurt();
            });
        }

        public static bool CheckCollision(Vec2 pos, Vec2 targetPos, Fix64 radius)
        {
            var l = (int)(pos.x - radius);
            var r = (int)(pos.x + radius);
            var t = (int)(pos.y - radius);
            var b = (int)(pos.y + radius);

            return l <= targetPos.x && targetPos.x <= r && t <= targetPos.y && targetPos.y <= b;
        }
    }
}
