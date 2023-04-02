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
            IUnit[] FindTargets();
            void Attack(IUnit[] targets);
            Fix64 Cooldown { get; }
        }

        public static StateMachine AIAttackInPlace(this IAttackerUnit attacker)
        {
            var sm = new StateMachine(attacker.UID);

            IUnit[] targets = null;

            // search for targets
            sm.NewState("searching").OnRunIn((st) =>
            {
                targets = null;
            }).Run((st, te) =>
            {
                targets = attacker.FindTargets();
            }).AsDefault();

            // attack
            var cooldown = Fix64.Zero;
            sm.NewState("attacking").Run((st, te) =>
            {
                attacker.Attack(targets);
                cooldown = attacker.Cooldown;
            });

            // cooldown
            sm.NewState("cooldown").Run((st, te) =>
            {
                cooldown -= te;
            });

            sm.Trans().From("searching").To("attacking").When((st) => targets != null && targets.Length != 0);
            sm.Trans().From("attacking").To("cooldown").When((st) => cooldown > 0);
            sm.Trans().From("cooldown").To("searching").When((st) => cooldown <= 0);

            return sm;
        }
    }
}