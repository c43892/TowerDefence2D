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
        public static StateMachine MoveAndSelfExplode(this IAttackerUnit attacker, IEnumerable<Vec2> path, Func<Fix64> speedProvider)
        {
            var sm = new StateMachine(attacker.UID);

            var canAttack = false;

            // 沿给定路径移动
            Func<Fix64, bool> move = Move(attacker, path, speedProvider);
            sm.NewState("moving").Run((st, te) =>
            {
                move(te);
                canAttack = attacker.CanAttack();
            }).AsDefault();

            // 攻击目标
            sm.NewState("attacking").Run((st, te) =>
            {
                attacker.Attack();
                attacker.Hp = 0; // explode
            });

            sm.Trans().From("moving").To("attacking").When((st) => canAttack);

            return sm;
        }
    }
}