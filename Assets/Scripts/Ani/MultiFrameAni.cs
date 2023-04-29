using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefance.Res;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;

public class MultiFrameAni : MonoBehaviour
{
    public MultiAniData Data { get; set; }

    private SpriteRenderer SR;
    private Image Img;

    private AniData CurrentAniData { get; set; }
    private Sprite[] sprites;
    private int frameIndex = 0;
    private bool playing = false;
    private float timeElepsed = 0;
    private Action OnEnded;

    private void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        Img = GetComponent<Image>();
    }

    public void Play(string aniName, Action onEnded = null)
    {
        if (playing && CurrentAniData == Data.Data[aniName])
            return;

        playing = false;
        frameIndex = 0;
        timeElepsed = 0;
        OnEnded = onEnded;

        CurrentAniData = Data.Data[aniName];
        StartCoroutine(ResManager.LoadSpritesFromGroup(CurrentAniData.label, (spriteArr) =>
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
        while (timeElepsed >= CurrentAniData.interval)
        {
            timeElepsed -= CurrentAniData.interval;
            frameIndex++;

            if (frameIndex >= sprites.Length)
            {
                if (CurrentAniData.loop)
                    frameIndex %= sprites.Length;
                else
                {
                    frameIndex = sprites.Length - 1;
                    OnEnded?.Invoke();
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
