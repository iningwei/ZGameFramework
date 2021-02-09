using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSequence : MonoBehaviour
{
    public Sprite[] sprites;
    [SerializeField]
    float originDuration;
    [SerializeField]
    int index = 0;

    public SpriteRenderer render;

    int count = 0;
    float duration = 0;
    void Awake()
    {
        if (sprites != null && sprites.Length > 0)
        {
            count = sprites.Length;
        }

        duration = originDuration;
    }


    void Update()
    {
        if (originDuration <= 0 || count == 0)
        {
            return;
        }
        duration -= Time.deltaTime;
        if (duration < 0)
        {
            render.sprite = sprites[index];
            if (index + 1 == count)
            {
                index = 0;
            }
            else
            {
                index++;
            }

            duration = originDuration;
        }
    }
}
