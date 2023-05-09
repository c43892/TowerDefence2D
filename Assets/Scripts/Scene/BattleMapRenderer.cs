using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalPanic;
using Swift;
using Swift.Math;
using System.Linq;
using GalPanic.Res;
using UnityEngine.UI;

public class BattleMapRenderer : MonoBehaviour
{
    public FrameAni BackgroundAni;

    public BattleMap Map { get; set; }

    private Texture2D MaskTex;
    private Color[] MaskColors;

    public void UpdateMap()
    {
        if (Map == null)
            return;

        MaskTex = new Texture2D(Map.Width, Map.Height);
        MaskColors = new Color[Map.Width * Map.Height];

        Color Covered = new(0, 0, 0, 0);
        Color Uncovered = new(1, 1, 1, 1);

        FC.For2(Map.Width, Map.Height, (x, y) => MaskColors[y * Map.Width + x] = (y >= Map.Height / 2 ? Covered : Uncovered));
        MaskTex.SetPixelData(MaskColors, 0);

        var cfg = ConfigManager.GetAvatarAnimationConfig("Archer");
        BackgroundAni.Data = new AniData()
        {
            label = cfg.label,
            interval = cfg.interval,
            loop = cfg.loop
        };

        BackgroundAni.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
