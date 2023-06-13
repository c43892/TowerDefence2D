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
    private readonly List<KeyValuePair<string, Func<Vector4>>> gizmoRect = new();

    public void OnBattleEnded() => gizmoSphere.Clear();

    public void OnUnitAdded(BattleUnit u)
    {
        if (u.Radius > 0)
        {
            AddGizmoSphere(u.UID, () =>
            {
                var pos = SceneRenderer.GetUnitRenderer(u.UID).transform.position;
                return new Vector4(pos.x, pos.y, pos.z, (float)(u.Radius * u.Scale));
            });
        }
    }

    public void OnUnitRemoved(BattleUnit u)
    {
        RemoveGizmo(u.UID);
    }

    public void OnAiAdded(string ai, BattleUnit u, object args)
    {
        switch(ai)
        {
            case "StaticObstacle":
                int[] area = args as int[];
                AddGizmoRect(u.UID, () => new Vector4(area[0], area[1], area[2], area[3]));
                break;
        }
    }

    public void AddGizmoSphere(string objId, Func<Vector4> provider)
    {
        gizmoSphere.Add(new(objId, provider));
    }

    public void AddGizmoRect(string objId, Func<Vector4> provider)
    {
        gizmoRect.Add(new(objId, provider));
    }

    public void RemoveGizmo(string objId)
    {
        gizmoSphere.RemoveAll(kv => kv.Key == objId);
        gizmoRect.RemoveAll(kv => kv.Key == objId);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        gizmoSphere.Travel((kv) =>
        {
            var v = kv.Value.Invoke();
            Gizmos.DrawWireSphere(new Vector3(v.x, v.y, v.z), v.w / 10);
        });
        gizmoRect.Travel((kv) =>
        {
            var v = kv.Value.Invoke();
            var x = v.x + v.z / 2 + 0.5f;
            var y = v.y + v.z / 2 + 0.5f;
            var w = v.z / 10f;
            var h = v.w / 10f;
            Gizmos.DrawCube(SceneRenderer.GetScenePos(new Vec2(x, y)), new Vector3(w, h, 0));
        });
    }
}
