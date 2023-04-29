using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefance;
using TowerDefance.Res;
using Swift;
using UnityEngine;

public class ConfigManager
{
    static Dictionary<string, BattleConfig> battleConfigs = null;
    static Dictionary<string, TowerConfig> towerConfigs = null;
    static Dictionary<string, EnemyConfig> enemyConfigs = null;
    static Dictionary<string, Dictionary<string, AnimationConfig>> aniConfigs = null;
    static Dictionary<string, AnimationConfig> avatarAniConfigs = null;
    static Dictionary<string, AnimationConfig> effectAniConfigs = null;

    public static IEnumerator Init()
    {
        yield return ResManager.LoadConfig<Dictionary<string, BattleConfig>>("Battles", configs => battleConfigs = configs);
        yield return ResManager.LoadConfig<Dictionary<string, TowerConfig>>("Towers", configs => towerConfigs = configs);
        yield return ResManager.LoadConfig<Dictionary<string, EnemyConfig>>("Enemies", configs => enemyConfigs = configs);
        yield return ResManager.LoadConfig<Dictionary<string, Dictionary<string, AnimationConfig>>>("UnitAnimations", configs => aniConfigs = configs);
        yield return ResManager.LoadConfig<Dictionary<string, AnimationConfig>>("AvatarAnimations", configs => avatarAniConfigs = configs);
        yield return ResManager.LoadConfig<Dictionary<string, AnimationConfig>>("EffectAnimations", configs => effectAniConfigs = configs);
    }

    public static BattleConfig GetBattleConfig(string name) => battleConfigs[name];
    public static TowerConfig GetTowerConfig(string name) => towerConfigs[name];
    public static EnemyConfig GetEnemyConfig(string name) => enemyConfigs[name];
    public static Dictionary<string, AnimationConfig> GetUnitAnimationConfigs(string name) => aniConfigs[name];
    public static AnimationConfig GetAvatarAnimationConfig(string name) => avatarAniConfigs[name];
    public static AnimationConfig GetEffectAnimationConfig(string name) => effectAniConfigs[name];
}
