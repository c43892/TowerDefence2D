using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;

public class FrameAni : MonoBehaviour
{
    public Sprite[] Sprites;
    public float FrameInterval;
    public bool AutoPlay;
    public bool Loop;
    
    private SpriteRenderer SR;
    private Image Img;
    private int frameIndex = 0;
    private bool playing = false;
    private float timeElepsed = 0;

    private void Start()
    {
    }

    void LoadAni(string aniName)
    {
        AsyncOperationHandle<IList<IResourceLocation>> handle = Addressables.LoadResourceLocationsAsync("myFolder");
    }

    public void StartPlay()
    {
        frameIndex = 0;
        playing = true;
        timeElepsed = 0;

        SR = GetComponent<SpriteRenderer>();
        Img = GetComponent<Image>();
    }

    private void Update()
    {
        if (!playing)
            return;

        timeElepsed += Time.deltaTime;
        while (timeElepsed >= FrameInterval)
        {
            timeElepsed -= FrameInterval;
            frameIndex++;

            if (frameIndex >= Sprites.Length)
            {
                if (Loop)
                    frameIndex %= Sprites.Length;
                else
                    frameIndex = Sprites.Length - 1;
            }

            if (SR != null)
                SR.sprite = Sprites[frameIndex];

            if (Img != null)
                Img.sprite = Sprites[frameIndex];
        }
    }
}
