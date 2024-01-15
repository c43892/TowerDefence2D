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
using static GalPanic.BattleMap;

public partial class BattleSceneRender : MonoBehaviour
{
    public BattleMapRenderer MapRenderer;
    public FrameAni Cursor;
    public TraceRenderer Trace;

    public Transform RectTopLeft;
    public Transform RectRightBottom;

    private Battle bt;
    private LocalDriver driver = new();

    public Battle Bt
    {
        get => bt;
        set
        {
            bt = value;
            driver.SetBattle(bt);

            if (bt == null)
                return;

            MapRenderer.Map = bt.Map;
            InitBattleEvents(bt);
            bt.OnCompletionChanged += (fillType, ptsChanged) => UpdateMap(fillType, ptsChanged);

            var leftTop = new Vector3(-bt.Map.Width, -bt.Map.Height, 0) / 10;
            var rightBottom = new Vector3(bt.Map.Width, bt.Map.Height, 0) / 10;
            SetRectArea(leftTop, rightBottom);
        }
    }

    public void SetCursorSpeed(Func<float> cursorSpeed, Func<float> cursorBackspeed)
    {
        driver.SetCursorSpeed(cursorSpeed, cursorBackspeed);
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

    public void UpdateMap(GridType fillType, List<Vec2> ptsChanged) => MapRenderer.UpdateMap(fillType, ptsChanged);
    public void SetAvatar(string frontAni, string backAni) => MapRenderer.SetAvatar(frontAni, backAni);

    private void Update()
    {
        if (Bt == null)
            return;

        CheckCursorAction();

        driver.OnTimeElapsed(Time.deltaTime);

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

    public Vector3 GetScenePos(Vec2 pos) => GetScenePos(pos.x, pos.y);
    public Vector3 GetScenePos(Fix64 x, Fix64 y)
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

    void CheckCursorAction()
    {
        var dx = 0;
        var dy = 0;
        var forceUnsafe = Input.GetKey(KeyCode.Space);

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            dx = -1;
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            dx = 1;
        else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            dy = 1;
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            dy = -1;

        driver.Input(dx, dy, forceUnsafe);

        UpdateLineRender();

        #endregion
    }
}
