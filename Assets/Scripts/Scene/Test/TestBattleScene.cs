using Swift;
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

    public GameObject UnitModel;
    public GameObject TowerBaseModel;

    TowerDefanceBattle Bt { get; set; }
    readonly Dictionary<string, GameObject> unitObjs = new();

    private void Start()
    {
        InitBattleEventsHandler();
    }

    public void Init(TowerDefanceBattle bt)
    {
        ClearObjs();

        Bt = bt;
    }

    void ClearObjs()
    {
        foreach (var obj in unitObjs.Values)
            Destroy(obj);

        unitObjs.Clear();
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

    BattleMapUnitModel AddUnitObj(BattleMapUnit unit)
    {
        var untiObj = Instantiate(UnitModel);
        unitObjs[unit.UID] = untiObj;

        untiObj.transform.SetParent(UnitRoot);
        untiObj.SetActive(true);

        var model = untiObj.GetComponent<BattleMapUnitModel>();
        model.Unit = unit;

        return model;
    }

    private void Update()
    {
        if (Bt == null)
            return;

        var dt = Time.deltaTime;

        if (Bt.Running)
            Bt.OnTimeElapsed(dt);

        UpdateBattleEffect(dt);
    }
}
