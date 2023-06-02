using System;
using System.Collections;
using System.Collections.Generic;
using GalPanic;
using GalPanic.Res;
using Swift;
using Swift.Math;
using UnityEngine;

public class ConfigManager
{
    static Dictionary<string, Dictionary<string, AnimationConfig>> unitAniConfigs = null;
    static Dictionary<string, AnimationConfig> avatarAniConfigs = null;
    static Dictionary<string, AnimationConfig> effectAniConfigs = null;
    static Dictionary<string, BattleUnitConfig> battleUnitConfigs = null;
    static Dictionary<string, BattleConfig> battleConfigs = null;

    public static IEnumerator Init()
    {
        yield return ResManager.LoadConfig<Dictionary<string, Dictionary<string, AnimationConfig>>>("UnitAnimations", configs => unitAniConfigs = configs);
        yield return ResManager.LoadConfig<Dictionary<string, AnimationConfig>>("AvatarAnimations", configs => avatarAniConfigs = configs);
        yield return ResManager.LoadConfig<Dictionary<string, AnimationConfig>>("EffectAnimations", configs => effectAniConfigs = configs);
        yield return ResManager.LoadConfig<Dictionary<string, BattleUnitConfig>>("BattleUnits", configs => battleUnitConfigs = configs);
        yield return ResManager.LoadConfig<Dictionary<string, BattleConfig>>("Battles", configs => battleConfigs = configs);
    }

    public static Dictionary<string, AnimationConfig> GetUnitAnimationConfigs(string name) => unitAniConfigs[name];
    public static AnimationConfig GetAvatarAnimationConfig(string name) => avatarAniConfigs[name];
    public static AnimationConfig GetEffectAnimationConfig(string name) => effectAniConfigs[name];
    public static BattleUnitConfig GetBattleUnitConfig(string name) => GetBattleUnitConfig(name, new());
    private static BattleUnitConfig GetBattleUnitConfig(string name, List<string> mergedList)
    {
        var cfg = battleUnitConfigs[name];
        if (cfg.merged || cfg.merges == null)
            return cfg;

        cfg.merges?.Travel(m =>
        {
            if (mergedList.Contains(m))
            {
                var mergedStr = name + " - ";
                mergedList.Travel(m => mergedStr += m + " - ");
                mergedStr += m;
                throw new Exception($"BattleUnit config has a merging loop: {mergedStr}");
            }

            var mergeCfg = GetBattleUnitConfig(m, mergedList);

            if (cfg.type == null)
                cfg.type = mergeCfg.type;

            foreach (var k in mergeCfg.ai.Keys)
            {
                if (!cfg.ai.ContainsKey(k))
                    cfg.ai[k] = mergeCfg.ai[k];
                else
                {
                    foreach (var pk in mergeCfg.ai[k].Keys)
                    {
                        if (!cfg.ai[k].ContainsKey(pk))
                            cfg.ai[k][pk] = mergeCfg.ai[k][pk];
                    }
                }
            }
        });

        cfg.merged = true;

        return battleUnitConfigs[name];
    }
    public static BattleConfig GetBattleConfig(string name) => battleConfigs[name];

    public class SpawnUnitIntervallyCfg
    {
        public Fix64 Interval;
    }
}
