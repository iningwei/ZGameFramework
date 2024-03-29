﻿using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SearchRefrence : EditorWindow
{
    /// <summary>
    /// 查找资源被哪些prefab引用
    /// 流程--------->prefab中直接引用到的资源，可以通过查找prefab txt是否包含资源的guid
    /// -------------->若prefab是通过mat间接引用到的资源：查找mat txt中是否有目标guid，然后再在prefab txt中找mat的guid。最终获得目标prefab
    /// </summary>
    [MenuItem("Assets/SearchAssetRefrenceByPrefab")]
    [MenuItem("工具/查找/SearchAssetRefrenceByPrefab")]
    static void DoSearchRefrence()
    {
        SearchRefrence window = (SearchRefrence)EditorWindow.GetWindow(typeof(SearchRefrence), false, "Searching", true);
        window.Show();
        Object[] assets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        if (assets.Length > 0)
        {
            searchObject = assets[0];
            doSearch();
        }
    }

    private static Object searchObject;
    private static List<Object> result = new List<Object>();
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        searchObject = EditorGUILayout.ObjectField(searchObject, typeof(Object), true, GUILayout.Width(200));
        if (GUILayout.Button("Search", GUILayout.Width(100)))
        {
            doSearch();
        }
        EditorGUILayout.EndHorizontal();

        //显示结果
        showResult();
    }

    static void doSearch()
    {
        result.Clear();

        if (searchObject == null)
            return;

        string assetPath = AssetDatabase.GetAssetPath(searchObject);
        var matRefs = searchMatRef(assetPath);
        if (matRefs.Count == 0)
        {
            searchPrefabRef(assetPath);
        }
        else
        {
            for (int j = 0; j < matRefs.Count; j++)
            {
                searchPrefabRef(AssetDatabase.GetAssetPath(matRefs[j]));
            }
        }


        EditorUtility.ClearProgressBar();
    }
    void showResult()
    {
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < result.Count; i++)
        {
            EditorGUILayout.ObjectField(result[i], typeof(Object), true, GUILayout.Width(300));
        }
        EditorGUILayout.EndHorizontal();
    }
    static List<Material> searchMatRef(string assetPath)
    {
        string assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
        //只检查material
        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });

        int length = guids.Length;
        Debug.Log("SearchMatRef 材质球：" + length + ", 目标guid:" + assetGuid);
        List<Material> result = new List<Material>();
        for (int i = 0; i < length; i++)
        {
            string filePath = AssetDatabase.GUIDToAssetPath(guids[i]);
            EditorUtility.DisplayCancelableProgressBar("Checking Materials", filePath, i / (float)length);

            //检查是否包含guid
            string content = File.ReadAllText(filePath);
            if (content.Contains(assetGuid))
            {
                Object fileObject = AssetDatabase.LoadAssetAtPath(filePath, typeof(Object));
                result.Add(fileObject as Material);
            }
        }
        return result;
    }

    static void searchPrefabRef(string assetPath)
    {
        string assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
        //只检查prefab
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });

        int length = guids.Length;
        Debug.Log("SearchPrefabRef 预制件个数：" + length + ", 目标guid:" + assetGuid);
        for (int i = 0; i < length; i++)
        {
            string filePath = AssetDatabase.GUIDToAssetPath(guids[i]);
            EditorUtility.DisplayCancelableProgressBar("Checking Prefabs", filePath, i / (float)length);

            //检查是否包含guid
            string content = File.ReadAllText(filePath);
            if (content.Contains(assetGuid))
            {
                Object fileObject = AssetDatabase.LoadAssetAtPath(filePath, typeof(Object));
                result.Add(fileObject);
            }
        }
    }

}
