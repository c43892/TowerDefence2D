using Swift;
using System;
using System.Collections.Generic;
using TowerDefance;
using TowerDefance.Game;
using UnityEngine;

public partial class TestBattleScene
{
    public Transform EffectRoot;

    public GameObject BulletModel;

    readonly List<Func<float, bool>> effets = new();

    void InitBattleEventsHandler()
    {
        BattleMap.OnUnitRemoved += (u) => RemoveUnitObj(u.UID);

        SkillAttackingTargets.AboutToAttacking += (skill, attacker, targets) =>
        {
            var attackerObj = GetUnitObj((attacker as BattleMap.IUnit).UID);

            // flying process
            FC.ForEach(targets, (i, t) =>
            {
                var bulletObj = Instantiate(BulletModel);

                // initial position
                bulletObj.SetActive(true);
                bulletObj.transform.SetParent(EffectRoot);
                bulletObj.transform.position = attackerObj.transform.position;

                var targetUID = (t as BattleMap.IUnit).UID;
                var targetObj = GetUnitObj(targetUID);
                var targetPos = targetObj.transform.position;

                var flyingSpeed = (attackerObj.transform.position - targetPos).magnitude / (SkillAttacking.ATTACKING_DEPLAY / 1000f);

                effets.Add((te) =>
                {
                    targetObj = GetUnitObj(targetUID);
                    if (targetObj != null)
                        targetPos = targetObj.transform.position;

                    var dir = targetPos - bulletObj.transform.position;
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
            });
        };

        SkillAttackingTargets.OnAttackingDone += (skill, attacker, attackingResults) => { };
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
