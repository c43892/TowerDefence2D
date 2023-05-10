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

public class BattleSceneRender : MonoBehaviour
{
    public BattleMapRenderer MapRenderer;
    public Transform Cursor;
    public TraceRenderer Trace;

    public Transform RectTopLeft;
    public Transform RectRightBottom;

    private Battle bt;
    public Battle Bt
    {
        get => bt;
        set
        {
            bt = value;
            MapRenderer.Map = bt.Map;
        }
    }

    private void Start()
    {
        Trace.transform.localPosition = RectTopLeft.localPosition;
    }

    public void UpdateMap()
    {
        MapRenderer.UpdateMap();
    }

    private void Update()
    {
        if (Bt == null)
            return;

        var divX = (float)bt.CursorX / bt.Map.Width;
        var divY = (float)bt.CursorY / bt.Map.Height;
        var x = RectTopLeft.localPosition.x + (RectRightBottom.localPosition.x - RectTopLeft.localPosition.x) * divX;
        var y = RectTopLeft.localPosition.y + (RectRightBottom.localPosition.y - RectTopLeft.localPosition.y) * divY;
        Cursor.localPosition = new Vector3(x, y, 0);
    }
}
