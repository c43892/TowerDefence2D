using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TraceRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void UpdateLine(List<Vector3> points)
    {
        lineRenderer.positionCount = points.Count;

        for (int i = 0; i < points.Count; i++)
            lineRenderer.SetPosition(i, transform.TransformPoint(points[i]));
    }
}
