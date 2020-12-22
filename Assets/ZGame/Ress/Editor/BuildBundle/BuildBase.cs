using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Threading;
using ZGame.Ress.AB.Holder;
using ZGame.Ress.AB;

namespace ZGame.RessEditor
{
    /// <summary>
    /// 打包基类
    /// </summary>
    public class BuildBase
    {
        public string abPrefix;
        public virtual bool Build(UnityEngine.Object obj)
        {
            Debug.LogError("can not build ab:" + AssetDatabase.GetAssetOrScenePath(obj));
            return false;
        }

        /// <summary>
        /// 对预制件进行统一的检测
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns>有错误则返回false</returns>
        public bool CheckPrefab(GameObject prefab)
        {
            string path = AssetDatabase.GetAssetPath(prefab);
            if (!path.EndsWith(".prefab"))
            {
                Debug.LogError("target not a prefab , path:" + path);
                return false;
            }

            var allChild = (prefab as GameObject).GetComponentsInChildren(typeof(Transform), true);
            for (int i = 0; i < allChild.Length; i++)
            {
                if (BuildCommand.CheckValidOfU2D(allChild[i].gameObject) == false)
                {
                    return false;
                }
                if (BuildCommand.CheckValidOfRenderer(allChild[i].gameObject) == false)
                {
                    return false;
                }
                if (BuildCommand.CheckValidOfUGUI(allChild[i].gameObject) == false)
                {
                    return false;
                }
            }

            return true;
        }



        #region 处理U2D预制件相关依赖
        public virtual List<AssetBundleBuild> GetSpriteMapOfU2D(GameObject prefab)
        {
            List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();
            var holder = prefab.GetComponent<U2DSpriteHolder>();
            if (holder != null)
            {
                GameObject.DestroyImmediate(holder);
            }

            string prefabPath = AssetDatabase.GetAssetPath(prefab);
            var newPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            newPrefab.name = "tempNewPrefab_" + prefab.name;



            holder = newPrefab.AddComponent<U2DSpriteHolder>();

            //holder = prefab.AddComponent<UGUITextureHolder>();//报错
            holder.entities = new List<U2DSpriteHolder.SpriteEntity>();

            //保存需要打包的图片
            //key为图片名，value为路径
            var needBuildTextures = new Dictionary<string, string>();

            var allChilds = newPrefab.GetComponentsInChildren(typeof(Transform), true);
            for (int i = 0; i < allChilds.Length; i++)
            {
                SpriteRenderer sr = allChilds[i].GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    U2DSpriteHolder.SpriteEntity entity = new U2DSpriteHolder.SpriteEntity
                        (sr.transform, sr.sprite != null ? sr.sprite.texture.name : "", sr.sprite != null ? sr.sprite.name : "", sr.sharedMaterial, sr.sharedMaterial.shader.name);
                    holder.entities.Add(entity);

                    if (sr.sprite != null)
                    {
                        string texPath = AssetDatabase.GetAssetPath(sr.sprite);
                        //Debug.LogWarning("atlasName:" + sr.sprite.texture.name + ",spriteName:" + sr.sprite.name + ", texPath:" + texPath);
                        needBuildTextures[sr.sprite.texture.name] = texPath;
                    }
                }
            }


            //处理打包map                    
            foreach (var item in needBuildTextures)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = ABTypeUtil.GetPreFix(ABType.Sprite) + item.Key.ToLower() + IOTools.abSuffix;
                build.assetNames = new string[] { item.Value };
                buildMap.Add(build);
            }

            bool isSuccess = false;
            PrefabUtility.SaveAsPrefabAsset(newPrefab, prefabPath, out isSuccess);
            GameObject.DestroyImmediate(newPrefab);

