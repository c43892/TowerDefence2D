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
        BattleMap.OnMapUnitRemoved += (map, u) => RemoveUnitObj(u.UID);
        BattleMap.OnMapUnitAdded += (map, u) => AddUnitObj(u);

        SkillAttackingTargets.AboutToAttacking += (skill, attacker, targets) =>
        {
            var attackerObj = GetUnitObj((attacker as BattleMapUnit).UID);

            // flying process
            FC.ForEach(targets, (i, t) =>
            {
                var bulletObj = Instantiate(BulletModel);

                // initial position
                bulletObj.SetActive(true);
                bulletObj.transform.SetParent(EffectRoot);
                bulletObj.transform.position = attackerObj.transform.position;

                var targetUID = (t as BattleMapUnit).UID;
                var targetObj = GetUnitObj(targetUID);
                var targetPos = targetObj.transform.position;

                var flyingTimeLeft = SkillAttacking.ATTACKING_DEPLAY;
                effets.Add((te) =>
                {
                    targetObj = GetUnitObj(targetUID);
                    if (targetObj != null)
                        targetPos = targetObj.transform.position;

                    var dir = targetPos - bulletObj.transform.position;
                    var dist = dir.magnitude;
                    var flyingSpeed = (bulletObj.transform.position - targetPos).magnitude / flyingTimeLeft;
                    var distMoved = flyingSpeed * te;
                    flyingTimeLeft -= te;

                    if (distMoved >= dist || flyingTimeLeft <= 0)
                    {
                        // done
                        Destroy(bulletObj);
                        return false;
                    }

                    var dPos = dir * ((float)distMoved) / dist;
                    bulletObj.transform.position += dPos;

                    return true;
                });

                if (attacker is Tower)
                {
                    var mainTarget = targets[0];
                    var mainTargetObj = GetUnitObj((mainTarget as BattleMapUnit).UID);
                    var flipX = (mainTargetObj.transform.position - attackerObj.transform.position).x > 0;
                    attackerObj.transform.localRotation = flipX ? Quaternion.AngleAxis(180, Vector3.up) : Quaternion.AngleAxis(0, Vector3.up);
                    attackerObj.GetComponent<FrameAni>().StartPlay();
                }
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
