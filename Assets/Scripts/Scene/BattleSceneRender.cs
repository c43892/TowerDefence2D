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

public partial class BattleSceneRender : MonoBehaviour
{
    public BattleMapRenderer MapRenderer;
    public FrameAni Cursor;
    public TraceRenderer Trace;

    public Transform RectTopLeft;
    public Transform RectRightBottom;

    public Func<float> GetCursorSpeed = null;

    private Battle bt;
    public Battle Bt
    {
        get => bt;
        set
        {
            bt = value;
            if (bt == null)
                return;

            MapRenderer.Map = bt.Map;
            InitBattleEvents(bt);
            bt.OnCompletionChanged += UpdateMap;

            var leftTop = new Vector3(-bt.Map.Width / 2, -bt.Map.Height / 2, 0) / 10;
            var rightBottom = new Vector3(bt.Map.Width / 2, bt.Map.Height / 2, 0) / 10;
            SetRectArea(leftTop, rightBottom);
        }
    }

    private void SetRectArea(Vector3 topLeft, Vector3 rightBottom)
    {
        RectTopLeft.localPosition = topLeft;
        RectRightBottom.localPosition = rightBottom;
        Trace.transform.localPosition = RectTopLeft.localPosition;
    }

    public void Clear()
    {
        MapRenderer.Clear();
        UpdateLineRender();
        ClearUnits();
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

        bt.OnTimeElapsed(Time.deltaTime);

        UpdateCursor();
        UpdateUnitObjs();
    }

    #region cursor controlling

    void SetPos(Transform t, Vec2 pos)
    {
        var divX = (float)(pos.x) / (bt.Map.Width - 1);
        var divY = (float)(pos.y) / (bt.Map.Height - 1);
        var x = RectTopLeft.localPosition.x + (RectRightBottom.localPosition.x - RectTopLeft.localPosition.x) * divX;
        var y = RectTopLeft.localPosition.y + (RectRightBottom.localPosition.y - RectTopLeft.localPosition.y) * divY;
        t.localPosition = new Vector3(x, y, 0);
    }

    void UpdateCursor()
    {
        SetPos(Cursor.transform, new Vec2(bt.CursorX, bt.CursorY));
    }

    void UpdateLineRender()
    {
        var rectSize = RectRightBottom.localPosition - RectTopLeft.localPosition;
        Trace.UpdateLine(TraceLine.Select((pt) => new Vector3(
            pt.Key * rectSize.x / bt.Map.Width,
            pt.Value * rectSize.y / bt.Map.Height,
            0)).ToList());
    }

    private KeyValuePair<int, int> StartPt => bt.CursorStartPos;
    private List<KeyValuePair<int, int>> TraceLine => bt.TraceLine;
    private float tracingDelayTimer = 0;

    void CheckArrowKeysUpDown()
    {
        var forceUnsafe = Input.GetKey(KeyCode.Space);
        Cursor.Data = ConfigManager.GetEffectAnimationConfig(bt.IsCursorSafe ? "SafeCursor" : "UnsafeCursor").ToData();
        Cursor.Play(true);

        var cursorSpeed = GetCursorSpeed == null ? 0 : GetCursorSpeed();

        tracingDelayTimer += Time.deltaTime;
        if (bt.Ended || tracingDelayTimer < 1 / cursorSpeed)
            return;

        tracingDelayTimer -= 1 / cursorSpeed;

        var dx = 0;
        var dy = 0;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            dx = -1;
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            dx = 1;
        else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            dy = 1;
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            dy = -1;
            

        if (dx == 0 && dy == 0)
        {
            if (TraceLine.Count > 1)
            {
                TraceLine.RemoveAt(TraceLine.Count - 1);
                var pt = TraceLine.Count > 1 ? TraceLine[^1] : StartPt;
                bt.ForceCursor(pt.Key, pt.Value);
                UpdateLineRender();
            }
        }
        else
        {
            var x = bt.CursorX + dx;
            var y = bt.CursorY + dy;
            var pt = new KeyValuePair<int, int>(x, y);
            var n = TraceLine.IndexOf(pt);

            if (n >= 0 && n == TraceLine.Count - 2)
            {
                // go back 1 step
                TraceLine.RemoveAt(TraceLine.Count - 1);
                UpdateLineRender();
                bt.ForceCursor(TraceLine.Count > 0 ? TraceLine[^1] : StartPt);
            }
            else if (n < 0)
            {
                var oldX = bt.CursorX;
                var oldY = bt.CursorY;

                if (bt.TryMovingCursor(dx, dy, forceUnsafe))
                {
                    if (bt.Map[x, y] == BattleMap.GridType.Covered)
                    {
                        if (TraceLine.Count == 0)
                            bt.StartAt(oldX, oldY);

                        TraceLine.Add(new(bt.CursorX, bt.CursorY));
                        UpdateLineRender();
                    }
                    else if (TraceLine.Count > 0)
                    {
                        TraceLine.Add(new(bt.CursorX, bt.CursorY));

                        // find a straight line at least having 3 pts
                        var onX = 0;
                        var onY = 0;
                        var middlePt = TraceLine[0];
                        for (var i = 1; i < TraceLine.Count - 1; i++)
                        {
                            var pt0 = TraceLine[i - 1];
                            var pt1 = TraceLine[i];
                            var pt2 = TraceLine[i + 1];

                            if (pt2.Key - pt1.Key == pt1.Key - pt0.Key) onX = pt1.Key - pt0.Key;
                            if (pt2.Value - pt1.Value == pt1.Value - pt0.Value) onY = pt1.Value - pt0.Value;

                            if (onX != 0 || onY != 0)
                            {
                                middlePt = pt1;
                                break;
                            }
                        }

                        bt.Map.FillPts(TraceLine, BattleMap.GridType.Uncovered);

                        TraceLine.Clear();
                        UpdateLineRender();

                        if (onX != 0)
                            bt.Map.CompeteFilling(middlePt.Key, middlePt.Value - 1, middlePt.Key, middlePt.Value + 1);
                        else if (onY != 0)
                            bt.Map.CompeteFilling(middlePt.Key - 1, middlePt.Value, middlePt.Key + 1, middlePt.Value);
                    }
                }
            }
        }

        #endregion

    }
}
