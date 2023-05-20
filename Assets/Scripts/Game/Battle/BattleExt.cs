using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using System;
using System.Linq;
using Unity.VisualScripting;

namespace GalPanic
{
    public static class BattleExt
    {
        public static BattleUnit AddUnitAt(this Battle battle, string unitType, Vec2 pos)
        {
            var cfg = ConfigManager.GetBattleUnitConfig(unitType);
            var unit = new BattleUnit(
                battle.Map,
                cfg.type,
                cfg.ai,
                cfg.radius,
                pos,
                new Vec2(cfg.speedX, cfg.speedY)
            );

            battle.Map.AddUnit(unit);
            return unit;
        }
    }
}
