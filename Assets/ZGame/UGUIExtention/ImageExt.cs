using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZGame.UGUIExtention
{
    public static class ImageExt
    {
        public static void Transparent(this Image img)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
        }
        public static void Opaque(this Image img)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        }
    }
}