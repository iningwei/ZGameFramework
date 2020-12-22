﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GetSpriteRendererSize
{
    [MenuItem("ZGame/Unity2D/GetSpriteRenderSize")]
    [MenuItem("GameObject/Extentions/GetSpriteRenderSize", false, 11)]
    static void GetSize()
    {
        GameObject obj = Selection.activeObject as GameObject;
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            var extents = sr.sprite.bounds.extents;
            var lossyScale = obj.GetComponent<RectTransform>().lossyScale;
            Debug.Log(obj.name + " extents x:" + extents.x + ", y:" + extents.y + ", z:" + extents.z
                + ", globalScele:" + lossyScale
                + ", real extents x:" + extents.x * lossyScale.x + ", y:" + extents.y * lossyScale.y + ",z:" + extents.z * lossyScale.z);
        }
    }
}
