﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class GetObjChildCount
{
    [MenuItem("GameObject/GetChildCount", false, 13)]
    public static void Get()
    {
        GameObject obj = Selection.activeObject as GameObject;
        Debug.LogError(obj.name + ", child count:" + obj.transform.childCount);
    }
}
