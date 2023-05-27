using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using System;
using System.Linq;
using static UnityEditor.PlayerSettings;
using UnityEngine.UI;

namespace GalPanic
{
    public static class BattleExt
    {
        public static BattleUnit AddUnitAt(this Battle battle, string unitType, Vec2 pos, bool isKeyUnit = false)
        {
            var cfg = ConfigManager.GetBattleUnitConfig(unitType);
            var unit = new BattleUnit(
                battle.Map,
                cfg.type
            )
            {
                Pos = pos,
                IsKeyUnit = isKeyUnit
            };

            unit.BuildAI(cfg.ai);
            battle.Map.AddUnit(unit);
            return unit;
        }

        public static StateMachine AIMoveAndReflect(this BattleUnit u, string args)
        {
            var vs = args.Split(", ".ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(v => float.Parse(v)).ToArray();
            var dir = new Vec2(vs[0], vs[1]);
            return u.MoveForward(() => dir, (x, y) => !u.Map.IsBlocked(x, y), true, newDir => dir = newDir);
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

        public static StateMachine AIReleaseUnitWhenCollisionOnTraceLine(this BattleUnit u, string args)
        {
            var ps = args.Split(", ".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToArray();
            Fix64 radius = float.Parse(ps[0]);
            var unitType = ps[1];
            Fix64 cooldown = float.Parse(ps[2]);

            var sm = new StateMachine(u.UID);

            Fix64 cooldownTimer = 0;
            sm.NewState("checking").AsDefault().Run((st, te) =>
            {
                var traceLine = u.Battle.Cursor.TraceLine;
                if (traceLine.Count == 0)
                    return;

                var collisionPos = traceLine[0];
                var collided = traceLine.Any((p) =>
                {
                    var r = CheckCollision(u.Pos, p, radius);
                    if (r)
                        collisionPos = p;

                    return r;
                });

                if (collided)
                {
                    u.Battle.AddUnitAt(unitType, collisionPos);
                    cooldownTimer = cooldown;
                }
            });

            sm.NewState("cooldown").Run((st, te) => cooldownTimer -= te);

            sm.Trans().From("cooldown").To("checking").When((st) => cooldownTimer <= 0);
            sm.Trans().From("checking").To("cooldown").When((st) => cooldownTimer > 0);

            return sm;
        }

        public static StateMachine AIMoveOnPtsList(this BattleUnit u, string args)
        {
            var speed = float.Parse(args);
            return u.MoveOnPtsList(() => u.Battle.Cursor.TraceLine, () => speed, () =>
            {
                if (!u.Battle.IsCursorSafe)
                    u.Battle.CursorHurt();

                u.Battle.KillUnit(u);
            });
        }

        public static StateMachine AIMoveAndRush(this BattleUnit u, string args)
        {
            var vs = args.Split(", ".ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(v => float.Parse(v)).ToArray();
            var dir = new Vec2(vs[0], vs[1]);
            var movingTime = vs[2];

            var rushSpeedScale = vs[3];
            var rushingTime = vs[4];
            Vec2 getRushSpeed() => dir * rushSpeedScale;

            var sm = new StateMachine(u.UID);

            var movingTimer = (Fix64)movingTime;
            var move = u.MoveForwardStateRunner(() => dir, (x, y) => !u.Map.IsBlocked(x, y), true, newDir => dir = newDir);
            sm.NewState("moving")
            .OnRunIn((st) => movingTimer = (Fix64)movingTime)
            .Run((_, te) =>
            {
                move(te);
                movingTimer -= te;
            }).AsDefault();

            var rushingTimer = Fix64.Zero;
            var rush = u.MoveForwardStateRunner(getRushSpeed, (x, y) => !u.Map.IsBlocked(x, y), false);
            sm.NewState("rushing")
            .OnRunIn((st) => rushingTimer = (Fix64)rushingTime)
            .Run((_, te) =>
            {
                rush(te);
                rushingTimer -= te;
            });

            sm.Trans().From("moving").To("rushing").When(_ => movingTimer <= 0);
            sm.Trans().From("rushing").To("moving").When(_ => rushingTimer <= 0);

            return sm;
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
