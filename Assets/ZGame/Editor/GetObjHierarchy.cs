using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System;

//come from
//http://answers.unity3d.com/questions/266244/how-can-i-add-copypaste-clipboard-support-to-my-ga.html

public class ClipboardHelper
{
    private static PropertyInfo m_systemCopyBufferProperty = null;
    private static PropertyInfo GetSystemCopyBufferProperty()
    {
        if (m_systemCopyBufferProperty == null)
        {

            Type T = typeof(GUIUtility);
            m_systemCopyBufferProperty = T.GetProperty("systemCopyBuffer");// , BindingFlags.Static| BindingFlags.NonPublic//加上这个报错
            if (m_systemCopyBufferProperty == null)
                throw new Exception("Can't access internal member 'GUIUtility.systemCopyBuffer' it may have been removed / renamed");
        }
        return m_systemCopyBufferProperty;
    }
    public static string clipBoard
    {
        get
        {
            PropertyInfo P = GetSystemCopyBufferProperty();
            return (string)P.GetValue(null, null);
        }
        set
        {
            PropertyInfo P = GetSystemCopyBufferProperty();
            P.SetValue(null, value, null);
        }
    }
}


/// <summary>
/// 获得Hierarchy面板内选中物体的层级关系。并保存到剪贴板
/// </summary>
public class GetObjHierarchy : ScriptableWizard
{
    public int upperCount = 1;//追溯层级

    [MenuItem("工具/获得物体的层级")]
    static void GetHierarchy()
    {
        ScriptableWizard.DisplayWizard("获得物体层级", typeof(GetObjHierarchy), "确定", "取消");
    }



    private void OnWizardCreate()
    {
        if (isValid)
        {
            getHierarchyPath(upperCount);
        }

    }

    private void OnWizardOtherButton()
    {
        Close();
    }

    private void OnWizardUpdate()
    {
        helpString = "Hierarchy面板选中目标物体，获得其层级(点击确认后，层级会自动复制到剪贴板)";
        if (upperCount < 1)
        {
            errorString = "upperCount必须大于等于1";
            isValid = false;
        }
        else
        {
            errorString = "";
            isValid = true;
        }
    }






    static void getHierarchyPath(int parentCount)
    {
        GameObject selectObj = Selection.activeGameObject;

        if (selectObj == null)
        {
            Debug.LogError("选择一个物体");
            return;
        }
        string path = selectObj.name;
        for (int i = 0; i < parentCount; i++)
        {
            var parent = selectObj.transform.parent;

            if (parent == null)
            {
                Debug.LogError("父物体数量不匹配");
                return;
            }
            else
            {
                selectObj = parent.gameObject;
                path = selectObj.name + "/" + path;
            }
        }

        Debug.Log("path is:" + path);

        ClipboardHelper.clipBoard = path;
    }


}