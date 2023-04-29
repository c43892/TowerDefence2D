using System.Collections;
using System.Collections.Generic;
using TowerDefance.Res;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;

public class FrameAni : MonoBehaviour
{
    public AniData Data { get; set; }

    private SpriteRenderer SR;
    private Image Img;

    private Sprite[] sprites;
    private int frameIndex = 0;
    private bool playing = false;
    private float timeElepsed = 0;

    private void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        Img = GetComponent<Image>();        
    }

    public void Play()
    {
        frameIndex = 0;
        timeElepsed = 0;

        StartCoroutine(ResManager.LoadSpritesFromGroup(Data.label, (spriteArr) =>
        {
            sprites = spriteArr;
            playing = true;
        }));
    }

    private void Update()
    {
        if (!playing)
            return;

        timeElepsed += Time.deltaTime;
        while (timeElepsed >= Data.interval)
        {
            timeElepsed -= Data.interval;
            frameIndex++;

            if (frameIndex >= sprites.Length)
            {
                if (Data.loop)
                    frameIndex %= sprites.Length;
                else
                {
                    frameIndex = sprites.Length - 1;
                    playing = false;
                }
            }

            if (SR != null)
                SR.sprite = sprites[frameIndex];

            if (Img != null)
                Img.sprite = sprites[frameIndex];
        }
    }
}
