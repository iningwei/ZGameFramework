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


    Image image;
    public bool isLoop = true;

    //非loop状态下， 播放完后，是否设置Image对应的obj不可见
    public bool isHideWhilePlayFinishedInNoLoopStatus = false;

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
                index = 0;
                if (isLoop == false)
                {
                    isQuit = true;
                    if (isHideWhilePlayFinishedInNoLoopStatus)
                    {
                        this.gameObject.SetActive(false);
                    }
                    return;
                }
            }
            else
            {
                index++;
            }
            image.sprite = sprites[index];


            durationValue = duration;
        }
    }
}