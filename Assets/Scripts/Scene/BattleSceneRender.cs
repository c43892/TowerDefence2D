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

    public void UpdateMap() => MapRenderer.UpdateMap();
    public void SetAvatar(string frontAni, string backAni) => MapRenderer.SetAvatar(frontAni, backAni);

    private void Update()
    {
        if (Bt == null)
            return;

        CheckCursorAction();

        bt.OnTimeElapsed(Time.deltaTime);

        UpdateCursor();
        UpdateUnitObjs();
    }

    private void UpdateUnitObjs()
    {
        unitObjs.Values.Travel(obj =>
        {
            SetPos(obj.transform, obj.Unit.Pos);
            obj.transform.localRotation = Quaternion.Euler(0, 0, (float)obj.Unit.Dir);
            obj.transform.localScale = Vector3.one * (float)obj.Unit.Scale;
        });
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

    string oldCursorStatus = "";
    void UpdateCursor()
    {
        SetPos(Cursor.transform, bt.Cursor.Pos);

        var cursorStatus = bt.Cursor.CoolDown > 0 ? "CoolDown" : (bt.Map.IsBlocked(bt.Cursor.Pos) ? "SafeCursor" : "UnsafeCursor");
        if (oldCursorStatus == cursorStatus)
            return;

        oldCursorStatus = cursorStatus;
        Cursor.Data = ConfigManager.GetEffectAnimationConfig(cursorStatus).ToData();
        Cursor.Play(true);
    }

    void UpdateLineRender()
    {
        var rectSize = RectRightBottom.localPosition - RectTopLeft.localPosition;
        Vector3 Pos2V3(Vec2 pt) => new((float)pt.x * rectSize.x / bt.Map.Width, (float)pt.y * rectSize.y / bt.Map.Height, 0);
        Trace.UpdateLine(Pos2V3(bt.Cursor.StartPos), bt.Cursor.TraceLine.ToList().Select(Pos2V3).ToList());
    }

    private float tracingDelayTimer = 0;

    void CheckCursorAction()
    {
        var forceUnsafe = Input.GetKey(KeyCode.Space);
        var cursorSpeed = GetCursorSpeed == null ? 0 : GetCursorSpeed();

        tracingDelayTimer += Time.deltaTime;
        if (bt.Cursor.CoolDown > 0 || bt.Ended || tracingDelayTimer < 1 / cursorSpeed)
            return;

        tracingDelayTimer %= 1 / cursorSpeed;

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
            bt.SetbackCursor();
        else
            bt.TryMoveCursor(dx, dy, forceUnsafe);

        UpdateLineRender();

        #endregion
    }
}
