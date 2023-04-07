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
        public interface IAttackerUnit : IUnit
        {
            void Attack();
            bool CanAttack();
            Fix64 AttackingInterval { get; }
        }

        public static StateMachine AIAttackInPlace(this IAttackerUnit attacker)
        {
            var sm = new StateMachine(attacker.UID);

            // attack
            var cooldown = Fix64.Zero;
            sm.NewState("attacking").Run((st, te) =>
            {
                attacker.Attack();
                cooldown = attacker.AttackingInterval;
            }).AsDefault();

            // cooldown
            sm.NewState("cooldown").Run((st, te) =>
            {
                cooldown -= te;
            });

            sm.Trans().From("attacking").To("cooldown").When((st) => cooldown > 0);
            sm.Trans().From("cooldown").To("attacking").When((st) => cooldown <= 0);

            return sm;
        }
    }
}