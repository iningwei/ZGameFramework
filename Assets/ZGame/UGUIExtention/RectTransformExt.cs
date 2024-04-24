using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace ZGame.UGUIExtentio
{
    public static class RectTransformExt
    {
        public static void RebuildLayout(this RectTransform trans)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(trans);
        }
    }
}