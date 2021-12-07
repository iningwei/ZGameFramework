using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Texture2DExt
{
    public static Sprite Texture2DToSprite(this Texture2D tex)
    {
        int width = tex.width;
        int height = tex.height;
        Sprite s = Sprite.Create(tex, new Rect(0, 0, width, height), Vector2.zero);
        return s;
    }
}
