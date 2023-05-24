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

    public void UpdateLine(Vector3 startPos, List<Vector3> points)
    {
        lineRenderer.positionCount = points.Count + 1;

        lineRenderer.SetPosition(0, transform.TransformPoint(startPos));
        FC.For(points.Count, (i) => lineRenderer.SetPosition(i + 1, transform.TransformPoint(points[i])));
    }
}
