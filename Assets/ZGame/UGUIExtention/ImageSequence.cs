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

    int count = 0;
    float durationValue = 0;

    bool isQuit = false;

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
        durationValue -= Time.deltaTime;
        if (durationValue < 0)
        {
            if (index + 1 == count)
            {
                index = 0;
                if (isLoop == false)
                {
                    isQuit = true;
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