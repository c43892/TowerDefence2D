using System.Collections;
using System.Collections.Generic;
using TowerDefance.Game;
using UnityEngine;
using Swift;
using System;

[RequireComponent(typeof(MultiFrameAni))]
public class BattleMapUnitModel : MonoBehaviour
{
    public BattleMapUnit Unit;

    private MultiFrameAni ani;

    public void Play(string aniName, Action onEnded = null)
    {
        var ani = GetComponent<MultiFrameAni>();
        if (ani.Data == null)
            LoadAniData();

        ani.Play(aniName, onEnded);
    }

    private void Update()
    {
        transform.localPosition = new Vector3((float)Unit.Pos.x, (float)Unit.Pos.y, 0);
    }

    void LoadAniData()
    {
        var aniCfgs = ConfigManager.GetUnitAnimationConfigs(Unit.Type);
        var multiAniData = new MultiAniData();
        aniCfgs.Keys.Travel(aniName => multiAniData.Data[aniName] = new AniData() { label = aniCfgs[aniName].label, interval = aniCfgs[aniName].interval, loop = aniCfgs[aniName].loop });

        ani = GetComponent<MultiFrameAni>();
        ani.Data = multiAniData;
    }
}
