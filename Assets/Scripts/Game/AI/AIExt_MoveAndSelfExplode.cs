using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using Swift.Math;

namespace TowerDefance
{
    public static partial class AIUnitExt
    {
        public static StateMachine MoveAndSelfExplode(this IAttackerUnit attacker, List<Vec2> path, Fix64 maxSpeed)
        {
            var sm = new StateMachine(attacker.UID);

            IUnit[] targets = null;

            // 沿给定路径移动
            Func<Fix64, bool> move = Move(attacker, path, maxSpeed);
            sm.NewState("moving").Run((st, te) =>
            {
                move(te);
                targets = attacker.FindTargets();
            }).AsDefault();

            // 攻击目标
            sm.NewState("attacking").Run((st, te) =>
            {
                attacker.Attack(targets);
                attacker.Hp = 0; // explode
            });

            sm.Trans().From("moving").To("attacking").When((st) => targets != null && targets.Length > 0);

            return sm;
        }
    }
}