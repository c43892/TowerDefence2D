using Swift;
using Swift.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefance.Game;
using UnityEngine;
using TowerDefance;

public partial class TestBattleScene
{
    public Transform EffectRoot;

    public GameObject BulletModel;

    readonly List<Func<float, bool>> effets = new();

    void InitBattleEventsHandler()
    {
        SkillAttackingSingleTargetPhysical.OnAttacking += (skill, attacker, target, dhp) =>
        {
            var attackerObj = GetUnitObj((attacker as BattleMap.IUnit).UID);
            var targetObj = GetUnitObj((target as BattleMap.IUnit).UID);
            var bulletObj = Instantiate(BulletModel);

            // initial position
            bulletObj.SetActive(true);
            bulletObj.transform.SetParent(EffectRoot);
            bulletObj.transform.position = attackerObj.transform.position;

            // flying process
            var flyingSpeed = 10f;
            effets.Add((te) =>
            {
                var dir = targetObj.transform.position - bulletObj.transform.position;
                var dist = dir.magnitude;
                var distMoved = flyingSpeed * te;

                if (distMoved >= dist)
                {
                    // done
                    Destroy(bulletObj);
                    return false;
                }

                var dPos = dir * distMoved / dist;
                bulletObj.transform.position += dPos;

                return true;
            });
        };
    }

    void UpdateBattleEffect(float te)
    {
        List<Func<float, bool>> toRemove = new();

        foreach (var e in effets)
            if (!e(te))
                toRemove.Add(e);

        foreach (var e in toRemove)
            effets.Remove(e);
    }
}
