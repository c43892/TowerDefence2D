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

[RequireComponent(typeof(FrameAni))]
public class BattleUnitRenderer : MonoBehaviour
{
    public FrameAni Ani;
    public BattleUnit Unit;

    private void Start()
    {
        Ani = GetComponent<FrameAni>();
    }

    public void UpdateUnit()
    {
        if (Unit == null)
            return;

        var cfgs = ConfigManager.GetUnitAnimationConfigs(Unit.Type);
        Ani.Data = cfgs.Values.ToArray()[0].ToData();
        Ani.Play();
    }
}
