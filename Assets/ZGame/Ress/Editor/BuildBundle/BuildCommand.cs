using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ZGame.Res;

namespace ZGame.RessEditor
{
    public class BuildCommand
    {
        //[MenuItem("工具/测试")]
        //public static void Test()
        //{
        //    GameObject selectObj = Selection.activeGameObject;
        //    var mat = selectObj.GetComponent<Image>().material;

        //    if (mat != null)
        //    {
        //        Debug.Log("aaaa:" + selectObj.GetComponent<Image>().sprite.name);
        //        Debug.Log("zz:" + mat.name);
        //        Debug.Log("xx:" + mat.shader.name);
        //    }


        //    Debug.LogError(selectObj.name);
        //}

        [MenuItem("Assets/对选择项打AB包")]
        public static void Build()
        {
            BuildConfig.Init();
            if (!CheckCommonRes())
            {
                return;
            }

            Object[] assets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets) as Object[];
            for (int i = 0; i < assets.Length; i++)
            {
                var path = AssetDatabase.GetAssetOrScenePath(assets[i]);
                BuildConfig.getBuildFunc(path, assets[i])(assets[i]);
            }

            // 完成后要删除一些无用的文件
            var files = Directory.GetFileSystemEntries(BuildConfig.outputPath);
            foreach (var v in files)
            {
                if (!v.EndsWith(IOTools.abSuffix))
                {
                    File.Delete(v);
                }
            }

            Debug.Log("打包   bundle完毕");

        }



        static AssetBundleBuild commonResMap;
        static List<Shader> shaders;
        //对基础资源进行检查
        public static bool CheckCommonRes()
        {
            commonResMap = new AssetBundleBuild();
            commonResMap.assetBundleName = "common";
            commonResMap.assetBundleVariant = IOTools.abSuffixWithoutPoint;

            List<string> assetPaths = new List<string>();

            //添加shader
            string shaderListPath = "Assets/ArtResources/ShaderList/ShaderList.prefab";
            var shaderList = AssetDatabase.LoadAssetAtPath(shaderListPath, typeof(GameObject)) as GameObject;
            if (shaderList == null)
            {
                Debug.LogError("error," + shaderListPath + "出错！");
                return false;
            }

            shaders = new List<Shader>(shaderList.GetComponent<ShaderList>().Shaders);

            for (int i = 0; i < shaders.Count; i++)
            {
                if (shaders[i] == null)
                {
                    Debug.LogError("shaderlist have null shader");
                    return false;
                }

                string path = AssetDatabase.GetAssetPath(shaders[i]);
                if (path == null || !path.Contains("Assets"))
                {
                    Debug.LogError("该shader不在工程内..." + path == null ? shaders[i].name : path);
                    return false;
                }

                assetPaths.Add(path);

            }


            //添加字体资源
            //TODO:字体暂时不通过类似Shader的方式进行添加，统一在这里把需要的字体打进bundle
            //特别要注意，如果新用到了字体，需要在这里添加,因为暂时没有提供类似shader的 字体检测工具           
            List<string> fontPathList = new List<string>();
            fontPathList.Add("Assets/ArtResources/Font/Poppins-Medium.ttf");
            fontPathList.Add("Assets/ArtResources/Font/impact.ttf");
            for (int i = 0; i < fontPathList.Count; i++)
            {
                if (!File.Exists(Application.dataPath + fontPathList[i].Replace("Assets", "")))
                {
                    Debug.LogError("Error, specific font not exist：" + fontPathList[i]);
                    return false;
                }
                else
                {
                    assetPaths.Add(fontPathList[i]);
                }
            }


            commonResMap.assetNames = assetPaths.ToArray();
            return true;
        }

        public static AssetBundleBuild GetCommonMap()
        {
            return commonResMap;
        }


