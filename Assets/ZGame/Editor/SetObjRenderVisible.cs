using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SetObjRenderVisible : Editor
{
    [MenuItem("GameObject/SetObjAndChildsRenderStatus/Opossite")]
    static void SetObjRenderStatusOpossite()
    {
        GameObject obj = Selection.activeObject as GameObject;
        var renders = obj.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renders.Length; i++)
        {
            renders[i].enabled = !renders[i].enabled;
        }
        Debug.Log("finish SetObjAndChildsRenderStatus opossite");
        EditorUtility.SetDirty(obj);
    }

    [MenuItem("GameObject/SetObjAndChildsRenderStatus/Visible")]
    static void SetObjRenderStatusVisible()
    {
        GameObject obj = Selection.activeObject as GameObject;
        var renders = obj.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renders.Length; i++)
        {
            renders[i].enabled = true;
        }
        Debug.Log("finish SetObjAndChildsRenderStatus visible");
        EditorUtility.SetDirty(obj);
    }

    [MenuItem("GameObject/SetObjAndChildsRenderStatus/Unvisible")]
    static void SetObjRenderStatusUnvisible()
    {
        GameObject obj = Selection.activeObject as GameObject;
        var renders = obj.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renders.Length; i++)
        {
            renders[i].enabled = false;
        }
        Debug.Log("finish SetObjAndChildsRenderStatus unvisible");
        EditorUtility.SetDirty(obj);
    }
}
