﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class FindMissingScriptsRecursively : EditorWindow
{

    static int go_count = 0, components_count = 0, missing_count = 0;

    [MenuItem("工具/查找/FindMissingScriptsRecursively")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FindMissingScriptsRecursively));
    }



    public void OnGUI()
    {
        EditorGUILayout.Space(10);
        if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
        {
            GameObject[] go = Selection.gameObjects;
            FindInSelected(go);
        }
        EditorGUILayout.Space(10);
        if (GUILayout.Button("Find Missing Scripts in all prefabs"))
        {

            FindInAllPrefabs();
        }
    }

    private static void FindInAllPrefabs()
    {
        go_count = 0;
        components_count = 0;
        missing_count = 0;

        string[] allPrefabs = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });

        foreach (var item in allPrefabs)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(item);
            if (prefabPath.Contains("temp_for_prefab"))
            {
                continue;
            }
            var go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
            FindInGO(go);
        }

        Debug.Log("find finished!");



    }

    private static void FindInSelected(GameObject[] go)
    {

        go_count = 0;
        components_count = 0;
        missing_count = 0;
        foreach (GameObject g in go)
        {
            FindInGO(g);
        }
        Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
    }

    private static void FindInGO(GameObject g)
    {
        go_count++;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            components_count++;
            if (components[i] == null)
            {
                missing_count++;
                string s = g.name;
                Transform t = g.transform;
                while (t.parent != null)
                {
                    s = t.parent.name + "/" + s;
                    t = t.parent;
                }
                Debug.Log(s + " has an empty script attached in position: " + i, g);
            }
        }

        // Now recurse through each child GO (if there are any):  
        foreach (Transform childT in g.transform)
        {
            //Debug.Log("Searching " + childT.name  + " " );  
            FindInGO(childT.gameObject);
        }
    }
}