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

    public void OnBattleEnded() => gizmoSphere.Clear();

    public void OnUnitAdded(BattleUnit u)
    {
        if (u.Radius > 0)
        {
            AddGizmo(u.UID, () =>
            {
                var pos = SceneRenderer.GetUnitRenderer(u.UID).transform.position;
                return new Vector4(pos.x, pos.y, pos.z, (float)(u.Radius * u.Scale));
            });
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
            Gizmos.DrawWireSphere(new Vector3(v.x, v.y, v.z), v.w / 10);
        });
    }
}
