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

public class BattleMapRenderer : MonoBehaviour
{
    public FrameAni BackgroundAni;

    public BattleMap Map { get; set; }

    private Texture2D MaskTex;
    private Color[] MaskColors;

    public void Clear()
    {
    }

    public void UpdateMap()
    {
        if (Map == null)
            return;

        if (MaskTex == null || MaskColors == null)
        {
            MaskTex = new Texture2D(Map.Width, Map.Height);
            MaskColors = new Color[Map.Width * Map.Height];
            BackgroundAni.MaskTex = MaskTex;
        }

        Color32 Covered = new(0, 0, 0, 0);
        Color32 Uncovered = new(255, 255, 255, 255);

        NativeArray<Color32> pixels = new(Map.Width * Map.Height, Allocator.Temp);
        FC.For2(Map.Width, Map.Height, (x, y) => pixels[y * Map.Width + x] = Map[x, y] == BattleMap.GridType.Uncovered ? Uncovered : Covered);

        // Use SetPixelData to apply the pixel data to the texture.
        MaskTex.SetPixelData(pixels, 0);
        MaskTex.Apply();

        pixels.Dispose();

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
