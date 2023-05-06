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
    public SpriteMask Mask;

    public BattleMap Map { get; set; }

    private Texture2D MaskTex;
    private Color[] MaskColors;

    public void UpdateMap()
    {
        if (Map == null)
            return;

        MaskTex = new Texture2D(Map.Width, Map.Height);
        MaskColors = new Color[Map.Width * Map.Height];

        FC.For2(Map.Width, Map.Height, (x, y) => MaskColors[y * Map.Width + x] = (Map[x, y] != BattleMap.GridType.Covered ? Color.white : Color.black));
        MaskTex.SetPixelData(MaskColors, 0);

        Mask.sprite = Sprite.Create(MaskTex, new Rect(0, 0, MaskTex.width, MaskTex.height), new Vector2(0.5f, 0.5f));

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
