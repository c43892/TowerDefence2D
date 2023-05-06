using System;
using System.Collections;
using System.Collections.Generic;
using GalPanic;
using GalPanic.Res;
using Swift;
using UnityEngine;

public class ConfigManager
{
    static Dictionary<string, Dictionary<string, AnimationConfig>> aniConfigs = null;
    static Dictionary<string, AnimationConfig> avatarAniConfigs = null;
    static Dictionary<string, AnimationConfig> effectAniConfigs = null;

    public static IEnumerator Init()
    {
        yield return ResManager.LoadConfig<Dictionary<string, Dictionary<string, AnimationConfig>>>("UnitAnimations", configs => aniConfigs = configs);
        yield return ResManager.LoadConfig<Dictionary<string, AnimationConfig>>("AvatarAnimations", configs => avatarAniConfigs = configs);
        yield return ResManager.LoadConfig<Dictionary<string, AnimationConfig>>("EffectAnimations", configs => effectAniConfigs = configs);
    }

    public static Dictionary<string, AnimationConfig> GetUnitAnimationConfigs(string name) => aniConfigs[name];
    public static AnimationConfig GetAvatarAnimationConfig(string name) => avatarAniConfigs[name];
    public static AnimationConfig GetEffectAnimationConfig(string name) => effectAniConfigs[name];
}
