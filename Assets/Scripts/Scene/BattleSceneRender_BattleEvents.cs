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
using Unity.VisualScripting;

public partial class BattleSceneRender : MonoBehaviour
{
    public Transform UnitRoot;
    public BattleUnitRenderer UnitModel;

    readonly Dictionary<string, BattleUnitRenderer> unitObjs = new();

    void InitBattleEvents(Battle bt)
    {
        bt.Map.OnUnitAdded += OnUnitAdded;
        bt.Map.OnUnitRemoved += OnUnitRemoved;
    }

    private void ClearUnits()
    {
        unitObjs.Values.Travel(u => Destroy(u.gameObject));
        unitObjs.Clear();
    }

    private void OnUnitAdded(BattleUnit u)
    {
        var obj = Instantiate(UnitModel, UnitRoot);
        unitObjs[u.UID] = obj;

        obj.gameObject.SetActive(true);
        obj.Unit = u;
        obj.UpdateUnit();
    }

    private void OnUnitRemoved(BattleUnit u)
    {
        var obj = unitObjs[u.UID].gameObject;
        unitObjs.Remove(u.UID);
        Destroy(obj);
    }

    void UpdateUnits()
    {
        unitObjs.Values.Travel(obj => SetPos(obj.transform, obj.Unit.Pos));
    }
}
