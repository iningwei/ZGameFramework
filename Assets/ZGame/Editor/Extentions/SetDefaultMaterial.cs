using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class SetDefaultMaterial : MonoBehaviour
{
    [MenuItem("GameObject/Extentions/SetMaterial-UIDefault")]
    static void SetUGUIImgDefaultMaterial()
    {
        string defaultMatPath = "Assets/ZGame/Art/Shader/BuildIn/UI-Default.mat";

        var selectObj = Selection.activeGameObject;
        var objs = selectObj.GetComponentsInChildren(typeof(Transform), true);
        for (int i = 0; i < objs.Length; i++)
        {
            var image = objs[i].GetComponent<Image>();
            if (image != null &&
                image.material.name == "Default UI Material")
            {
                image.material = AssetDatabase.LoadAssetAtPath(defaultMatPath, typeof(Material)) as Material;
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            var text = objs[i].GetComponent<Text>();
            if (text != null && text.material.name == "Default UI Material")
            {
                text.material = AssetDatabase.LoadAssetAtPath(defaultMatPath, typeof(Material)) as Material;
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }
}
