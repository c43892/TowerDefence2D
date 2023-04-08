using Swift.Math;
using System;
using System.Collections.Generic;
using TowerDefance.Game;
using Unity.VisualScripting;
using UnityEngine;

public partial class TestBattleScene : MonoBehaviour
{
    public Transform UnitRoot;
    public Transform MapRoot;

    public GameObject EnemyModel;
    public GameObject TowerModel;
    public GameObject TowerBaseModel;

    TestBattle Bt { get; set; }
    readonly Dictionary<string, GameObject> unitObjs = new();

    private void Start()
    {
        InitBattleEventsHandler();
    }

    public void Init(TestBattle bt)
    {
        foreach (var obj in unitObjs.Values)
            Destroy(obj);

        unitObjs.Clear();

        Bt = bt;
        Bt.Map.ForEachUnit(AddUnitObj);
    }

    void UpdateUnitsModels()
    {
        Bt.Map.ForEachUnit((unit) =>
        {
            unitObjs[unit.UID].transform.localPosition = new Vector3((float)unit.Pos.x, (float)unit.Pos.y, 0);
        });
    }

    GameObject GetUnitObj(string uid)
    {
        return unitObjs.ContainsKey(uid) ? unitObjs[uid] : null;
    }

    void RemoveUnitObj(string uid)
    {
        var unitObj = unitObjs[uid];
        Destroy(unitObj);
        unitObjs.Remove(uid);
    }

    void AddUnitObj(BattleMap.IUnit unit)
    {
        GameObject obj;
        if (unit is Enemy)
            obj = Instantiate(EnemyModel);
        else if (unit is Tower)
            obj = Instantiate(TowerModel);
        else if (unit is TowerBase)
            obj = Instantiate(TowerBaseModel);
        else
            throw new Exception($"unsupported unit type {unit.GetType().Name}");

        obj.transform.SetParent(UnitRoot);
        obj.transform.localPosition = new Vector3((float)unit.Pos.x, (float)unit.Pos.y, 0);
        obj.SetActive(true);
        unitObjs[unit.UID] = obj;
    }

    private void Update()
    {
        if (Bt == null)
            return;

        var dt = Time.deltaTime;

        if (Bt.Running)
            Bt.OnTimeElapsed((int)(dt * 1000));

        UpdateUnitsModels();
        UpdateBattleEffect(dt);
    }
}