        /// <summary>
        /// 对Renderer组件进行合法性检测
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool CheckValidOfRenderer(GameObject target)
        {
            string targetHierarchyPath = getHierarchyPath(target.transform);
            Debug.Log("CheckValidOfRenderer:" + targetHierarchyPath);
            var renderers = target.GetComponents<Renderer>();//一个obj上可能会挂载多个renderer组件。比如挂载LineRenderer和SpriteRenderer


            if (renderers != null && renderers.Length > 0)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    var render = renderers[i];
                    var mats = render.sharedMaterials;
                    for (int j = 0; j < mats.Length; j++)
                    {
                        var mat = mats[j];
                        if (mat != null)
                        {

                            if (mat.name == "Default-ParticleSystem")
                            {
                                Debug.LogError("使用了默认材质球， mat.name:" + mat.name + ", objPath:" + render.transform.Hierarchy());
                                return false;
                            }
                            //TODO:
                            if (isShaderInList(mat.shader.name, getHierarchyPath(target.transform)) == false)
                            {
                                return false;
                            }
                        }
                    }
                }

            }

            return true;
        }



        /// <summary>
        /// 对Unity2D的组件进行合法性检测
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool CheckValidOfU2D(GameObject target)
        {
            string targetHierarchyPath = getHierarchyPath(target.transform);
            Debug.Log("CheckValidOfU2D:" + targetHierarchyPath);

            SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                if (sr.sharedMaterial != null && !isValidMatOrShaderOfU2D(target, sr.sharedMaterial))
                {
                    return false;
                }



            }
            return true;
        }




        /// <summary>
        /// 是否使用了默认材质球或者shader
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        static bool isValidMatOrShaderOfU2D(GameObject target, Material mat)
        {
            if (mat.name == "Sprites-Default")//使用了Unity2D的默认材质球
            {
                Debug.LogError("使用了U2D默认材质球，" + target.name);

                return false;
            }
            else
            {
                if (mat.shader == null)
                {
                    Debug.LogError("材质球无shader," + target.name);
                    return false;
                }
                if (mat.shader.name == "Sprites/Default")//使用了U2D的默认shader
                {
                    Debug.LogError("材质球使用了U2D默认shader," + target.name);
                    return false;
                }


                if (isShaderInList(mat.shader.name, getHierarchyPath(target.transform)) == false)
                {
                    return false;
                }
            }

            return true;
        }



        static bool isShaderInList(string shaderName, string objHirarchy)
        {
            if (shaders.Find(a => a.name == shaderName) != null)
            {
                return true;
            }

            //////if (shaderName == "GUI/Text Shader")//TODO:TextMesh组件使用的MeshRender
            //////{
            //////    return true;
            //////}
            ///

            if (shaderName == "Spine/Skeleton")//TODO:暂时spine骨骼还没有做shader、图片等拆分
            {
                return true;
            }
            Debug.LogError("使用的shader没有加入shaderList中，shader:" + shaderName + ",      objPath:" + objHirarchy);
            return false;
        }



        /// <summary>
        /// 对UGUI 的合法性进行检测
        /// </summary>
        /// <param name="target"></param>  
        /// <returns></returns>
        public static bool CheckValidOfUGUI(GameObject target)
        {
            string targetHierarchyPath = getHierarchyPath(target.transform);
            Debug.Log("CheckValidOfUGUI：" + targetHierarchyPath);


            Image img = target.GetComponent<Image>();
            if (img != null)//图片
            {

                if (!isValidImageOfUGUI(img))
                {
                    return false;
                }


                if (!isValidMatOrShaderOfUGUI(target, img.material))
                {

                    return false;
                }
            }

            Text text = target.GetComponent<Text>();
            if (text != null)//文字
            {
                if (!isValidMatOrShaderOfUGUI(target, text.material))
                {
                    return false;
                }

                if (!isValidTextOfUGUI(text))
                {
                    return false;
                }
            }


            return true;
        }


        static bool isValidTextOfUGUI(Text text)
        {
            if (text.font == null)
            {
                Debug.LogError("未指定字体，" + text.gameObject);
                return false;
            }
            else
            {
                if (text.font.name == "Arial")
                {
                    Debug.LogError("使用了默认Arial字体，" + text.gameObject);
                    return false;
                }
            }
            return true;
        }

        static bool isValidImageOfUGUI(Image img)
        {
            if (img.sprite == null)
            {
                //允许Image不使用贴图
            }
            else
            {
                //TODO:后续需要限定，sprite名字不允许使用这几个
                if (img.sprite.name == "UISprite" ||
                    img.sprite.name == "Background" ||
                    img.sprite.name == "Knob" ||
                    img.sprite.name == "UIMask" ||
                    img.sprite.name == "InputFieldBackground" ||
                    img.sprite.name == "DropdownArrow" ||
                    img.sprite.name == "Checkmark")
                {
                    Debug.LogError("使用了默认贴图," + img.name + ", 贴图名：" + img.sprite.name);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 是否使用了默认材质球或者shader
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        static bool isValidMatOrShaderOfUGUI(GameObject target, Material mat)
        {

            if (mat.name == "Default UI Material")//使用了UGUI的默认材质球
            {
                Debug.LogError("used ugui's default mat: " + target.name);
                return false;
            }
            else
            {
                if (mat.shader == null)
                {
                    Debug.LogError("mat have no shader: " + target.name);
                    return false;
                }
                //if (mat.shader.name == "UI/Default")//使用了UGUI的默认shader
                //{
                //    Debug.LogError("材质球使用了ugui默认shader," + target.name);
                //    return false;
                //}


                if (isShaderInList(mat.shader.name, getHierarchyPath(target.transform)) == false)
                {
                    return false;
                }
            }

            return true;
        }

        static string getHierarchyPath(Transform target)
        {
            string path = target.name;
            Transform sun = target;
            while (sun.transform.parent != null)
            {
                sun = sun.transform.parent;
                path = sun.name + "/" + path;
            }

            return path;
        }
    }
}