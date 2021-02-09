using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System;



///////// <summary>
///////// 获得Hierarchy面板内选中物体的层级关系。并保存到剪贴板
///////// </summary>
//////public class GetObjHierarchy : ScriptableWizard
//////{
//////    public int upperCount = 1;//追溯层级

//////    [MenuItem("工具/获得物体的层级")]
//////    static void GetHierarchy()
//////    {
//////        ScriptableWizard.DisplayWizard("获得物体层级", typeof(GetObjHierarchy), "确定", "取消");
//////    }

//////    private void OnWizardCreate()
//////    {
//////        if (isValid)
//////        {
//////            getHierarchyPath(upperCount);
//////        }
//////    }

//////    private void OnWizardOtherButton()
//////    {
//////        Close();
//////    }

//////    private void OnWizardUpdate()
//////    {
//////        helpString = "Hierarchy面板选中目标物体，获得其层级(点击确认后，层级会自动复制到剪贴板)";
//////        if (upperCount < 1)
//////        {
//////            errorString = "upperCount必须大于等于1";
//////            isValid = false;
//////        }
//////        else
//////        {
//////            errorString = "";
//////            isValid = true;
//////        }
//////    }

//////    static void getHierarchyPath(int parentCount)
//////    {
//////        GameObject selectObj = Selection.activeGameObject;

//////        if (selectObj == null)
//////        {
//////            Debug.LogError("选择一个物体");
//////            return;
//////        }
//////        string path = selectObj.name;
//////        for (int i = 0; i < parentCount; i++)
//////        {
//////            var parent = selectObj.transform.parent;

//////            if (parent == null)
//////            {
//////                Debug.LogError("父物体数量不匹配");
//////                return;
//////            }
//////            else
//////            {
//////                selectObj = parent.gameObject;
//////                path = selectObj.name + "/" + path;
//////            }
//////        }

//////        Debug.Log("path is:" + path);

//////        ClipboardHelper.clipBoard = path;
//////    }
//////}



public class GetObjHierarchy
{

    [MenuItem("GameObject/GetObjHierarchy", false, 15)]
    public static void Get()
    {
        GameObject obj = Selection.activeObject as GameObject;
        string path = obj.GetPathInHierarchy();
        ClipboardHelper.clipBoard = path;
        Debug.Log("path:" + path + ", has copied to you clipboard!");


    }
}