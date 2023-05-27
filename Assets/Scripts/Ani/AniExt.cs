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
    public static class AniExt
    {
        public static AniData ToData(this AnimationConfig cfg)
        {
            return new AniData()
            {
                label = cfg.label,
                scale = cfg.scale,
                interval = cfg.interval,
                loop = cfg.loop,
                flipX = cfg.flipX,
                flipY = cfg.flipY
            };
        }
    }
}
