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

        CheckArrowKeysUpDown();

        var divX = (float)bt.CursorX / bt.Map.Width;
        var divY = (float)bt.CursorY / bt.Map.Height;
        var x = RectTopLeft.localPosition.x + (RectRightBottom.localPosition.x - RectTopLeft.localPosition.x) * divX;
        var y = RectTopLeft.localPosition.y + (RectRightBottom.localPosition.y - RectTopLeft.localPosition.y) * divY;
        Cursor.localPosition = new Vector3(x, y, 0);
    }

    private KeyValuePair<int, int> startPt = new(0, 0);
    private readonly List<KeyValuePair<int, int>> traceLine = new(); 
    void CheckArrowKeysUpDown()
    {
        var dx = 0;
        var dy = 0;

        if (Input.GetKey(KeyCode.LeftArrow))
            dx = -1;
        else if (Input.GetKey(KeyCode.RightArrow))
            dx = 1;
        else if (Input.GetKey(KeyCode.UpArrow))
            dy = 1;
        else if (Input.GetKey(KeyCode.DownArrow))
            dy = -1;

        void UpdateLineRender()
        {
            var rectSize = RectRightBottom.localPosition - RectTopLeft.localPosition;
            Trace.UpdateLine(traceLine.Select((pt) => new Vector3(
                pt.Key * rectSize.x / bt.Map.Width,
                pt.Value * rectSize.y / bt.Map.Height,
                0)).ToList());
        };

        if (dx == 0 && dy == 0)
        {
            if (traceLine.Count > 0)
            {
                traceLine.RemoveAt(traceLine.Count - 1);
                var pt = traceLine.Count > 0 ? traceLine[^1] : startPt;
                bt.ForceCursor(pt.Key, pt.Value);
                UpdateLineRender();
            }
        }
        else
        {
            if (traceLine.Count == 0)
            {
                startPt = new(bt.CursorX, bt.CursorY);
                traceLine.Add(startPt);
            }

            var x = bt.CursorX + dx;
            var y = bt.CursorY + dy;
            var pt = new KeyValuePair<int, int>(x, y);
            var n = traceLine.IndexOf(pt);

            if (n >= 0 && n == traceLine.Count - 2)
            {
                // go back 1 step
                traceLine.RemoveAt(traceLine.Count - 1);
                UpdateLineRender();
                bt.ForceCursor(traceLine.Count > 0 ? traceLine[^1] : startPt);
            }
            else if (n < 0 && bt.TryMovingCursor(dx, dy))
            {
                traceLine.Add(new(bt.CursorX, bt.CursorY));
                UpdateLineRender();
            }
        }
    }
}
