using System.Collections;
using System.Collections.Generic;
using TowerDefance.Game;
using UnityEngine;
using Swift.Math;
using TowerDefance.Res;
using TowerDefance;
using Swift;
using System;

public class TestUIManager : MonoBehaviour
{
    public Transform StartScreen;
    public Transform WinnerScreen;
    public Transform LoserScreen;

    public TestBattleScene BtScene;
    public TestTowerCardContainer CardContainer;
    public ITowerProvider TowerProvider;

    TowerDefanceBattle bt = null;

    public IEnumerator Start()
    {
        yield return ConfigManager.Init();

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
            var tower = Tower.Create(ConfigManager.GetTowerConfig(type));
            tower.Skill = new SkillAttackingTargets("PhysicalSingleAttack", 6, 1);
            tower.ValidTargetTypes = new Type[] { typeof(Enemy) };

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
        var btCfg = ConfigManager.GetBattleConfig("TestBattle");
        bt = TowerDefanceBattle.Create(btCfg);

        BtScene.Init(bt);

        bt.OnWon += (bt) =>
        {
            WinnerScreen.gameObject.SetActive(true);
            var aniCfg = ConfigManager.GetAvatarAnimationConfig("Archer");
            var ani = WinnerScreen.GetComponentInChildren<FrameAni>();
            ani.Data = new AniData()
            {
                label = aniCfg.label,
                interval = aniCfg.interval,
                loop = aniCfg.loop,
            };
            ani.Play();
        };

        bt.OnFailed += (bt) => LoserScreen.gameObject.SetActive(true);

        bt.Start();
    }
}
