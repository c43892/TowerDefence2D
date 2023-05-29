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
        public static Action<string, BattleUnit, object> OnAbortAddUnitAI = null;

        public static BattleUnit AddUnitAt(this Battle battle, string unitName, Vec2 pos, bool isKeyUnit = false)
        {
            var cfg = ConfigManager.GetBattleUnitConfig(unitName);
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
            Fix64 r = float.Parse(args);

            OnAbortAddUnitAI?.Invoke("KillUnsafeCursorOnCollision", u, r);

            return u.SimpleState((st, te) =>
            {
                if (!u.Battle.IsCursorSafe && CheckCollision(u.Pos, u.Battle.Cursor.Pos, r * u.Scale))
                    u.Battle.CursorHurt();
            });
        }

        public static StateMachine AIReleaseUnitWhenCollisionOnTraceLine(this BattleUnit u, string args)
        {
            var ps = args.Split(", ".ToArray(), StringSplitOptions.RemoveEmptyEntries).ToArray();
            Fix64 r = float.Parse(ps[0]);
            var unitName = ps[1];
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
                    if (CheckCollision(u.Pos, p, r * u.Scale))
                    {
                        collisionPos = p;
                        return true;
                    }
                    else
                        return false;
                });

                if (collided)
                {
                    u.Battle.AddUnitAt(unitName, collisionPos);
                    cooldownTimer = cooldown;
                }
            });

            sm.NewState("cooldown").Run((st, te) => cooldownTimer -= te);

            sm.Trans().From("cooldown").To("checking").When((st) => cooldownTimer <= 0);
            sm.Trans().From("checking").To("cooldown").When((st) => cooldownTimer > 0);

            OnAbortAddUnitAI?.Invoke("ReleaseUnitWhenCollisionOnTraceLine", u, r);

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

        public static StateMachine AIMoveAndTurnAndSkill(this BattleUnit u, 
            Vec2 movingDir, Fix64 movingTime, 
            Fix64 turningSpeed, Fix64 turningAngleMin, Fix64 turningAngleMax,
            Fix64 skillTime, 
            Action<string> inSkill,
            Action<Fix64> runSkill)
        {
            var sm = new StateMachine(u.UID);

            var movingTimer = (Fix64)movingTime;
            var move = u.MoveForwardStateRunner(() => movingDir, (x, y) => !u.Map.IsBlocked(x, y), true, newDir =>
            {
                movingDir = newDir;
                u.Dir = movingDir.ToAngle();
            });

            var dAngle = Fix64.Zero;
            var turningDir = 0;

            var skillTimer = Fix64.Zero;

            u.Dir = movingDir.ToAngle();
            sm.NewState("moving")
            .Run((_, te) =>
            {
                move(te);
                movingTimer -= te;
            })
            .OnRunOut(_ => skillTimer = skillTime)
            .AsDefault();

            sm.NewState("skill")
            .OnRunIn(inSkill)
            .Run((_, te) =>
            {
                runSkill(te);
                skillTimer -= te;
            })
            .OnRunOut(_ =>
            {
                dAngle = RandomUtils.RandomNext(turningAngleMin, turningAngleMax);
                turningDir = RandomUtils.RandomNOrP();
            });

            void turn(Fix64 te)
            {
                var da = turningSpeed * te;
                if (da > dAngle)
                    da = dAngle;

                dAngle -= da;

                var toAngle = movingDir.ToAngle() + da * turningDir;
                movingDir = toAngle.ToV2Dir() * movingDir.Length;
                u.Dir = toAngle;
            }
            sm.NewState("turning")
            .Run((_, te) => turn(te))
            .OnRunOut(_ => movingTimer = movingTime);

            sm.Trans().From("moving").To("skill").When(_ => movingTimer < 0);
            sm.Trans().From("skill").To("turning").When(_ => skillTimer < 0);
            sm.Trans().From("turning").To("moving").When(_ => dAngle <= 0);

            return sm;
        }

        public static StateMachine AIMoveAndTurnAndRush(this BattleUnit u, string args)
        {
            var vs = args.Split(", ".ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(v => float.Parse(v)).ToArray();
            var dir = new Vec2(vs[0], vs[1]);
            var movingTime = vs[2];

            var turningAngleMin = vs[3];
            var turningAngleMax = vs[4];
            var turningSpeed = vs[5];

            var rushSpeedScale = vs[6];
            var rushingTime = vs[7];
            var rush = u.MoveForwardStateRunner(() => u.Dir.ToV2Dir() * rushSpeedScale, (x, y) => !u.Map.IsBlocked(x, y));

            return u.AIMoveAndTurnAndSkill(dir, movingTime, turningSpeed, turningAngleMin, turningAngleMax, rushingTime, null, rush);
        }

        public static StateMachine AIMoveAndTurnAndScale(this BattleUnit u, string args)
        {
            var vs = args.Split(", ".ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(v => float.Parse(v)).ToArray();
            var dir = new Vec2(vs[0], vs[1]);
            var movingTime = vs[2];

            var turningAngleMin = vs[3];
            var turningAngleMax = vs[4];
            var turningSpeed = vs[5];

            var scaleMax = vs[6];
            var scaleTime = vs[7];
            var halfScaleTime = scaleTime / 2;

            var scaleTimer = Fix64.Zero;
            void resetScaleTimer() { scaleTimer = 0; }

            void scaleRnner(Fix64 te)
            {
                scaleTimer += te;
                u.Scale = (scaleMax - 1) * (1 - MathEx.Abs(scaleTimer - halfScaleTime) / halfScaleTime) + 1;
            };

            return u.AIMoveAndTurnAndSkill(dir, movingTime, turningSpeed, turningAngleMin, turningAngleMax, scaleTime, (_) => resetScaleTimer(), scaleRnner);
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
