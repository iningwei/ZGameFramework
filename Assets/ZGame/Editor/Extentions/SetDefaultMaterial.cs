using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class SetDefaultMaterial : MonoBehaviour
{
    [MenuItem("GameObject/Extentions/SetMaterial-With-Self-Default")]
    static void SetUGUIImgDefaultMaterial()
    {
        string defaultUIMatPath = "Assets/ZGame/Art/Shader/BuildIn/My-UI-Default.mat";
        string defaultSpriteMatPath = "Assets/ZGame/Art/Shader/BuildIn/My-Sprites-Default.mat";
        string defaultParticleSystemMatPath = "Assets/ZGame/Art/Shader/BuildIn/My-ParticleSystem-Default.mat";

        var selectObj = Selection.activeGameObject;
        var objs = selectObj.GetComponentsInChildren(typeof(Transform), true);
        for (int i = 0; i < objs.Length; i++)
        {
            var image = objs[i].GetComponent<Image>();
            if (image != null &&
                image.material.name == "Default UI Material")
            {
                image.material = AssetDatabase.LoadAssetAtPath(defaultUIMatPath, typeof(Material)) as Material;
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            var text = objs[i].GetComponent<Text>();
            if (text != null && text.material.name == "Default UI Material")
            {
                text.material = AssetDatabase.LoadAssetAtPath(defaultUIMatPath, typeof(Material)) as Material;
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            var sr = objs[i].GetComponent<SpriteRenderer>();
            if (sr != null && sr.sharedMaterial.name == "Sprites-Default")
            {
                sr.sharedMaterial = AssetDatabase.LoadAssetAtPath(defaultSpriteMatPath, typeof(Material)) as Material;
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }


            var pr = objs[i].GetComponent<ParticleSystemRenderer>();
            //TODO:当pr enable的时候，若pr.sharedMaterial若为null，则弹出报错信息
            //或者提供工具检测出这种情况
            if (pr != null && pr.sharedMaterial != null && pr.sharedMaterial.name == "Default-ParticleSystem")
            {
                pr.sharedMaterial = AssetDatabase.LoadAssetAtPath(defaultParticleSystemMatPath, typeof(Material)) as Material;
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }
}
