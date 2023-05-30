using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ZGame.Res;
using ZGame.Ress.AB.Holder;
using ZGame.Ress.Info;

namespace ZGame.RessEditor
{
    public class BuildCommand
    {

        [MenuItem("工具/打包/打ab包/一键打所有资源ab包")]
        public static void OneKeyBuild()
        {
            if (EditorUtility.DisplayDialog("警告", "确定要一键打所有资源ab包吗？", "OK", "Cancel"))
            {
                BuildConfig.Init();
                if (!CheckCommonRes())
                {
                    return;
                }

                //打音效
                var audioGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/ArtResources/Audio" });
                for (int i = 0; i < audioGuids.Length; i++)
                {
                    string guid = audioGuids[i];
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    AudioClip asset = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                    BuildConfig.getBuildFunc(path, asset)(asset);
                }


                //打预制件
                var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
                for (int i = 0; i < prefabGuids.Length; i++)
                {
                    string guid = prefabGuids[i];
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (path.Contains("temp_for_prefab"))
                    {
                        continue;
                    }


                    string allPath = Application.dataPath + path.Replace("Assets", "");
                    string fileName = Path.GetFileNameWithoutExtension(allPath);

                    //过滤出一些暂时不需要打ab的
                    if (fileName == "role_hud_4vboss"

                        || fileName == "scene_0009"

                      || fileName == "TaskHintWindow"
                      || fileName == "TaskWindow")
                    {
                        continue;
                    }
                    GameObject asset = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                    if (asset != null)
                    {
                        var holder = asset.GetComponent<RootCompInfoHolder>();
                        if (holder != null)
                        {
                            BuildConfig.getBuildFunc(path, asset)(asset);
                        }
                    }

                }

                //打图片（虽然预制件对图片有进行引用，但仍有图集或者图片没有通过预制件直接引用）
                var spriteGuids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/ArtResources/Sprite" });
                for (int i = 0; i < spriteGuids.Length; i++)
                {
                    string guid = spriteGuids[i];
                    string path = AssetDatabase.GUIDToAssetPath(guid);

                    string allPath = Application.dataPath + path.Replace("Assets", "");
                    string fileName = Path.GetFileNameWithoutExtension(allPath);

                    //Debug.Log("path:" + path);
                    //过滤掉一些当前版本没用到的图片资源
                    if (fileName == "textmeshpro-imgs"
                        || fileName == "battlepetshow_2"

                        || fileName == "task")
                    {
                        continue;
                    }
                    Texture2D asset = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    BuildConfig.getBuildFunc(path, asset)(asset);
                }


                var textureGuids = AssetDatabase.FindAssets("t:Texture", new[] { "Assets/ArtResources/Texture/BattleBoxingShown" });
                for (int i = 0; i < textureGuids.Length; i++)
                {
                    string guid = textureGuids[i];
                    string path = AssetDatabase.GUIDToAssetPath(guid);

                    string allPath = Application.dataPath + path.Replace("Assets", "");
                    string fileName = Path.GetFileNameWithoutExtension(allPath);

                    if (fileName == "battle_show_bast"
                        || fileName == "battle_show_corgi"
                       )
                    {
                        continue;
                    }
                    Texture asset = AssetDatabase.LoadAssetAtPath<Texture>(path);
                    BuildConfig.getBuildFunc(path, asset)(asset);
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

                Debug.Log("一键打所有资源ab包    完毕");
            }
            else
            {
                Debug.LogError("取消打包");
            }

        }



        [MenuItem("Assets/对选择项打AB包")]
        public static void Build()
        {
            BuildConfig.Init();
            if (!CheckCommonRes())
            {
                return;
            }

            UnityEngine.Object[] assets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets) as UnityEngine.Object[];
            for (int i = 0; i < assets.Length; i++)
            {
                var path = AssetDatabase.GetAssetOrScenePath(assets[i]);
                BuildConfig.getBuildFunc(path, assets[i])(assets[i]);
            }

            DeleteAfterBuildAB();

            Debug.Log("finished bundle build!");
        }


