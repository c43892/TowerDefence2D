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
    public Func<float> GetCursorBackSpeed = null;

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
            bt.OnCompletionChanged += (_) => UpdateMap();

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

    public Vector3 GetScenePos(Vec2 pos) => GetScenePos((int)pos.x, (int)pos.y);
    public Vector3 GetScenePos(int x, int y)
    {
        var divX = (float)x / (bt.Map.Width - 1);
        var divY = (float)y / (bt.Map.Height - 1);
        var sx = RectTopLeft.localPosition.x + (RectRightBottom.localPosition.x - RectTopLeft.localPosition.x) * divX;
        var sy = RectTopLeft.localPosition.y + (RectRightBottom.localPosition.y - RectTopLeft.localPosition.y) * divY;
        return new Vector3(sx, sy, 0);
    }

    void SetPos(Transform t, Vec2 pos)
    {
        t.localPosition = GetScenePos(pos);
    }

    string oldCursorStatus = "";
    void UpdateCursor()
    {
        SetPos(Cursor.transform, bt.Cursor.Pos);

        var cursorStatus = bt.Cursor.CoolDown > 0 ? "CoolDown" : (bt.IsCursorSafe ? "SafeCursor" : "UnsafeCursor");
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

    private float movingTimer = 0;
    private float backMovingTimer;

    void CheckCursorAction()
    {
        var forceUnsafe = Input.GetKey(KeyCode.Space);
        var cursorSpeed = GetCursorSpeed == null ? 0 : GetCursorSpeed();
        var cursorBackSpeed = GetCursorBackSpeed == null ? 0 : GetCursorBackSpeed();

        if (bt.Cursor.CoolDown > 0 || bt.Ended)
            return;

        movingTimer += cursorSpeed * Time.deltaTime;
        backMovingTimer += cursorBackSpeed * Time.deltaTime;

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

        if (movingTimer >= 1)
        {
            if (dx != 0 || dy != 0)
                bt.TryMoveCursor(dx, dy, forceUnsafe);

            movingTimer %= 1;
        }

        if (backMovingTimer > 1)
        {
            if (dx == 0 && dy == 0)
                bt.SetbackCursor();

            backMovingTimer %= 1;
        }

        UpdateLineRender();

        #endregion
    }
}
