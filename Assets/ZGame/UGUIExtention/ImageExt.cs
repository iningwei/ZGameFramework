using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZGame.Ress.AB; 

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

        public static void SetGray(this Image img)
        {
            //MaterialPropertyBlock目前还不支持UGUI，故选择通过加载mat的方式来实现gray 
            ABManager.Instance.LoadMat("My-UI-Default-Gray", (r) =>
            {
                img.material = r.GetResAsset<Material>();
#if UNITY_EDITOR
                img.material.shader = Shader.Find("My/UI/Default-Gray");
#endif
            });
        }
        public static void SetUnGray(this Image img)
        {
            ABManager.Instance.LoadMat("My-UI-Default", (r) =>
            {
                img.material = r.GetResAsset<Material>();
#if UNITY_EDITOR
                img.material.shader = Shader.Find("My/UI/Default");
#endif

            });
        }
    }
}