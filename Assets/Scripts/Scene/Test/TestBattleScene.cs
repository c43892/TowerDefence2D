using Swift;
using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefance.Game;
using UnityEngine;

public partial class TestBattleScene : MonoBehaviour
{
    public Transform UnitRoot;

    public GameObject EnemyModel;
    public GameObject TowerModel;

    TestBattle bt;
    readonly Dictionary<string, GameObject> unitObjs = new();

    private void Start()
    {
        bt = new TestBattle();
        bt.Init();

        InitUnits();
        InitBattleEventsHandler();
    }

    void InitUnits()
    {
        foreach (var obj in unitObjs.Values)
            Destroy(obj);

        unitObjs.Clear();

        bt.Map.ForEachUnit((unit) =>
        {
            GameObject obj = null;

            if (unit is Enemy)
                obj = Instantiate(EnemyModel);
            else if (unit is Tower)
                obj = Instantiate(TowerModel);
            else
                throw new Exception($"unsupported unit type {unit.GetType().Name}");

            obj.transform.SetParent(UnitRoot);
            obj.transform.localPosition = new Vector3((float)unit.Pos.x, (float)unit.Pos.y, 0);
            obj.SetActive(true);
            unitObjs[unit.UID] = obj;
        });
    }

    void UpdateUnitsModels()
    {
        bt.Map.ForEachUnit((unit) =>
        {
            unitObjs[unit.UID].transform.localPosition = new Vector3((float)unit.Pos.x, (float)unit.Pos.y, 0);
        });
    }

    GameObject GetUnitObj(string uid)
    {
        return unitObjs[uid];
    }

    private void Update()
    {
        var dt = Time.deltaTime;
        bt.OnTimeElapsed((int)(dt * 1000));

        UpdateUnitsModels();
        UpdateBattleEffect(dt);
    }
}
