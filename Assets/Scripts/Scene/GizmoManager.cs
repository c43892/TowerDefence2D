using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalPanic;
using Swift;
using Swift.Math;
using System.Linq;
using GalPanic.Res;
using UnityEngine.UI;
using Unity.Collections;
using System;

public class GizmoManager : MonoBehaviour
{
    public BattleSceneRender SceneRenderer;

    private readonly List<KeyValuePair<string, Func<Vector4>>> gizmoSphere = new();

    public void OnAbortAddUnitAI(string evt, BattleUnit u, object args)
    {
        switch (evt)
        {
            case "ReleaseUnitWhenCollisionOnTraceLine":
            case "KillUnsafeCursorOnCollision":
                var r = (Fix64)args / 10;
                AddGizmo(u.UID, () =>
                {
                    var pos = SceneRenderer.GetUnitRenderer(u.UID).transform.position;
                    return new Vector4(pos.x, pos.y, pos.z, (float)(r * u.Scale));
                });
                break;

            default:
                break;
        }
    }

    public void OnBattleUnitRemoved(BattleUnit u)
    {
        RemoveGizmo(u.UID);
    }

    public void AddGizmo(string objId, Func<Vector4> provider)
    {
        gizmoSphere.Add(new(objId, provider));
    }

    public void RemoveGizmo(string objId)
    {
        gizmoSphere.RemoveAll(kv => kv.Key == objId);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        gizmoSphere.Travel((kv) =>
        {
            var v = kv.Value.Invoke();
            Gizmos.DrawWireSphere(new Vector3(v.x, v.y, v.z), v.w);
        });
    }
}
