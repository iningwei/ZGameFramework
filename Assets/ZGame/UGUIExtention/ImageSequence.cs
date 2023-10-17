using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageSequence : MonoBehaviour
{
    public Sprite[] sprites;
    public float duration;
    public int index = 0;


    public bool isLoop = true;
    public float lastFramePauseDuration = 0;//最后一帧动画播放后，停留的时间。一般在isLoop为true时才使用
    float lastFramePausedTime = 0f;
    public Action playFinishedCallback;//一般非loop状态才用得到

    Image image;
    int count = 0;
    float durationValue = 0;

    bool isQuit = false;
    public bool useRealtime = false;

    void OnEnable()
    {
        if (image == null)
        {
            image = this.gameObject.GetComponent<Image>();
        }
        if (sprites != null && sprites.Length > 0)
        {
            count = sprites.Length;
        }


        durationValue = duration;
        isQuit = false;
        image.sprite = sprites[index];
    }


    void Update()
    {
        if (duration <= 0 || count == 0 || isQuit)
        {
            return;
        }
        durationValue -= useRealtime ? Time.unscaledDeltaTime : Time.deltaTime; ;
        if (durationValue < 0)
        {
            if (index + 1 == count)
            {
                if (isLoop == false)
                {
                    index = 0;
                    isQuit = true;
                    if (this.playFinishedCallback != null)
                    {
                        this.playFinishedCallback();
                    }
                    return;
                }
                else
                {
                    if (lastFramePauseDuration > 0f)
                    {
                        lastFramePausedTime += useRealtime ? Time.unscaledDeltaTime : Time.deltaTime;

                        if (lastFramePausedTime > lastFramePauseDuration)
                        {
                            index = 0;
                            lastFramePausedTime = 0f;
                            durationValue = duration;
                        }
                    }
                    else
                    {
                        index = 0;
                        durationValue = duration;
                    }
                }
            }
            else
            {
                index++;
                durationValue = duration;
            }
            image.sprite = sprites[index];
        }
    }
}