        [MenuItem("Assets/对选择文件夹内所有预制件添加RootCompInfoHolder，并设置类型为OtherPrefab")]
        public static void SetPrefabRootCompInfoHolder()
        {
            var allAssetPathList = GetAllTargetFilesFormSelectFolder("*.prefab");
            Debug.Log("count:" + allAssetPathList.Count);

            if (allAssetPathList.Count > 0)
            {
                foreach (var path in allAssetPathList)
                {
                    string assetPath = path.Substring(path.IndexOf("Assets"));
                    var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
                    var holder = asset.GetOrAddComponent<RootCompInfoHolder>();
                    holder.abType = Ress.AB.ABType.OtherPrefab;

                    EditorUtility.SetDirty(asset);
                    AssetDatabase.SaveAssets(); //保存
                }
            }

        }

        [MenuItem("Assets/对选择文件夹内所有预制件打AB包(类型为OtherPrefab)")]
        public static void BuildPrefabAB()
        {
            BuildConfig.Init();
            if (!CheckCommonRes())
            {
                return;
            }

            var allAssetPathList = GetAllTargetFilesFormSelectFolder("*.prefab");
            foreach (var path in allAssetPathList)
            {
                string assetPath = path.Substring(path.IndexOf("Assets"));
                var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
                new BuildOtherPrefab().Build(asset);
            }

            DeleteAfterBuildAB();
        }


        [MenuItem("Assets/对选择文件夹内所有光照贴图打ab")]
        public static void BuildLightMap()
        {
            BuildConfig.Init();
            if (!CheckCommonRes())
            {
                return;
            }

            var allAssetPathList = GetAllTargetFilesFormSelectFolder("*.hdr");
            foreach (var path in allAssetPathList)
            {
                string assetPath = path.Substring(path.IndexOf("Assets"));
                var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture));
                new BuildTexture().Build(asset);
            }

