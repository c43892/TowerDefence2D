using System.Collections;
using System.Collections.Generic;
using TowerDefance.Game;
using UnityEngine;
using Swift.Math;

public class TestUIManager : MonoBehaviour
{
    public Transform StartScreen;
    public Transform WinnerScreen;
    public Transform LoserScreen;

    public TestBattleScene BtScene;
    public TestTowerCardContainer CardContainer;
    public ITowerProvider TowerProvider;

    TestBattle bt = null;

    public void Start()
    {
        StartScreen.gameObject.SetActive(true);
        WinnerScreen.gameObject.SetActive(false);
        LoserScreen.gameObject.SetActive(false);

        Init();
    }

    private void Init()
    {
        CardContainer.WorldPos2BattleMapPos = (wPos) =>
        {
            var mapPos = BtScene.MapRoot.worldToLocalMatrix.MultiplyPoint(wPos);
            var gx = (int)(mapPos.x + 0.5f);
            var gy = (int)(mapPos.y + 0.5f);
            return new Vec2(gx, gy);
        };

        CardContainer.BattleMapPos2UnitObjLocalPos = (mapPos) => new Vector3((float)mapPos.x, (float)mapPos.y, -1);

        CardContainer.OnCreatingTower += (type, pos) =>
        {
            var tower = TowerProvider.CreateTower(type);
            bt.AddUnitAt(tower, pos);
            CardContainer.Refresh();
        };
    }

    public void OnStartBtn()
    {
        StartScreen.gameObject.SetActive(false);
        WinnerScreen.gameObject.SetActive(false);
        LoserScreen.gameObject.SetActive(false);

        StartBattle();
        StartUI();
    }

    void StartUI()
    {
        TowerProvider = new TestPlayer(3);

        CardContainer.AllValidTowerTypes = () => TowerProvider.AllValidTowerTypes;
        CardContainer.IsValidTowerPos = (pos) => bt.Map.IsOccupiedAt(pos);
        CardContainer.Refresh();
    }

    void StartBattle()
    {
        bt = new TestBattle();
        bt.Init();

        BtScene.Init(bt);

        bt.OnWon += (bt) => WinnerScreen.gameObject.SetActive(true);
        bt.OnFailed += (bt) => LoserScreen.gameObject.SetActive(true);

        bt.Start();
    }
}