            return buildMap;
        }

        public virtual void SetTextHolderOfU2D(GameObject prefab)
        {
            List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();
            var holder = prefab.GetComponent<U2DTextHolder>();
            if (holder != null)
            {
                GameObject.DestroyImmediate(holder);
            }

            string prefabPath = AssetDatabase.GetAssetPath(prefab);
            var newPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            newPrefab.name = "tempNewPrefab_" + prefab.name;



            holder = newPrefab.AddComponent<U2DTextHolder>();


            holder.entities = new List<U2DTextHolder.TextEntity>();



            var allChilds = newPrefab.GetComponentsInChildren(typeof(Transform), true);
            for (int i = 0; i < allChilds.Length; i++)
            {
                TextMesh text = allChilds[i].GetComponent<TextMesh>();
                if (text != null && text.font != null)
                {
                    U2DTextHolder.TextEntity entity = new U2DTextHolder.TextEntity
                        (text.transform, text.font.name);
                    holder.entities.Add(entity);
                }
            }


            bool isSuccess = false;
            PrefabUtility.SaveAsPrefabAsset(newPrefab, prefabPath, out isSuccess);
            GameObject.DestroyImmediate(newPrefab);
        }

        #endregion



        #region 处理目标物体材质球的图片依赖
        class BuildTextureValue
        {
            public string path;
            public MatTextureHolder.MatInfo.MatType matType;
            public BuildTextureValue(string path, MatTextureHolder.MatInfo.MatType matType)
            {
                this.path = path;
                this.matType = matType;
            }
        }

        public List<AssetBundleBuild> GetMatTexMap(string originPath, GameObject tmpPrefab)
        {
            List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();
            //////var holder = prefab.GetComponent<MatTexHolder>();
            //////if (holder != null)
            //////{
            //////    GameObject.DestroyImmediate(holder);
            //////}

            //////string prefabPath = AssetDatabase.GetAssetPath(prefab);
            ////////新版本的预制件系统，已经不能直接对预制件进行操作(比如AddComponent)了，必须把预制件实例化物体
            //////var newPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            //////newPrefab.name = "tempNewPrefab_" + prefab.name;

            //////holder = newPrefab.AddComponent<MatTexHolder>();


            var holder = tmpPrefab.GetComponent<MatTextureHolder>();
            var holders = tmpPrefab.GetComponents<MatTextureHolder>();
            if (holder == null)
            {
                Debug.LogError(originPath + " error, not have MatTexHolder component");
            }
            if (holders != null && holders.Length > 1)
            {
                Debug.LogError(originPath + " error, have more than 1 MatTexHolder");
            }
            if (holder.finishedSet == true)
            {
                Debug.LogError("finishedSet should be false，please uncheck!");
            }


            holder.allMatInfos = new List<MatTextureHolder.TransformMatInfo>();

            //保存需要打包的图片 或图集名
            var needBuildTextures = new Dictionary<string, BuildTextureValue>();


            //所有的Render类型组件，包括MeshRender,TrailRender,ParticleSystem使用的Render
            //还有 SpriteRenderer,其中SpriteRenderer使用的图片类型为 精灵 格式
            var rendererChilds = new List<Renderer>();
            tmpPrefab.GetComponentsInChildren<Renderer>(true, rendererChilds);

            var imageChilds = new List<Image>();//UGUI Image组件，使用的图片类型为 精灵 格式
            tmpPrefab.GetComponentsInChildren<Image>(true, imageChilds);


            var textChilds = new List<Text>();
            tmpPrefab.GetComponentsInChildren<Text>(true, textChilds);


            //TODO:其它类型有图片依赖的
            Debug.Log(tmpPrefab.name + "'s Renderer count:" + rendererChilds.Count);
            for (int i = 0; i < rendererChilds.Count; i++)
            {
                Renderer renderer = rendererChilds[i];

                //if (
                //    renderer.transform.Hierarchy().Contains("guaiwu") ||
                //    renderer.transform.Hierarchy().Contains("bullet") ||
                //    renderer.transform.Hierarchy().Contains("missile") ||
                //    renderer.transform.Hierarchy().Contains("trail") ||
                //    renderer.transform.Hierarchy().Contains("body"))
                //{
                //    continue;//说明是Spine做的东西，不需要处理依赖
                //}

                bool isSprite = false;
                var matInfos = new List<MatTextureHolder.MatInfo>();

                if (renderer is SpriteRenderer)
                {
                    isSprite = true;
                    SpriteRenderer sr = renderer as SpriteRenderer;
                    if (sr.sprite == null)
                    {
                        continue;
                    }
                    string atlasName = sr.sprite.texture.name;
                    string texName = sr.sprite.name;
                    MatTextureHolder.TextureInfo texInfo = new MatTextureHolder.TextureInfo(isSprite, "", texName, atlasName);


                    MatTextureHolder.MatInfo matInfo = new MatTextureHolder.MatInfo(sr.sharedMaterial,
                                 MatTextureHolder.MatInfo.MatType.SpriteRenderer,
                               sr.sharedMaterial.shader.name,
                               new List<MatTextureHolder.TextureInfo>() { texInfo }
                               );


                    string texPath = AssetDatabase.GetAssetPath(sr.sprite.texture);
                    needBuildTextures[sr.sprite.texture.name] = new BuildTextureValue(texPath, MatTextureHolder.MatInfo.MatType.SpriteRenderer);

                    matInfos.Add(matInfo);
                    MatTextureHolder.TransformMatInfo trsMatInfo = new MatTextureHolder.TransformMatInfo(renderer.transform, matInfos);

                    holder.allMatInfos.Add(trsMatInfo);
                }
                else
                {
                    isSprite = false;
                    var mats = renderer.sharedMaterials;
                    if (mats != null && mats.Length > 0)
                    {

                        foreach (var mat in mats)
                        {
                            if (mat == null)
                            {
                                continue;
                            }
                            if (mat.name == "Font Material")//对于字体
                            {
                                continue;
                            }
                            var texInfos = new List<MatTextureHolder.TextureInfo>();
                            for (int j = 0; j < ShaderUtil.GetPropertyCount(mat.shader); j++)
                            {
                                var t = ShaderUtil.GetPropertyType(mat.shader, j);
                                if (t == ShaderUtil.ShaderPropertyType.TexEnv)
                                {
                                    string propertyName = ShaderUtil.GetPropertyName(mat.shader, j);
                                    Texture tex = mat.GetTexture(propertyName);
                                    if (tex == null)
                                    {
                                        continue;
                                    }
                                    //检测使用的图片是否在工程内
                                    string texPath = AssetDatabase.GetAssetPath(tex);
                                    if (!texPath.Contains("Assets"))
                                    {
                                        Debug.LogError("error, texPath:" + texPath + ", renderer:" + renderer.gameObject);
                                        continue;
                                    }
                                    //TODO:这里需要检测使用的图片是否是基本图片类型，不允许使用sprite类型

                                    string atlasName = "";
                                    string texName = tex.name;


                                    needBuildTextures[tex.name] = new BuildTextureValue(texPath, MatTextureHolder.MatInfo.MatType.NormalRenderer);

                                    MatTextureHolder.TextureInfo texInfo = new MatTextureHolder.TextureInfo(isSprite, propertyName, texName, atlasName);
                                    texInfos.Add(texInfo);
                                }
                            }

                            MatTextureHolder.MatInfo matInfo = new MatTextureHolder.MatInfo(mat,
                               MatTextureHolder.MatInfo.MatType.NormalRenderer,
                               mat.shader.name,
                               texInfos);
                            matInfos.Add(matInfo);
                        }

                        MatTextureHolder.TransformMatInfo trsMatInfo = new MatTextureHolder.TransformMatInfo(renderer.transform, matInfos);

                        holder.allMatInfos.Add(trsMatInfo);
                    }
                    else
                    {
                        Debug.Log(renderer.name + " 没有材质球 [这里可能跟期望的不同，注意！！！]");
                    }
                }
            }


            Debug.Log(tmpPrefab.name + "'s Image count:" + imageChilds.Count);
            for (int i = 0; i < imageChilds.Count; i++)
            {
                Image image = imageChilds[i];
                bool isSprite = true;
                var matInfos = new List<MatTextureHolder.MatInfo>();
                if (image.sprite == null && image.material == null)//sprite和material都为null,则跳过。
                                                                   //只有sprite，没有material则是错误
                                                                   //只有material，没有sprite，属于正常情况
                {
                    continue;
                }
                string atlasName = image.sprite == null ? "" : image.sprite.texture.name;
                string texName = image.sprite == null ? "" : image.sprite.name;
                MatTextureHolder.TextureInfo texInfo = new MatTextureHolder.TextureInfo(isSprite, "", texName, atlasName);

                MatTextureHolder.MatInfo matInfo = new MatTextureHolder.MatInfo(image.material,
                             MatTextureHolder.MatInfo.MatType.Image,
                           image.material.shader.name,
                          image.sprite == null ? null : new List<MatTextureHolder.TextureInfo>() { texInfo }
                           );

                if (image.sprite != null)
                {
                    string texPath = AssetDatabase.GetAssetPath(image.sprite.texture);
                    needBuildTextures[image.sprite.texture.name] = new BuildTextureValue(texPath, MatTextureHolder.MatInfo.MatType.Image);
                }

                matInfos.Add(matInfo);
                MatTextureHolder.TransformMatInfo trsMatInfo = new MatTextureHolder.TransformMatInfo(image.transform, matInfos);

                holder.allMatInfos.Add(trsMatInfo);
            }



            Debug.Log(tmpPrefab.name + "'s Text Count:" + textChilds.Count);
            for (int i = 0; i < textChilds.Count; i++)
            {
                Text text = textChilds[i];
                bool isSprite = false;
                var matInfos = new List<MatTextureHolder.MatInfo>();

                string atlasName = "";
                string texName = "";
                MatTextureHolder.TextureInfo texInfo = new MatTextureHolder.TextureInfo(isSprite, "", texName, atlasName);

                MatTextureHolder.MatInfo matInfo = new MatTextureHolder.MatInfo(text.material,
                             MatTextureHolder.MatInfo.MatType.Image,
                           text.material.shader.name,
                           null
                           );


                matInfos.Add(matInfo);
                MatTextureHolder.TransformMatInfo trsMatInfo = new MatTextureHolder.TransformMatInfo(text.transform, matInfos);

                holder.allMatInfos.Add(trsMatInfo);
            }


            //处理打包map   
            Debug.Log(tmpPrefab.name + "'s Texture count:" + needBuildTextures.Keys.Count);
            foreach (var item in needBuildTextures)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                string preFix = "";
                switch (item.Value.matType)
                {
                    case MatTextureHolder.MatInfo.MatType.Image:
                        preFix = ABTypeUtil.GetPreFix(ABType.Sprite);
                        break;
                    case MatTextureHolder.MatInfo.MatType.SpriteRenderer:
                        preFix = ABTypeUtil.GetPreFix(ABType.Sprite);
                        break;
                    case MatTextureHolder.MatInfo.MatType.NormalRenderer:
                        preFix = ABTypeUtil.GetPreFix(ABType.Texture);
                        break;
                    default:
                        break;
                }
                build.assetBundleName = preFix + item.Key.ToLower() + IOTools.abSuffix;
                build.assetNames = new string[] { item.Value.path };
                buildMap.Add(build);
            }

            return buildMap;
        }


        #endregion




        #region 处理UGUI预制件相关依赖
        //////public static List<AssetBundleBuild> GetSpriteMapOfUGUI(GameObject prefab)
        //////{
        //////    List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();
        //////    var holder = prefab.GetComponent<UGUISpriteHolder>();
        //////    if (holder != null)
        //////    {
        //////        GameObject.DestroyImmediate(holder);
        //////    }

        //////    string prefabPath = AssetDatabase.GetAssetPath(prefab);
        //////    var newPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        //////    newPrefab.name = "tempNewPrefab_" + prefab.name;



        //////    holder = newPrefab.AddComponent<UGUISpriteHolder>();


        //////    holder.entities = new List<UGUISpriteHolder.SpriteEntity>();

        //////    //保存需要打包的图片
        //////    //key为图片名，value为路径
        //////    var needBuildTextures = new Dictionary<string, string>();

        //////    var allChilds = newPrefab.GetComponentsInChildren(typeof(Transform), true);
        //////    for (int i = 0; i < allChilds.Length; i++)
        //////    {
        //////        Image img = allChilds[i].GetComponent<Image>();
        //////        if (img != null)
        //////        {
        //////            UGUISpriteHolder.SpriteEntity entity = new UGUISpriteHolder.SpriteEntity
        //////                (UGUISpriteHolder.UIType.Image, img.transform, img.sprite != null ? img.sprite.texture.name : "",
        //////                img.sprite != null ? img.sprite.name : "", img.material, img.material.shader.name);
        //////            holder.entities.Add(entity);

        //////            if (img.sprite != null)
        //////            {
        //////                string texPath = AssetDatabase.GetAssetPath(img.sprite);
        //////                //Debug.LogWarning("atlasName:" + img.mainTexture.name + ",spriteName:" + img.sprite.name + ", texPath:" + texPath);           
        //////                needBuildTextures[img.mainTexture.name] = texPath;
        //////            }

        //////        }
        //////    }


        //////    //处理打包map                    
        //////    foreach (var item in needBuildTextures)
        //////    {
        //////        AssetBundleBuild build = new AssetBundleBuild();
        //////        build.assetBundleName = ABTypeUtil.GetPreFix(ABType.Sprite) + item.Key.ToLower() + IOTools.abSuffix;
        //////        build.assetNames = new string[] { item.Value };
        //////        buildMap.Add(build);
        //////    }




        //////    bool isSuccess = false;
        //////    PrefabUtility.SaveAsPrefabAsset(newPrefab, prefabPath, out isSuccess);
        //////    GameObject.DestroyImmediate(newPrefab);
        //////    return buildMap;
        //////}


        //////public static void SetTextHolderOfUGUI(GameObject prefab)
        //////{
        //////    List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();
        //////    var holder = prefab.GetComponent<UGUITextHolder>();
        //////    if (holder != null)
        //////    {
        //////        GameObject.DestroyImmediate(holder);
        //////    }

        //////    string prefabPath = AssetDatabase.GetAssetPath(prefab);
        //////    var newPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        //////    newPrefab.name = "tempNewPrefab_" + prefab.name;



        //////    holder = newPrefab.AddComponent<UGUITextHolder>();

        //////    //holder = prefab.AddComponent<UGUITextureHolder>();//报错
        //////    holder.entities = new List<UGUITextHolder.TextEntity>();



        //////    var allChilds = newPrefab.GetComponentsInChildren(typeof(Transform), true);
        //////    for (int i = 0; i < allChilds.Length; i++)
        //////    {
        //////        Text text = allChilds[i].GetComponent<Text>();
        //////        if (text != null && text.font != null)
        //////        {
        //////            UGUITextHolder.TextEntity entity = new UGUITextHolder.TextEntity
        //////                (text.transform, text.font.name);
        //////            holder.entities.Add(entity);
        //////        }
        //////    }


        //////    bool isSuccess = false;
        //////    PrefabUtility.SaveAsPrefabAsset(newPrefab, prefabPath, out isSuccess);
        //////    GameObject.DestroyImmediate(newPrefab);

        //////}
        #endregion

        #region 处理场景依赖


        #endregion

        #region 处理2D Sprite预制件相关依赖
        #endregion
    }
}