            DeleteAfterBuildAB();
        }

        [MenuItem("Assets/对选择文件夹内所有窗体打ab")]
        public static void BuildWindow()
        {
            BuildConfig.Init();
            if (!CheckCommonRes())
            {
                return;
            }

            var allAssetPathList = GetAllTargetFilesFormSelectFolder("*.prefab");
            foreach (var path in allAssetPathList)
            {
                if (path.EndsWith("Window.prefab") == false)
                {
                    continue;
                }
                string assetPath = path.Substring(path.IndexOf("Assets"));
                var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
                new BuildWindow().Build(asset);
            }

            DeleteAfterBuildAB();
        }


        static List<string> GetAllTargetFilesFormSelectFolder(string _extension)
        {
            // 获取所有选中 文件、文件夹的 GUID
            string[] guids = Selection.assetGUIDs;
            List<string> allAssetPathList = new List<string>();
            foreach (var guid in guids)
            {
                // 将 GUID 转换为 路径
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                // 判断是否文件夹
                if (Directory.Exists(assetPath))
                {
                    searchDirectory(assetPath, _extension, ref allAssetPathList);
                }
            }
            return allAssetPathList;
        }

        static void searchDirectory(string directory, string _extension, ref List<string> outputList)
        {
            DirectoryInfo dInfo = new DirectoryInfo(directory);
            // 获取 文件夹以及子文件加中所有扩展名为  _extension 的文件
            FileInfo[] fileInfoArr = dInfo.GetFiles(_extension, SearchOption.AllDirectories);
            for (int i = 0; i < fileInfoArr.Length; ++i)
            {
                string fullName = fileInfoArr[i].FullName;
                Debug.Log(fullName);
                outputList.Add(fullName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool BuildLightMapTexAB(string path)
        {
            var asset = AssetDatabase.LoadAssetAtPath(path, typeof(Texture));
            return new BuildTexture().Build(asset);
        }

        public static bool BuildBytes(string path)
        {
            Debug.Log("----build bytes path:" + path);
            string assetPath = path.Substring(path.IndexOf("Assets"));
            Debug.Log("----build bytes assetPath:" + assetPath);
            var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset));
            return new BuildByte().Build(asset);
        }

        //由于场景打AB会出现rootcompinfo信息丢失，故提供一种分步打ab的方式。
        [MenuItem("Assets/对场景打AB/step1-->收集场景RootCompInfo")]
        public static void CollectSceneRootCompInfo()
        {
            BuildConfig.Init();
            if (!CheckCommonRes())
            {
                return;
            }


            UnityEngine.Object[] assets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets) as UnityEngine.Object[];
            if (assets != null)
            {
                var path = AssetDatabase.GetAssetOrScenePath(assets[0]);
                if (path.Contains(".unity") == false)
                {
                    Debug.LogError("error,your select is not a scene");
                    return;
                }
            }
            if (assets.Length > 1)
            {
                Debug.LogError("Only support select one scene!");
                return;
            }

            new BuildScene().GetSceneRootCompInfo(assets[0]);



        }
        [MenuItem("Assets/对场景打AB/step2-->打AB")]
        public static void BuildSceneAB()
        {
            BuildConfig.Init();
            if (!CheckCommonRes())
            {
                return;
            }


            UnityEngine.Object[] assets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets) as UnityEngine.Object[];
            if (assets != null)
            {
                var path = AssetDatabase.GetAssetOrScenePath(assets[0]);
                if (path.Contains(".unity") == false)
                {
                    Debug.LogError("error,your select is not a scene");
                    return;
                }
            }
            if (assets.Length > 1)
            {
                Debug.LogError("Only support select one scene!");
                return;
            }

            new BuildScene().BuildSceneAB(assets[0]);

            DeleteAfterBuildAB();
        }


        [MenuItem("工具/打包/打ab包/删除无用文件")]
        public static void DeleteAfterBuildAB()
        {
            // 完成后要删除一些无用的文件（meta文件）
            var files = Directory.GetFileSystemEntries(BuildConfig.outputPath);
            foreach (var v in files)
            {
                if (!v.EndsWith(IOTools.abSuffix))
                {
                    File.Delete(v);
                }
            }
        }


        static AssetBundleBuild commonResMap;
        static List<Shader> shaders;
        //check common res
        public static bool CheckCommonRes()
        {
            commonResMap = new AssetBundleBuild();
            commonResMap.assetBundleName = "common";
            commonResMap.assetBundleVariant = IOTools.abSuffixWithoutPoint;

            List<string> assetPaths = new List<string>();

            //add shader
            string shaderListPath = "Assets/ArtResources/ShaderList/ShaderList.prefab";
            var shaderList = AssetDatabase.LoadAssetAtPath(shaderListPath, typeof(GameObject)) as GameObject;
            if (shaderList == null)
            {
                Debug.LogError("error," + shaderListPath);
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
                if (path == null)
                {
                    Debug.LogError("该shader不在工程内,shader name:" + shaders[i].name);
                    return false;
                }
                if (!path.Contains("Assets"))
                {
                    Debug.LogWarning("warnning,该shader不在Assets目录下，path:" + path);
                }

                assetPaths.Add(path);

            }

            //添加shader 变体资源
            string shaderVariantsPath = "Assets/ArtResources/ShaderList/ShaderVariants.shadervariants";
            if (File.Exists(Application.dataPath + shaderVariantsPath.Replace("Assets", "")))
            {
                assetPaths.Add(shaderVariantsPath);
            }

            //添加字体资源
            //TODO:字体暂时不通过类似Shader的方式进行添加，统一在这里把需要的字体打进bundle
            //特别要注意，如果新用到了字体，需要在这里添加,因为暂时没有提供类似shader的 字体检测工具           
            List<string> fontPathList = new List<string>();
            //////fontPathList.Add("Assets/ArtResources/Font/Poppins-Medium.ttf");
            //////fontPathList.Add("Assets/ArtResources/Font/impact.ttf");


            //fontPathList.Add("Assets/ArtResources/Font/fzltthjt.ttf");
            fontPathList.Add("Assets/ArtResources/Font/arial.ttf");
            fontPathList.Add("Assets/ArtResources/Font/PangMenZhengDao2.0.ttf");
            fontPathList.Add("Assets/ArtResources/Font/SourceHanSansSC-Medium.otf");


            //Textmesh pro sdf 文件
            fontPathList.Add("Assets/ArtResources/Font/arial SDF.asset");
            fontPathList.Add("Assets/ArtResources/Font/PangMenZhengDao2.0 SDF.asset");
            fontPathList.Add("Assets/ArtResources/Font/SourceHanSansSC-Medium SDF.asset");


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

                            //ParticlesUnlit是urp粒子系统的默认材质球
                            if (mat.name == "Default-ParticleSystem" || mat.name == "ParticlesUnlit")
                            {
                                Debug.LogError("使用了默认材质球， mat.name:" + mat.name + ", objPath:" + render.transform.GetHierarchy());
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

        public static void FillRootCompInfoHolder(GameObject obj)
        {
            CompInfoHolder holder = obj.GetComponent<CompInfoHolder>();
            if (holder == null)
            {
                Debug.LogError("error, " + obj.GetHierarchy() + ", have no CompInfoHolder Component");
                return;
            }
            fillCompInfoHolder(obj, holder);
        }

        public static void FillDynamicCompInfoHolder(GameObject obj)
        {
            List<GameObject> childs = obj.GetChilds(false, true);
            for (int i = 0; i < childs.Count; i++)
            {
                GameObject child = childs[i];
                var dynamicInfoHolder = child.GetComponent<DynamicCompInfoHolder>();
                if (dynamicInfoHolder != null)
                {
                    fillCompInfoHolder(child, dynamicInfoHolder);
                }
            }
        }

        static void fillCompInfoHolder(GameObject obj, CompInfoHolder infoHolder)
        {
            //////infoHolder.buildInCompImageInfos = GenericExt.ParseList<BuildInCompImageInfo>(new BuildInCompImageCollection().GetCompInfo(obj));

            //////infoHolder.buildInCompSpriteRendererInfos = GenericExt.ParseList<BuildInCompSpriteRendererInfo>(new BuildInCompSpriteRendererCollection().GetCompInfo(obj));
            //////infoHolder.buildInCompTextInfos = GenericExt.ParseList<BuildInCompTextInfo>(new BuildInCompTextCollection().GetCompInfo(obj));
            //////infoHolder.buildInCompRendererInfos = GenericExt.ParseList<BuildInCompRendererInfo>(new BuildInCompRendererCollection().GetCompInfo(obj));

            //////infoHolder.extCompImageSequenceInfos = GenericExt.ParseList<ExtCompImageSequenceInfo>(new ExtCompImageSequenceCollection().GetCompInfo(obj));
            //////infoHolder.extCompSpriteSequenceInfos = GenericExt.ParseList<ExtCompSpriteSequenceInfo>(new ExtCompSpriteSequenceCollection().GetCompInfo(obj));
            //////infoHolder.extCompMaterialTextureSequenceInfos = GenericExt.ParseList<ExtCompMaterialTextureSequenceInfo>(new ExtCompMaterialTextureSequenceCollection().GetCompInfo(obj));

            //////infoHolder.extCompSwapSpriteInfos = GenericExt.ParseList<ExtCompSwapSpriteInfo>(new ExtCompSwapSpriteCollection().GetCompInfo(obj));
            //////infoHolder.extCompSwitch2ButtonInfos = GenericExt.ParseList<ExtCompSwitch2ButtonInfo>(new ExtCompSwitch2ButtonCollection().GetCompInfo(obj));




            if (infoHolder is RootCompInfoHolder)
            {

                var abType = (infoHolder as RootCompInfoHolder).abType;
                if (abType == Ress.AB.ABType.Scene)
                {
                    //////Debug.LogError("LightmapInfoCollection@");

                    infoHolder.lightmapInfo = new LightmapInfoCollection().GetCompInfo(obj);
                }
            }


            Type t = infoHolder.GetType();
            var fis = t.GetFields();
            foreach (var fi in fis)
            {
                Type ft = fi.FieldType;


                if (ft.IsPublic && ft.IsGenericType)
                {
                    Type[] genericAs = ft.GetGenericArguments();//get fields's generic params
                    object[] atts = fi.GetCustomAttributes(typeof(CompInfoRefCollectionAttribute), true);
                    if (atts != null && atts.Length > 0)
                    {
                        string collectionClassName = ((CompInfoRefCollectionAttribute)atts[0]).refCollectionClassName;

                        Type collectionT = Type.GetType(collectionClassName);

                        object target = Activator.CreateInstance(collectionT);
                        MethodInfo mi = collectionT.GetMethod("GetCompInfo");
                        var compInfoResult = mi.Invoke(target, new object[] { obj });
                        List<CompInfo> ciList = null;
                        if (compInfoResult != null)
                        {
                            ciList = compInfoResult as List<CompInfo>;
                        }


                        Type genericExtType = typeof(GenericExt); //or  Type.GetType("GenericExt");
                        MethodInfo parseListMI = genericExtType.GetMethod("ParseList");
                        MethodInfo info = parseListMI.MakeGenericMethod(genericAs[0]);//对普通方法封装为对应类型的泛型方法，再使用Invoke进行调用
                        var result = info.Invoke(genericExtType, new object[] { ciList });

                        fi.SetValue(infoHolder, result);
                    }

                }

            }
        }

        public static List<AssetBundleBuild> GetGameObjectAssetBundleBuildMap(GameObject obj)
        {
            List<AssetBundleBuild> texMap = new List<AssetBundleBuild>();

            /*
            //////BuildInCompImageCollection buildInCompImageCollection = new BuildInCompImageCollection();
            //////texMap.AddRange(buildInCompImageCollection.GetResMap(obj));

            //////BuildInCompSpriteRendererCollection buildInCompSpriteRendererCollection = new BuildInCompSpriteRendererCollection();
            //////texMap.AddRange(buildInCompSpriteRendererCollection.GetResMap(obj));

            //////BuildInCompRendererCollection buildInCompRendererCollection = new BuildInCompRendererCollection();
            //////texMap.AddRange(buildInCompRendererCollection.GetResMap(obj));

             
            //////BuildInCompTextCollection buildInCompTextCollection = new BuildInCompTextCollection();
            //////texMap.AddRange(buildInCompTextCollection.GetResMap(obj));

            //////ExtCompImageSequenceCollection extCompImageSequenceCollection = new ExtCompImageSequenceCollection();
            //////texMap.AddRange(extCompImageSequenceCollection.GetResMap(obj));

            //////ExtCompSpriteSequenceCollection extCompSpriteSequenceCollection = new ExtCompSpriteSequenceCollection();
            //////texMap.AddRange(extCompSpriteSequenceCollection.GetResMap(obj));

            //////ExtCompSwapSpriteCollection extCompSwapSpriteCollection = new ExtCompSwapSpriteCollection();
            //////texMap.AddRange(extCompSwapSpriteCollection.GetResMap(obj));

            //////ExtCompSwitch2ButtonCollection extCompSwitch2ButtonCollection = new ExtCompSwitch2ButtonCollection();
            //////texMap.AddRange(extCompSwitch2ButtonCollection.GetResMap(obj));
            */


            //反射获得所有实现了IRefResCollection的类 
            Assembly ass = Assembly.GetAssembly(typeof(IRefResCollection));
            Type[] types = ass.GetTypes();
            foreach (var item in types)
            {
                if (item.IsInterface)
                {
                    continue;//跳过接口自己
                }

                Type[] ins = item.GetInterfaces();
                foreach (var ty in ins)
                {
                    if (ty == typeof(IRefResCollection))
                    {
                        //Debug.LogError("xxx:" + item.ToString());                        
                        object target = Activator.CreateInstance(item);
                        MethodInfo getResMap = item.GetMethod("GetResMap");
                        var result = getResMap.Invoke(target, new object[] { obj });
                        if (result != null)
                        {
                            texMap.AddRange(result as List<AssetBundleBuild>);
                        }
                    }
                }

            }

            //获得Lightmap中的图
            LightmapInfoCollection lightmapInfoCollection = new LightmapInfoCollection();
            texMap.AddRange(lightmapInfoCollection.GetResMap(obj));

            //去除texMap中的重复项
            List<AssetBundleBuild> finalTexMap = new List<AssetBundleBuild>();
            for (int i = 0; i < texMap.Count; i++)
            {
                bool flag = false;
                for (int j = 0; j < finalTexMap.Count; j++)
                {
                    if (finalTexMap[j].assetBundleName == texMap[i].assetBundleName)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    finalTexMap.Add(texMap[i]);
                }
            }


            return finalTexMap;
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
                //TODO:后续需要限定，sprite名字不允许使用这几个，避免误判断
                //TODO：后续名字改成配置形式，also:ArtFastTool.cs
                if (img.sprite.name == "UISprite" ||
                    img.sprite.name == "Background" ||
                    img.sprite.name == "Knob" ||
                    img.sprite.name == "UIMask" ||
                    img.sprite.name == "InputFieldBackground" ||
                    img.sprite.name == "DropdownArrow" ||
                    img.sprite.name == "Checkmark")
                {
                    Debug.LogError("使用了默认贴图," + img.name + ", 贴图名：" + img.sprite.name + "， path:" + img.transform.GetHierarchy());
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
                Debug.LogError("used ugui's default mat: " + target.GetHierarchy());
                return false;
            }
            else
            {
                if (mat.shader == null)
                {
                    Debug.LogError("mat have no shader: " + target.GetHierarchy());
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