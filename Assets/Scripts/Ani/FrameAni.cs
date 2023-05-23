using System.Collections;
using System.Collections.Generic;
using GalPanic.Res;
using Swift;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class FrameAni : MonoBehaviour
{
    public AniData Data { get; set; }
    public Texture2D MaskTex;

    private SpriteRenderer sr;

    private Sprite[] sprites;
    private int frameIndex = 0;
    private bool playing = false;
    private float timeElepsed = 0;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Play(bool restart = false)
    {
        if (!playing || restart)
        {
            frameIndex = 0;
            timeElepsed = 0;
            playing = false;
        }

        if (!playing)
        {
            playing = true;
            transform.localScale = Data.scale * Vector3.one;
            StartCoroutine(ResManager.LoadSpritesFromGroup(Data.label, (spriteArr) => sprites = spriteArr));
        }
    }

    private void Update()
    {
        if (!playing || sprites == null /* may not loaded yet */)
            return;

        timeElepsed += Time.deltaTime;
        while (timeElepsed >= Data.interval)
        {
            timeElepsed -= Data.interval;
            frameIndex++;
        }

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

        sr.sprite = sprites[frameIndex];
        sr.material.SetTexture("_MaskTex", MaskTex);
    }
}
