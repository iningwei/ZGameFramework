using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class ArtFastTool
{
    [MenuItem("GameObject/美术工具/替换自己和所有子物体的Unity默认材质球为基础材质球")]
    static void SetDefaultMaterial()
    {
        string defaultUIMatPath = "Assets/ZGame/Art/Shader/BuildIn/My-UI-Default.mat";
        string defaultSpriteMatPath = "Assets/ZGame/Art/Shader/BuildIn/My-Sprites-Default.mat";
        string defaultParticleSystemMatPath = "Assets/ZGame/Art/Shader/BuildIn/My-Particles-Standard-Unlit.mat";

        //////string defaultStandardMatPath = "Assets/ZGame/Art/Shader/BuildIn/My-Standard.mat";
        //////string defaultStandardSpecularMatPath = "Assets/ZGame/Art/Shader/BuildIn/My-StandardSpecular.mat";

        var selectObj = Selection.activeGameObject;
        var objs = selectObj.GetComponentsInChildren(typeof(Transform), true);
        for (int i = 0; i < objs.Length; i++)
        {
            var image = objs[i].GetComponent<Image>();
            if (image != null &&
                image.material.name == "Default UI Material")
            {
                image.material = AssetDatabase.LoadAssetAtPath(defaultUIMatPath, typeof(Material)) as Material;
                Debug.Log($"reset {image.transform.GetHierarchy()}'s Image material");
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            var text = objs[i].GetComponent<Text>();
            if (text != null && text.material.name == "Default UI Material")
            {
                text.material = AssetDatabase.LoadAssetAtPath(defaultUIMatPath, typeof(Material)) as Material;
                Debug.Log($"reset {image.transform.GetHierarchy()}'s Text material");

                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            var sr = objs[i].GetComponent<SpriteRenderer>();
            if (sr != null && sr.sharedMaterial.name == "Sprites-Default")
            {
                sr.sharedMaterial = AssetDatabase.LoadAssetAtPath(defaultSpriteMatPath, typeof(Material)) as Material;

                Debug.Log($"reset {image.transform.GetHierarchy()}'s SpriteRenderer material");
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }


            var pr = objs[i].GetComponent<ParticleSystemRenderer>(); 
            if (pr != null &&( pr.sharedMaterial != null && pr.sharedMaterial.name == "Default-ParticleSystem"))
            {
                pr.sharedMaterial = AssetDatabase.LoadAssetAtPath(defaultParticleSystemMatPath, typeof(Material)) as Material;
                Debug.Log($"reset {image.transform.GetHierarchy()}'s ParticleSystemRenderer material");
                //EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                EditorUtility.SetDirty(pr);
                AssetDatabase.SaveAssets();
            }
            if (pr != null && (pr.sharedMaterial == null))
            {
                pr.sharedMaterial = AssetDatabase.LoadAssetAtPath(defaultParticleSystemMatPath, typeof(Material)) as Material;
                Debug.Log($"  {image.transform.GetHierarchy()}'s ParticleSystemRenderer material is null ,set it to default");
                //EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                EditorUtility.SetDirty(pr);
                AssetDatabase.SaveAssets();
            }


            //see ShaderReplacement.cs
            ////////TODO：MeshRenderer和SkinedMeshRenderer 应该可以合并起来，通过父类Renderer来处理
            ////////MeshRenderer
            //////var mr = objs[i].GetComponent<MeshRenderer>();
            //////if (mr != null)
            //////{
            //////    var count = mr.sharedMaterials.Length;
            //////    if (count > 0)
            //////    {
            //////        for (int j = 0; j < count; j++)
            //////        {
            //////            var tmpMat = mr.sharedMaterials[j];
            //////            if (tmpMat != null)
            //////            {
            //////                if (tmpMat.shader.name == "Standard")
            //////                {
            //////                    tmpMat.shader = Shader.Find("My/Standard");
            //////                    Debug.Log($"reset {mr.transform.GetHierarchy()}'s MeshRenderer material's shader");

            //////                }
            //////                if (tmpMat.shader.name == "Standard (Specular setup)")
            //////                {
            //////                    tmpMat.shader = Shader.Find("My/Standard (Specular setup)");
            //////                    Debug.Log($"reset {mr.transform.GetHierarchy()}'s MeshRenderer material's shader");
            //////                }
            //////            }
            //////        }

            //////    }

            //////    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            //////}



            ////////SkinnedMeshRenderer
            //////var smr = objs[i].GetComponent<SkinnedMeshRenderer>();
            //////if (smr != null)
            //////{
            //////    var count = smr.sharedMaterials.Length;
            //////    if (count > 0)
            //////    {
            //////        for (int j = 0; j < count; j++)
            //////        {
            //////            var tmpMat = smr.sharedMaterials[j];
            //////            if (tmpMat != null)
            //////            {
            //////                if (tmpMat.shader.name == "Standard")
            //////                {
            //////                    tmpMat.shader = Shader.Find("My/Standard");
            //////                    Debug.Log($"reset {smr.transform.GetHierarchy()}'s MeshRenderer material's shader");

            //////                }
            //////                if (tmpMat.shader.name == "Standard (Specular setup)")
            //////                {
            //////                    tmpMat.shader = Shader.Find("My/Standard (Specular setup)");
            //////                    Debug.Log($"reset {smr.transform.GetHierarchy()}'s MeshRenderer material's shader");
            //////                }
            //////            }
            //////        }

            //////    }

            //////    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            //////}


        }
    }

    [MenuItem("GameObject/美术工具/设置自己和所有子物体使用默认图片(如果为空)")]
    static void SetUIDefaultTex()
    {
        string defaultSpritePath = "Assets/ZGame/Art/Texture/DefaultSprite.png";//LoadAssetAtPath()'s param must start with Assets/ and with suffix.

        var selectObj = Selection.activeGameObject;
        var objs = selectObj.GetComponentsInChildren(typeof(Transform), true);
        for (int i = 0; i < objs.Length; i++)
        {
            var obj = objs[i];
            //Image
            var img = obj.GetComponent<Image>();

            //if (img.sprite.name == "UISprite" ||
            //        img.sprite.name == "Background" ||
            //        img.sprite.name == "Knob" ||
            //        img.sprite.name == "UIMask" ||
            //        img.sprite.name == "InputFieldBackground" ||
            //        img.sprite.name == "DropdownArrow" ||
            //        img.sprite.name == "Checkmark")
            //TODO:这里要改，不写死，also see,BuildCommond.cs Line:787
            if (img != null && (img.sprite == null || (img.sprite.name == "UISprite" ||
            img.sprite.name == "Background" ||
            img.sprite.name == "Knob" ||
                img.sprite.name == "UIMask" ||
            img.sprite.name == "InputFieldBackground" ||
            img.sprite.name == "DropdownArrow" ||
            img.sprite.name == "Checkmark")))
            {
                var s = AssetDatabase.LoadAssetAtPath(defaultSpritePath, typeof(Sprite)) as UnityEngine.Sprite;
                if (s == null)
                {
                    Debug.LogError("error, s is null");
                }

                img.sprite = s;
                Debug.Log($"set Image with default sprite,path:{img.transform.GetHierarchy()}");
                EditorUtility.SetDirty(img);
            }
        }
    }


    [MenuItem("GameObject/美术工具/设置自己和所有子物体的TextMeshPro字体为默认字体")]
    static void SetTextMeshProWithDefaultFontAsset()
    {
        string defaultFontAssetPath = "Assets/ArtResources/Font/arial SDF.asset";
        var selectObj = Selection.activeGameObject;
        var objs = selectObj.GetComponentsInChildren(typeof(Transform), true);
        for (int i = 0; i < objs.Length; i++)
        {
            var obj = objs[i];
            //TextMeshProUGUI
            var tm = obj.GetComponent<TextMeshProUGUI>();
            if (tm != null && (tm.font == null || tm.font.name == "LiberationSans SDF"))
            {
                TMP_FontAsset s = AssetDatabase.LoadAssetAtPath(defaultFontAssetPath, typeof(TMP_FontAsset)) as TMP_FontAsset;
                if (s == null)
                {
                    Debug.LogError("error, s is null");
                }

                tm.font = s;
                Debug.Log($"set TextMeshProUGUI with default Font,path:{tm.transform.GetHierarchy()}");
                EditorUtility.SetDirty(tm);
            }


            //TextMeshProUGUI
            var tm_input = obj.GetComponent<TMP_InputField>();
            if (tm_input != null && (tm_input.fontAsset == null || tm_input.fontAsset.name == "LiberationSans SDF"))
            {
                TMP_FontAsset s = AssetDatabase.LoadAssetAtPath(defaultFontAssetPath, typeof(TMP_FontAsset)) as TMP_FontAsset;
                if (s == null)
                {
                    Debug.LogError("error, s is null");
                }

                tm_input.fontAsset = s;
                Debug.Log($"set TMP_InputField with default Font,path:{tm_input.transform.GetHierarchy()}");
                EditorUtility.SetDirty(tm_input);
            }

        }
    }

    [MenuItem("GameObject/美术工具/一键更新字体组件")]
    static void UpdateTextComponent()
    {
        var selectObj = Selection.activeGameObject;
        var objs = selectObj.GetComponentsInChildren(typeof(Transform), true);
        for (int i = 0; i < objs.Length; i++)
        {
            var obj = objs[i];
            var txtCom = obj.GetComponent<Text>();
            if (txtCom != null)
            {

                string text = txtCom.text;
                int size = txtCom.fontSize;
                float lineSpace = txtCom.lineSpacing;
                Color c = txtCom.color;
                FontStyle fs = txtCom.fontStyle;
                HorizontalWrapMode hMode = txtCom.horizontalOverflow;
                VerticalWrapMode vMode = txtCom.verticalOverflow;
                TextAnchor ta = txtCom.alignment;
                bool fit = txtCom.resizeTextForBestFit;
                int min = txtCom.resizeTextMinSize;
                int max = txtCom.resizeTextMaxSize;

                Object.DestroyImmediate(txtCom);
                Object.DestroyImmediate(obj.GetComponent<CanvasRenderer>());
                TextMeshProUGUI tmp = obj.gameObject.AddComponent<TextMeshProUGUI>();
                tmp.text = text;
                tmp.fontSize = size;
                tmp.lineSpacing = lineSpace;
                tmp.color = c;
                tmp.fontStyle = fs == FontStyle.BoldAndItalic ? TMPro.FontStyles.Bold : (TMPro.FontStyles)fs;
                tmp.enableWordWrapping = hMode == HorizontalWrapMode.Wrap ? true : false;
                //tmp.overflowMode = vMode == VerticalWrapMode.Overflow ? TextOverflowModes.Overflow : TextOverflowModes.Truncate;
                if (vMode == VerticalWrapMode.Truncate)
                {
                    tmp.overflowMode = hMode == HorizontalWrapMode.Wrap ? TextOverflowModes.Truncate : TextOverflowModes.Overflow;
                }
                else
                {
                    tmp.overflowMode = TextOverflowModes.Overflow;
                }
                List<TextAlignmentOptions> lst = new List<TextAlignmentOptions>()
                {
                    TextAlignmentOptions.TopLeft, TextAlignmentOptions.Top, TextAlignmentOptions.TopRight,
                    TextAlignmentOptions.MidlineLeft, TextAlignmentOptions.Center, TextAlignmentOptions.MidlineRight,
                    TextAlignmentOptions.BottomLeft, TextAlignmentOptions.Bottom, TextAlignmentOptions.BaselineRight
                };
                tmp.alignment = lst[(int)ta];
                tmp.enableAutoSizing = fit;
                if (fit)
                {
                    tmp.fontSizeMin = min;
                    tmp.fontSizeMax = max;
                }
                Debug.Log($"update Text Component To TextMeshProUGUI ,path:{tmp.transform.GetHierarchy()}");
            }
        }
        SetTextMeshProWithDefaultFontAsset();
    }


    [MenuItem("GameObject/美术工具/拼图一键快乐")]
    static void OneKeyHandleArt()
    {
        SetDefaultMaterial();
        SetUIDefaultTex();
        SetTextMeshProWithDefaultFontAsset();

    }
}
