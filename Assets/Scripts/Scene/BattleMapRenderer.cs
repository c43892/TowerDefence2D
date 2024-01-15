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
using static GalPanic.BattleMap;

public class BattleMapRenderer : MonoBehaviour
{
    public FrameAni ForegroundAni;
    public FrameAni BackgroundAni;

    public BattleMap Map { get; set; }

    private Texture2D MaskTex;
    private Color[] MaskColors;

    public void Clear()
    {
    }

    public void SetAvatar(string frontAni, string backAni)
    {
        BackgroundAni.Data = ConfigManager.GetAvatarAnimationConfig(backAni).ToData();
        ForegroundAni.Data = ConfigManager.GetAvatarAnimationConfig(frontAni).ToData();

        BackgroundAni.Play();
        ForegroundAni.Play();
    }

    NativeArray<Color32> pixels = default;
    bool dirty = false;
    public void UpdateMap(GridType fillType, List<Vec2> ptsChanged)
    {
        if (Map == null || (ptsChanged != null && ptsChanged.Count == 0))
            return;

        if (MaskTex == null || MaskColors == null)
        {
            MaskTex = new(Map.Width, Map.Height, TextureFormat.ARGB32, 0, false) { wrapMode = TextureWrapMode.Clamp };
            MaskColors = new Color[Map.Width * Map.Height];
            BackgroundAni.MaskTex = MaskTex;
            ForegroundAni.MaskTex = MaskTex;
        }

        Color32 Covered = new(0, 0, 0, 0);
        Color32 Uncovered = new(255, 255, 255, 255);

        if (pixels == default)
            pixels = new(Map.Width * Map.Height, Allocator.Persistent);

        if (ptsChanged != null)
            ptsChanged.Travel(pt => pixels[(int)pt.y * Map.Width + (int)pt.x] = fillType == GridType.Uncovered ? Uncovered : Covered);
        else
            FC.For2(Map.Width, Map.Height, (x, y) => pixels[y * Map.Width + x] = Covered);

        dirty = true;
    }

    void Redraw()
    {
        if (!dirty || pixels == default)
            return;

        MaskTex.SetPixelData(pixels, 0);
        MaskTex.Apply();
        dirty = false;
    }

    void LateUpdate()
    {
        Redraw();
    }

    private void OnDestroy()
    {
        if (pixels != default)
        {
            pixels.Dispose();
            pixels = default;
        }
    }
}
