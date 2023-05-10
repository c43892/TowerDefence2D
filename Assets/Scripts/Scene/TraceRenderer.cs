using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Swift;

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

        FC.For(points.Count, (i) => lineRenderer.SetPosition(i, transform.TransformPoint(points[i])));
    }
}
