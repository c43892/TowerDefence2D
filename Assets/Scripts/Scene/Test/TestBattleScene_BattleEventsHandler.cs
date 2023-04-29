using Swift;
using Swift.Math;
using System;
using System.Collections.Generic;
using TowerDefance;
using TowerDefance.Game;
using Unity.VisualScripting;
using UnityEngine;

public partial class TestBattleScene
{
    public Transform EffectRoot;

    public GameObject BulletModel;

    readonly List<Func<float, bool>> effets = new();

    // 计算抛物线轨迹
    bool CalculateParabola(Vector3 start, Vector3 mid, Vector3 end, float t, out Vector3 pos, out Fix64 tan)
    {
        Vec2 p1 = new(start.x, start.y);
        Vec2 p2 = new(mid.x, mid.y);
        Vec2 p3 = new(end.x, end.y);

        if (!MU.SolveQuadraticEquation(p1, p2, p3, out Fix64 a, out Fix64 b, out Fix64 c))
        {
            pos = end;
            tan = 0;
            return false;
        }

        var x = (end.x - start.x) * t + start.x;
        var y = a * x * x + b * x + c;
        var z = (end.x - start.x) * t + start.z;

        pos = new Vector3(x, (float)y, z);
        tan = 2 * a * x + b;
        return true;
    }

    void InitBattleEventsHandler()
    {
        BattleMap.OnMapUnitRemoved += (map, u) => RemoveUnitObj(u.UID);
        BattleMap.OnMapUnitAdded += (map, u) =>
        {
            var model = AddUnitObj(u);
            if (u is Enemy)
                model.Play("Moving");
            else
                model.Play("Idle");
        };

        SkillAttackingTargets.AboutToAttacking += (skill, attacker, targets) =>
        {
            var attackerObj = GetUnitObj((attacker as BattleMapUnit).UID);
            var attackerPos = attackerObj.transform.position;

            // flying process
            FC.ForEach(targets, (i, t) =>
            {
                var targetUID = (t as BattleMapUnit).UID;
                var targetObj = GetUnitObj(targetUID);
                var targetPos = targetObj.transform.position;

                var faceLeft = targetPos.x < attackerPos.x;
                var midPos = attackerPos;
                midPos.y = (attackerPos.y > targetPos.y ? attackerPos.y : targetPos.y) + 0.5f;
                var d = midPos.y - attackerPos.y;
                midPos.x = attackerPos.x + (faceLeft ? -d : d);

                var flyingTimeLeft = SkillAttacking.ATTACKING_DEPLAY;

                var bulletObj = Instantiate(BulletModel);
                bulletObj.SetActive(true);
                bulletObj.transform.SetParent(EffectRoot);
                bulletObj.transform.position = attackerPos;
                var aniCfg = ConfigManager.GetEffectAnimationConfig("ArcherAttacking");
                var ani = bulletObj.GetComponent<FrameAni>();
                ani.Data = new AniData() { label = aniCfg.label, interval = aniCfg.interval, loop = aniCfg.loop };
                ani.Play();

                effets.Add((te) =>
                {
                    targetObj = GetUnitObj(targetUID);
                    if (targetObj != null)
                        targetPos = targetObj.transform.position;

                    var flyingSpeed = (bulletObj.transform.position - targetPos).magnitude / flyingTimeLeft;

                    var t = (float)((SkillAttacking.ATTACKING_DEPLAY - flyingTimeLeft) / SkillAttacking.ATTACKING_DEPLAY);
                    CalculateParabola(attackerPos, midPos, targetPos, t, out Vector3 pos, out Fix64 tan);

                    bulletObj.transform.SetPositionAndRotation(pos, Quaternion.AngleAxis((float)MU.v2Degree(tan, 1), Vector3.forward));

                    flyingTimeLeft -= te;
                    if (flyingTimeLeft <= 0 || (bulletObj.transform.position - targetPos).magnitude < 0.1f)
                    {
                        Destroy(bulletObj);
                        return false;
                    }

                    return true;
                });

                if (attacker is Tower tower)
                {
                    var mainTarget = targets[0];
                    var mainTargetObj = GetUnitObj((mainTarget as BattleMapUnit).UID);
                    attackerObj.transform.localRotation = faceLeft ? Quaternion.AngleAxis(0, Vector3.up) : Quaternion.AngleAxis(180, Vector3.up);

                    var model = attackerObj.GetComponent<BattleMapUnitModel>();
                    model.Unit = tower;
                    model.Play("Attacking", () => model.Play("Idle"));
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
