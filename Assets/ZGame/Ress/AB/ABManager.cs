using AYellowpaper.SerializedCollections;
using FSG.MeshAnimator.ShaderAnimated;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ZGame.Event;
using ZGame.Ress.AB.Holder;
using ZGame.Ress.Info;
using ZGame.UGUIExtention;


namespace ZGame.Ress.AB
{
    public class ABCache : SingletonMonoBehaviour<ABCache>
    {
        public SerializedDictionary<string, SpriteRes> spriteCacheDic = new SerializedDictionary<string, SpriteRes>();

        public SerializedDictionary<string, TextureRes> textureCacheDic = new SerializedDictionary<string, TextureRes>();
        public SerializedDictionary<string, MatRes> matCacheDic = new SerializedDictionary<string, MatRes>();
        public SerializedDictionary<string, AudioRes> audioCacheDic = new SerializedDictionary<string, AudioRes>();
        public SerializedDictionary<string, MeshRes> meshCacheDic = new SerializedDictionary<string, MeshRes>();
        public SerializedDictionary<string, ObjectRes> objectCacheDic = new SerializedDictionary<string, ObjectRes>();



        public SerializedDictionary<string, WindowRes> windowCacheDic = new SerializedDictionary<string, WindowRes>();
        public SerializedDictionary<string, EffectRes> effectCacheDic = new SerializedDictionary<string, EffectRes>();
        public SerializedDictionary<string, OtherPrefabRes> otherPrefabCacheDic = new SerializedDictionary<string, OtherPrefabRes>();


        public void RemoveRes(Res res)
        {
            if (res is SpriteRes)
            {
                spriteCacheDic.Remove(res.resName);
            }
            else if (res is TextureRes)
            {
                textureCacheDic.Remove(res.resName);
            }
            else if (res is MatRes)
            {
                matCacheDic.Remove(res.resName);
            }
            else if (res is AudioRes)
            {
                audioCacheDic.Remove(res.resName);
            }
            else if (res is MeshRes)
            {
                meshCacheDic.Remove(res.resName);
            }
            else if (res is ObjectRes)
            {
                objectCacheDic.Remove(res.resName);
            }

            else if (res is WindowRes)
            {
                windowCacheDic.Remove(res.resName);
            }
            else if (res is EffectRes)
            {
                effectCacheDic.Remove(res.resName);
            }
            else if (res is OtherPrefabRes)
            {
                otherPrefabCacheDic.Remove(res.resName);
            }
        }

        public void RemoveRes(GameObject targetObj, ABType abType)
        {
            string targetKey = "";
            switch (abType)
            {
                case ABType.Effect:
                    foreach (var item in effectCacheDic)
                    {
                        if (item.Value.resObj == targetObj)
                        {
                            targetKey = item.Key;
                            break;
                        }
                    }
                    if (targetKey != "")
                    {
                        effectCacheDic.Remove(targetKey);
                    }
                    break;
                case ABType.Window:

                    foreach (var item in windowCacheDic)
                    {
                        if (item.Value.resObj == targetObj)
                        {
                            targetKey = item.Key;
                            break;
                        }
                    }
                    if (targetKey != "")
                    {
                        windowCacheDic.Remove(targetKey);
                    }
                    break;
                case ABType.OtherPrefab:
                    foreach (var item in otherPrefabCacheDic)
                    {
                        if (item.Value.resObj == targetObj)
                        {
                            targetKey = item.Key;
                            break;
                        }
                    }
                    if (targetKey != "")
                    {
                        otherPrefabCacheDic.Remove(targetKey);
                    }
                    break;
                default:
                    Debug.Log("not supported yet, abType:" + abType);
                    break;

            }
        }

        public void AddRes(Res res)
        {
            if (res is SpriteRes)
            {
                if (this.spriteCacheDic.ContainsKey(res.resName) == false)
                {
                    this.spriteCacheDic[res.resName] = res as SpriteRes;
                }
            }
            else if (res is TextureRes)
            {
                if (this.textureCacheDic.ContainsKey(res.resName) == false)
                {
                    this.textureCacheDic[res.resName] = res as TextureRes;
                }
            }
            else if (res is MatRes)
            {
                if (this.matCacheDic.ContainsKey(res.resName) == false)
                {
                    this.matCacheDic[res.resName] = res as MatRes;
                }
            }
            else if (res is AudioRes)
            {
                if (this.audioCacheDic.ContainsKey(res.resName) == false)
                {
                    this.audioCacheDic[res.resName] = res as AudioRes;
                }
            }
            else if (res is MeshRes)
            {
                if (this.meshCacheDic.ContainsKey(res.resName) == false)
                {
                    this.meshCacheDic[res.resName] = res as MeshRes;
                }
            }
            else if (res is ObjectRes)
            {
                if (this.objectCacheDic.ContainsKey(res.resName) == false)
                {
                    this.objectCacheDic[res.resName] = res as ObjectRes;
                }
            }
            else if (res is PrefabRes)
            {
                if (res is WindowRes)
                {
                    if (this.windowCacheDic.ContainsKey(res.resName) == false)
                    {
                        this.windowCacheDic[res.resName] = res as WindowRes;
                    }
                }
                else if (res is OtherPrefabRes)
                {
                    if (this.otherPrefabCacheDic.ContainsKey(res.resName) == false)
                    {
                        this.otherPrefabCacheDic[res.resName] = res as OtherPrefabRes;
                    }
                }
                else if (res is EffectRes)
                {
                    if (this.effectCacheDic.ContainsKey(res.resName) == false)
                    {
                        this.effectCacheDic[res.resName] = res as EffectRes;
                    }
                }
            }

        }

        public Res GetRes(string name, ABType abType)
        {
            if (abType == ABType.Sprite)
            {
                if (this.spriteCacheDic.ContainsKey(name))
                {
                    return this.spriteCacheDic[name];
                }
            }
            else if (abType == ABType.Texture)
            {
                if (this.textureCacheDic.ContainsKey(name))
                {
                    return this.textureCacheDic[name];
                }
            }
            else if (abType == ABType.Material)
            {
                if (this.matCacheDic.ContainsKey(name))
                {
                    return this.matCacheDic[name];
                }
            }
            else if (abType == ABType.Audio)
            {
                if (this.audioCacheDic.ContainsKey(name))
                {
                    return this.audioCacheDic[name];
                }
            }
            else if (abType == ABType.Mesh)
            {
                if (this.meshCacheDic.ContainsKey(name))
                {
                    return this.meshCacheDic[name];
                }
            }
            else if (abType == ABType.Object)
            {
                if (this.objectCacheDic.ContainsKey(name))
                {
                    return this.objectCacheDic[name];
                }
            }
            //其它类型不走缓存，有必要自己通过对象池来实现

            return null;
        }
    }


    public class ABManager : Singleton<ABManager>
    {
        bool isInit = false;

        public ABManager()
        {
            if (!isInit)
            {
                AB.Load("common", ABType.Null, (objs) =>
                {

                    int count = objs.Length;
                    for (int i = 0; i < count; i++)
                    {
                        if (objs[i].name == "ShaderVariants")
                        {
                            Debug.Log("ShaderVariants WarmUp");
                            (objs[i] as ShaderVariantCollection).WarmUp();//WarmUp() all variants,in case of stuck while to complie variant at run time
                        }
                    }

                    EventDispatcher.Instance.AddListener(EventID.OnABResLoaded, onABResLoaded);
                    EventDispatcher.Instance.AddListener(EventID.OnRootObjDestroy, onRootObjDestroy);
                    EventDispatcher.Instance.AddListener(EventID.OnChildObjDestroy, onChildObjDestroy);

                    EventDispatcher.Instance.AddListener(EventID.OnGameObjectInstantiate, onGameObjectInstantiate);
                    isInit = true;

                }, true, true);// load common ab,we can not call ab.Unload(false) , otherwise shader will miss on mobile/PC devices!so set keep AB status is true.
            }
        }


        public void RemoveCachedRes(ABType abType, Res res)
        {
            ABCache.Instance.RemoveRes(res);
        }
        public void RemoveCachedRes(ABType abType, GameObject obj)
        {
            ABCache.Instance.RemoveRes(obj, abType);
        }
        public Res GetCachedRes(ABType abType, string name)
        {
            return ABCache.Instance.GetRes(name, abType);
        }


        private void onGameObjectInstantiate(string evtId, object[] paras)
        {
            GameObject obj = paras[0] as GameObject;
            DynamicCompInfoHolder dynamicHolder = obj.GetComponent<DynamicCompInfoHolder>();
            if (dynamicHolder != null)
            {
                this.fillReferenceOfDynamicCompInfoHolder(obj);
                this.AddDestroyNotice(obj);
            }

            //////RootCompInfoHolder rootCompInfoHolder = obj.GetComponent<RootCompInfoHolder>();
            //////if (rootCompInfoHolder != null)
            //////{
            //////    this.FillReferenceOfRootCompInfoHolder(obj, true);
            //////    this.AddDestroyNotice(obj);
            //////}

        }

        private void onChildObjDestroy(string evtId, object[] paras)
        {
            GameObject obj = paras[0] as GameObject;
            removeSpriteReference(obj.transform);
            removeTextureReference(obj.transform);
            removeMatReference(obj.transform);
        }

        void removeSpriteReference(Transform trans)
        {
            //TODO:loop check cost too much.Need change!
            var spriteCacheList = new List<SpriteRes>(ABCache.Instance.spriteCacheDic.Values);
            SpriteRes sr;
            for (int j = spriteCacheList.Count - 1; j >= 0; j--)
            {
                sr = spriteCacheList[j];
                if (sr.CheckRefTrs(trans))
                {
                    sr.RemoveRefTrs(trans);
                }
            }

        }
        void removeTextureReference(Transform trans)
        {
            //TODO:loop check cost too much.Need change!  
            var textureCacheList = new List<TextureRes>(ABCache.Instance.textureCacheDic.Values);
            TextureRes tr;
            for (int j = textureCacheList.Count - 1; j >= 0; j--)
            {
                tr = textureCacheList[j];
                if (tr.CheckRefTrs(trans))
                {
                    tr.RemoveRefTrs(trans);
                }
            }

        }
        void removeMatReference(Transform trans)
        {
            //TODO:loop check cost too much.Need change!  
            var matCacheList = new List<MatRes>(ABCache.Instance.matCacheDic.Values);
            MatRes mr;
            for (int j = matCacheList.Count - 1; j >= 0; j--)
            {
                mr = matCacheList[j];
                if (mr.CheckRefTrs(trans))
                {
                    mr.RemoveRefTrs(trans);
                }
            }
        }
        void onRootObjDestroy(string evtId, object[] paras)
        {
            GameObject obj = paras[0] as GameObject;
            RootCompInfoHolder holder = obj.GetComponent<RootCompInfoHolder>();
            if (holder != null)
            {
                ABType abType = holder.abType;
                RemoveCachedRes(abType, obj);
            }
        }

        public void AddDestroyNotice(GameObject rootObj)
        {
            rootObj.GetOrAddComponent<OnObjDestroyNotice>();
        }
        private void onABResLoaded(string evtId, object[] paras)
        {
            Res res = paras[0] as Res;

            bool sync = true;
            if (paras.Length >= 2 && paras[1] != null)
            {
                sync = bool.Parse(paras[1].ToString());
            }

            if (res is WindowRes)
            {
                WindowRes wRes = res as WindowRes;

                GameObject obj = wRes.resObj as GameObject;
                FillReferenceOfRootCompInfoHolder(obj, sync);
                this.AddDestroyNotice(obj);

                wRes.AddRefTrs(obj.transform);


                ABCache.Instance.AddRes(wRes);
            }
            else if (res is EffectRes)
            {
                EffectRes effRes = res as EffectRes;

                GameObject obj = effRes.resObj as GameObject;
                FillReferenceOfRootCompInfoHolder(obj, sync);
                this.AddDestroyNotice(obj);

                effRes.AddRefTrs(obj.transform);

                ABCache.Instance.AddRes(effRes);
            }
            else if (res is OtherPrefabRes)
            {
                OtherPrefabRes opRes = res as OtherPrefabRes;
                GameObject obj = opRes.resObj as GameObject;
                FillReferenceOfRootCompInfoHolder(obj, sync);
                this.AddDestroyNotice(obj);

                opRes.AddRefTrs(obj.transform);

                ABCache.Instance.AddRes(opRes);
            }
            else if (res is SpriteRes)
            {
                SpriteRes sRes = res as SpriteRes;

                ABCache.Instance.AddRes(sRes);
            }
            else if (res is TextureRes)
            {
                TextureRes tRes = res as TextureRes;
                ABCache.Instance.AddRes(tRes);
            }
            else if (res is MatRes)
            {
                MatRes matRes = res as MatRes;
                ABCache.Instance.AddRes(matRes);
            }
            else if (res is MeshRes)
            {
                MeshRes meshRes = res as MeshRes;
                ABCache.Instance.AddRes(meshRes);
            }
            else if (res is AudioRes)
            {
                AudioRes aRes = res as AudioRes;
                ABCache.Instance.AddRes(aRes);
            }
            else if (res is ObjectRes)
            {
                ObjectRes oRes = res as ObjectRes;
                ABCache.Instance.AddRes(oRes);
            }
            else
            {
                Debug.LogError("onABResLoaded TODO::::" + res.resName);
            }
        }




        public void LoadWindow(string name, Action<UnityEngine.Object> callback, bool sync)
        {
            ABPrefab.Load(name, ABType.Window, callback, sync);
        }


        public void LoadScene(string name, LoadSceneMode loadSceneMode, Action<UnityEngine.Object> sceneLoadedCallback, Action lightmapAttachedCallback, bool sync)
        {
            ABScene.Load(name, loadSceneMode, sceneLoadedCallback, lightmapAttachedCallback, sync);
        }

        public void UnloadScene(string name, Action callback)
        {
            ABScene.Unload(name, callback);
        }

        public void LoadLargeScene(string name, LoadSceneMode loadSceneMode, Vector3 initPos, Action<UnityEngine.Object> sceneLoadedCallback, Action initChunksLoadedCallback, bool sync)
        {
            ABLargeScene.Load(name, loadSceneMode, initPos, sceneLoadedCallback, initChunksLoadedCallback, sync);
        }

        public void UnloadLargeScene(string name, Action callback)
        {
            ABLargeScene.Unload(name, callback);
        }


        public void LoadEffect(string name, Action<UnityEngine.Object> callback, bool sync)
        {
            ABPrefab.Load(name, ABType.Effect, callback, sync);
        }

        public void LoadOtherPrefab(string name, Action<UnityEngine.Object> callback, bool sync)
        {
            ABPrefab.Load(name, ABType.OtherPrefab, callback, sync);
        }

        public void ResetEditorShader(Transform tran, Material mat, string shaderName)
        {
#if UNITY_EDITOR
            if (mat != null)
            {
                var shader = Shader.Find(shaderName);
                if (shader != null)
                {
                    mat.shader = shader;

                    //bug hack处理
                    //在较低版本的Unity中，如果是standard shader且使用了Transparent或Fade或Cutout的渲染模式，通过AB的方式加载模型，那么在编辑器模式下RenderQueue可能值出错，造成渲染顺序上的错误
                    //2021.3.8f1中对于renderQueue值不匹配的已经自动设置了默认值，相关代码参见 StandardShaderGUI.cs(https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Inspector/StandardShaderGUI.cs)
                    //唯一恼火的是在客户端会打印出很多重置RenderQueue的日志(StandardShaderGUI.cs line 410)。因此本项目集成了官方的StardardShaderGUI.cs，并注释了相关打印信息

                    if (shaderName == "My/Standard" || shaderName == "My/Standard (Specular setup)")
                    {
                        if (mat.IsKeywordEnabled("_ALPHAPREMULTIPLY_ON"))
                        {
                            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                        }
                        if (mat.IsKeywordEnabled("_ALPHABLEND_ON"))
                        {
                            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                        }
                        if (mat.IsKeywordEnabled("_ALPHATEST_ON"))
                        {
                            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                        }
                    }
                }
                else
                {
                    DebugExt.LogE("can not find shader:" + shaderName);
                }
            }

#endif

        }

        void fillReferenceOfDynamicCompInfoHolder(GameObject target)
        {
            DynamicCompInfoHolder holder = target.GetComponent<DynamicCompInfoHolder>();
            if (holder == null)
            {
                DebugExt.LogE("no dynamicCompInfoHolder attached:" + target.GetHierarchy());
                return;
            }

            //----------------->BuildIn Image
            if (holder.buildInCompImageInfos != null && holder.buildInCompImageInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompImageInfos)
                {
                    Transform childTarget = item.tran;
                    if (item.refSprites != null && item.refSprites.Count > 0)
                    {
                        ImgRes imgRes = this.GetCachedRes(ABType.Sprite, item.refSprites[0].atlasName) as ImgRes;
                        if (imgRes != null)
                        {
                            imgRes.AddRefTrs(childTarget);
                        }
                        else
                        {
                            Debug.LogError("error, please check Image");
                        }
                    }
                    if (item.matIndex != -1)
                    {
                        MatRes matRes = this.GetCachedRes(ABType.Material, item.matName) as MatRes;
                        if (matRes != null)
                        {
                            matRes.AddRefTrs(childTarget);
                        }
                        else
                        {
                            Debug.LogError("error, please check mat");
                        }
                    }
                }
            }

            //-------------->BuildIn SpriteRenderer
            if (holder.buildInCompSpriteRendererInfos != null && holder.buildInCompSpriteRendererInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompSpriteRendererInfos)
                {
                    Transform childTarget = item.tran;
                    if (item.refSprites != null && item.refSprites.Count > 0)
                    {
                        ImgRes imgRes = this.GetCachedRes(ABType.Sprite, item.refSprites[0].atlasName) as ImgRes;
                        if (imgRes != null)
                        {
                            imgRes.AddRefTrs(childTarget);
                        }
                        else
                        {
                            Debug.LogError("error, please check SpriteRenderer");
                        }
                    }
                    if (item.matIndex != -1)
                    {
                        MatRes matRes = this.GetCachedRes(ABType.Material, item.matName) as MatRes;
                        if (matRes != null)
                        {
                            matRes.AddRefTrs(childTarget);
                        }
                        else
                        {
                            Debug.LogError("error, please check Material");
                        }
                    }
                }
            }

            //-------------->BuildIn Renderer
            if (holder.buildInCompRendererInfos != null && holder.buildInCompRendererInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompRendererInfos)
                {
                    Transform childTarget = item.tran;
                    for (int k = 0; k < item.refTextures.Count; k++)
                    {
                        ImgRes imgRes = this.GetCachedRes(ABType.Texture, item.refTextures[k].texName) as ImgRes;
                        if (imgRes != null)
                        {
                            imgRes.AddRefTrs(childTarget);
                        }
                        else
                        {
                            Debug.LogError("error, please check, no imgRes with texName:" + item.refTextures[k].texName);
                        }
                    }
                }
            }

            //-------------->Ext ImageSequence
            if (holder.extCompImageSequenceInfos != null && holder.extCompImageSequenceInfos.Count > 0)
            {
                foreach (var item in holder.extCompImageSequenceInfos)
                {
                    Transform childTarget = item.tran;
                    if (item.refSprites != null && item.refSprites.Count > 0)
                    {
                        for (int i = 0; i < item.refSprites.Count; i++)
                        {
                            ImgRes imgRes = this.GetCachedRes(ABType.Sprite, item.refSprites[i].atlasName) as ImgRes;
                            if (imgRes != null)
                            {
                                imgRes.AddRefTrs(childTarget);
                            }
                            else
                            {
                                DebugExt.LogE("error, please check ImageSequence");
                            }
                        }
                    }

                }
            }

            //-------------->Ext SpriteSequence
            if (holder.extCompSpriteSequenceInfos != null && holder.extCompSpriteSequenceInfos.Count > 0)
            {
                foreach (var item in holder.extCompSpriteSequenceInfos)
                {
                    Transform childTarget = item.tran;
                    if (item.refSprites != null && item.refSprites.Count > 0)
                    {
                        for (int i = 0; i < item.refSprites.Count; i++)
                        {
                            ImgRes imgRes = this.GetCachedRes(ABType.Sprite, item.refSprites[i].atlasName) as ImgRes;
                            if (imgRes != null)
                            {
                                imgRes.AddRefTrs(childTarget);
                            }
                            else
                            {
                                DebugExt.LogE("error, please check SpriteSequence");
                            }
                        }
                    }
                }
            }
            //-------------->Ext SwapSprite
            if (holder.extCompSwapSpriteInfos != null && holder.extCompSwapSpriteInfos.Count > 0)
            {

                foreach (var item in holder.extCompSwapSpriteInfos)
                {
                    Transform childTarget = item.tran;
                    if (item.refSprites != null && item.refSprites.Count > 0)
                    {
                        for (int i = 0; i < item.refSprites.Count; i++)
                        {
                            ImgRes imgRes = this.GetCachedRes(ABType.Sprite, item.refSprites[i].atlasName) as ImgRes;
                            if (imgRes != null)
                            {
                                imgRes.AddRefTrs(childTarget);
                            }
                            else
                            {
                                DebugExt.LogE("error, please check SwapSprite");
                            }
                        }
                    }
                }
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="sync">是否同步</param>
        public void FillReferenceOfRootCompInfoHolder(GameObject target, bool sync = true)
        {
            RootCompInfoHolder holder = target.GetComponent<RootCompInfoHolder>();
            if (holder == null || holder.finishedSet)
            {
                return;
            }


            //----------------->BuildIn Image
            if (holder.buildInCompImageInfos != null && holder.buildInCompImageInfos.Count > 0)
            {

                foreach (var item in holder.buildInCompImageInfos)
                {
                    Transform childTarget = item.tran;
                    Material imageMat = null;
                    if (item.matIndex != -1)
                    {
                        this.LoadMatWithCheck(item.matName, (mat) =>
                        {
                            imageMat = mat;
                            item.concreteCompImage.material = imageMat;
                        }, item.tran);
                    }
                    if (imageMat != null)
                    {
                        //add mat ref 
                        MatRes matRes = this.GetCachedRes(ABType.Material, item.matName) as MatRes;
                        matRes.AddRefTrs(childTarget);
                        if (item.refSprites != null && item.refSprites.Count > 0)
                        {
                            LoadSpriteToTarget<Image>(item.concreteCompImage, item.refSprites[0].atlasName, item.refSprites[0].spriteName, sync);
                        }

                        ResetEditorShader(childTarget, imageMat, item.shaderName);
                    }

                }
            }
            //-------------->BuildIn RawImage
            if (holder.buildInCompRawImageInfos != null && holder.buildInCompRawImageInfos.Count > 0)
            {

                foreach (var item in holder.buildInCompRawImageInfos)
                {
                    Transform childTarget = item.tran;
                    Material rawImgMat = null;
                    if (item.matIndex != -1)
                    {
                        this.LoadMatWithCheck(item.matName, (mat) =>
                        {
                            rawImgMat = mat;
                            item.concreteCompRawImage.material = mat;
                        }, item.tran);
                    }
                    if (rawImgMat != null)
                    {
                        //add mat ref 
                        MatRes matRes = this.GetCachedRes(ABType.Material, item.matName) as MatRes;
                        matRes.AddRefTrs(childTarget);

                        if (item.texName != "")
                        {

                            LoadTexture(item.texName, (tex) =>
                            {
                                var oldTex = item.concreteCompRawImage.texture;
                                string oldTexName = null;
                                if (oldTex != null)
                                {
                                    oldTexName = oldTex.name;
                                }

                                item.concreteCompRawImage.texture = tex;

                                //remove old ref
                                if (string.IsNullOrEmpty(oldTexName) == false && oldTexName != item.texName)
                                {
                                    var oldRes = this.GetCachedRes(ABType.Texture, oldTexName);
                                    if (oldRes != null)
                                    {
                                        oldRes.RemoveRefTrs(item.tran);
                                    }
                                }

                                //add ref
                                if (item.tran != null)
                                {
                                    TextureRes texRes = this.GetCachedRes(ABType.Texture, item.texName) as TextureRes;
                                    texRes.AddRefTrs(item.tran);
                                }

                            }, sync);
                        }

                        ResetEditorShader(childTarget, rawImgMat, item.shaderName);
                    }

                }
            }


            //-------------->BuildIn SpriteRenderer
            if (holder.buildInCompSpriteRendererInfos != null && holder.buildInCompSpriteRendererInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompSpriteRendererInfos)
                {
                    Transform childTarget = item.tran;
                    Material srMat = null;
                    if (item.matIndex != -1)
                    {
                        LoadMatWithCheck(item.matName, (mat) =>
                        {
                            srMat = mat;
                            item.concreteCompSpriteRenderer.material = srMat;

                        }, item.tran);
                    }
                    if (srMat != null)
                    {
                        //add mat ref 
                        MatRes matRes = this.GetCachedRes(ABType.Material, item.matName) as MatRes;
                        matRes.AddRefTrs(childTarget);
                        if (item.refSprites != null && item.refSprites.Count > 0)
                        {
                            LoadSpriteToTarget<SpriteRenderer>(childTarget.GetComponent<SpriteRenderer>(), item.refSprites[0].atlasName,
                                                   item.refSprites[0].spriteName, sync);
                        }

                        ResetEditorShader(childTarget, srMat, item.shaderName);
                    }
                }
            }
            //-------------->BuildIn Text
            if (holder.buildInCompTextInfos != null && holder.buildInCompTextInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompTextInfos)
                {
                    Transform childTarget = item.tran;
                    Material textMat = null;
                    if (item.matIndex != -1)
                    {
                        LoadMatWithCheck(item.matName, (mat) =>
                        {
                            textMat = mat;
                            item.concreteCompText.material = textMat;

                        }, item.tran);
                    }
                    if (textMat != null)
                    {
                        //add mat ref 
                        MatRes matRes = this.GetCachedRes(ABType.Material, item.matName) as MatRes;
                        matRes.AddRefTrs(childTarget);

                        ResetEditorShader(childTarget, textMat, item.shaderName);
                    }
                }
            }
            //-------->BuildIn TextMeshProUGUI
            if (holder.buildInCompTextMeshProUGUIInfos != null && holder.buildInCompTextMeshProUGUIInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompTextMeshProUGUIInfos)
                {
                    Transform childTarget = item.tran;
                    Material tmpTxtMat = null;
                    if (item.matIndex != -1)
                    {
                        LoadMatWithCheck(item.matName, (mat) =>
                        {
                            tmpTxtMat = mat;
                            item.concreteCompTextMeshProUGUI.material = tmpTxtMat;

                        }, item.tran);
                    }
                    if (tmpTxtMat != null)
                    {
                        //add mat ref 
                        MatRes matRes = this.GetCachedRes(ABType.Material, item.matName) as MatRes;
                        matRes.AddRefTrs(childTarget);

                        ResetEditorShader(childTarget, tmpTxtMat, item.shaderName);
                    }
                }
            }

            //-------------->BuildIn Renderer
            if (holder.buildInCompRendererInfos != null && holder.buildInCompRendererInfos.Count > 0)
            {
                //Debug.Log(target.name + ", buildInRenderer count:" + holder.buildInCompRendererInfos.Count + ", sync:" + sync.ToString());
                foreach (var item in holder.buildInCompRendererInfos)
                {
                    if (sync)
                    {
                        if (item.meshName != "")
                        {
                            Mesh mesh = null;
                            LoadMesh(item.meshName, (m) =>
                            {
                                mesh = m;
                                if (item.concreteCompRenderer is MeshRenderer)
                                {
                                    item.tran.GetComponent<MeshFilter>().sharedMesh = mesh;

                                    ShaderMeshAnimator sma = item.tran.GetComponent<ShaderMeshAnimator>();
                                    if (sma != null)
                                    {
                                        sma.baseMesh = mesh;
                                    }
                                }
                                else if (item.concreteCompRenderer is SkinnedMeshRenderer)
                                {
                                    (item.concreteCompRenderer as SkinnedMeshRenderer).sharedMesh = mesh;
                                }
                                else if (item.concreteCompRenderer is ParticleSystemRenderer)
                                {
                                    (item.concreteCompRenderer as ParticleSystemRenderer).mesh = mesh;
                                }

                                //add mesh ref 
                                MeshRes meshRes = this.GetCachedRes(ABType.Mesh, item.meshName) as MeshRes;
                                meshRes.AddRefTrs(item.tran);

                            }, sync);
                        }

                        if (item.matIndex != -1)
                        {
                            Material renderMat = null;
                            LoadMatWithCheck(item.matName, (mat) =>
                            {
                                renderMat = mat;
                                if (item.concreteCompRenderer is ParticleSystemRenderer)//particleSystemRenderer需要特殊处理
                                {
                                    item.concreteCompRenderer.sharedMaterial = renderMat;
                                }
                                else
                                {
                                    Material[] sharedMaterialsCopy = item.concreteCompRenderer.sharedMaterials;
                                    sharedMaterialsCopy[item.matIndex] = renderMat;
                                    item.concreteCompRenderer.sharedMaterials = sharedMaterialsCopy;
                                }
                            }, item.tran);
                            if (renderMat != null)
                            {
                                Transform childTarget = item.tran;
                                //add mat ref 
                                MatRes matRes = this.GetCachedRes(ABType.Material, item.matName) as MatRes;
                                matRes.AddRefTrs(childTarget);

                                if (item.refTextures != null && item.refTextures.Count > 0)
                                {
                                    for (int k = 0; k < item.refTextures.Count; k++)
                                    {
                                        LoadTextureToMat(childTarget, item.refTextures[k].texName, item.refTextures[k].shaderProp, renderMat, sync, null);
                                    }
                                }
                                ResetEditorShader(childTarget, renderMat, item.shaderName);
                            }
                        }
                    }
                    else
                    {
                        BuildInCompRenderInfoSyncOperator.Instance.Add(item);
                    }

                }
            }

            //-------------->BuildIn MeshCollider
            if (holder.buildInCompMeshColliderInfos != null && holder.buildInCompMeshColliderInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompMeshColliderInfos)
                {
                    if (item.meshName != "")
                    {
                        Mesh mesh = null;
                        LoadMesh(item.meshName, (m) =>
                        {
                            mesh = m;
                            item.concreteCompMeshCollider.sharedMesh = mesh;

                            //add mesh ref 
                            MeshRes meshRes = this.GetCachedRes(ABType.Mesh, item.meshName) as MeshRes;
                            meshRes.AddRefTrs(item.tran);
                        }, sync);
                    }
                }
            }
            //-------------->Ext ImageSequence
            if (holder.extCompImageSequenceInfos != null && holder.extCompImageSequenceInfos.Count > 0)
            {
                foreach (var item in holder.extCompImageSequenceInfos)
                {
                    Transform childTarget = item.tran;
                    if (item.refSprites != null && item.refSprites.Count > 0)
                    {
                        ImageSequence imageSequence = childTarget.GetComponent<ImageSequence>();
                        for (int i = 0; i < item.refSprites.Count; i++)
                        {
                            LoadSpriteToTarget(imageSequence, item.refSprites[i].atlasName, item.refSprites[i].spriteName, sync, i);
                        }
                    }

                }
            }

            //------------->Ext Material Texture Sequence
            if (holder.extCompMaterialTextureSequenceInfos != null && holder.extCompMaterialTextureSequenceInfos.Count > 0)
            {
                foreach (var item in holder.extCompMaterialTextureSequenceInfos)
                {
                    Transform childTarget = item.tran;
                    if (item.refTextures != null && item.refTextures.Count > 0)
                    {
                        MaterialTextureSequence matTextureSeq = childTarget.GetComponent<MaterialTextureSequence>();
                        for (int i = 0; i < item.refTextures.Count; i++)
                        {
                            LoadTextureToMatTextureSeq(matTextureSeq, item.refTextures[i].texName, i, sync);
                        }
                    }
                }
            }

            //-------------->Ext SpriteSequence
            if (holder.extCompSpriteSequenceInfos != null && holder.extCompSpriteSequenceInfos.Count > 0)
            {
                foreach (var item in holder.extCompSpriteSequenceInfos)
                {
                    Transform childTarget = item.tran;
                    if (item.refSprites != null && item.refSprites.Count > 0)
                    {
                        SpriteSequence spriteSequence = childTarget.GetComponent<SpriteSequence>();
                        for (int i = 0; i < item.refSprites.Count; i++)
                        {
                            LoadSpriteToTarget(spriteSequence, item.refSprites[i].atlasName, item.refSprites[i].spriteName, sync, i);
                        }
                    }
                }
            }
            //-------------->Ext SwapSprite
            if (holder.extCompSwapSpriteInfos != null && holder.extCompSwapSpriteInfos.Count > 0)
            {

                foreach (var item in holder.extCompSwapSpriteInfos)
                {
                    Transform childTarget = item.tran;
                    if (item.refSprites != null && item.refSprites.Count > 0)
                    {
                        SwapSprite swapSprite = childTarget.GetComponent<SwapSprite>();
                        for (int i = 0; i < item.refSprites.Count; i++)
                        {
                            LoadSpriteToTarget(swapSprite, item.refSprites[i].atlasName, item.refSprites[i].spriteName, sync, i);
                        }
                    }
                }
            }
            //-------------->Ext Switch2Button
            if (holder.extCompSwitch2ButtonInfos != null && holder.extCompSwitch2ButtonInfos.Count > 0)
            {

                foreach (var item in holder.extCompSwitch2ButtonInfos)
                {
                    Transform childTarget = item.tran;
                    if (item.refSprites != null && item.refSprites.Count > 0)
                    {
                        Switch2Button switch2Btn = childTarget.GetComponent<Switch2Button>();
                        for (int i = 0; i < item.refSprites.Count; i++)
                        {
                            LoadSpriteToTarget(switch2Btn, item.refSprites[i].atlasName, item.refSprites[i].spriteName, sync, i);
                        }
                    }
                }
            }
            //-------------->Ext ShaderMeshAnimator
            if (holder.extCompShaderMeshAnimationInfos != null && holder.extCompShaderMeshAnimationInfos.Count > 0)
            {
                foreach (var item in holder.extCompShaderMeshAnimationInfos)
                {
                    Transform childTarget = item.tran;
                    if (!string.IsNullOrEmpty(item.defaultAnimation) && item.animations.Count > 0)
                    {
                        ShaderMeshAnimator animator = childTarget.GetComponent<ShaderMeshAnimator>();
                        //defaultAnimation
                        LoadObject(item.defaultAnimation, (obj) =>
                        {
                            animator.defaultMeshAnimation = obj as ShaderMeshAnimation;

                            //add ShaderMeshAnim ref 
                            ObjectRes objectRes = this.GetCachedRes(ABType.Object, item.defaultAnimation) as ObjectRes;
                            objectRes.AddRefTrs(childTarget);

                        }, sync);
                        //animations
                        for (int i = 0; i < item.animations.Count; i++)
                        {
                            LoadObject(item.animations[i], (obj) =>
                            {
                                animator.meshAnimations[i] = obj as ShaderMeshAnimation;

                                //add ShaderMeshAnim ref 
                                ObjectRes objectRes = this.GetCachedRes(ABType.Object, item.animations[i]) as ObjectRes;
                                objectRes.AddRefTrs(childTarget);
                            }, sync);
                        }
                    }
                }
            }
            //---------------->Lightmap
            //if (holder.lightmapInfo != null && holder.lightmapInfo.lightmapColors != null && holder.lightmapInfo.lightmapColors.Count > 0 && holder.lightmapInfo.lightmapRenders != null && holder.lightmapInfo.lightmapRenders.Count > 0)
            //{
            //    ABScene.LoadLightmaps(holder.lightmapInfo, sync);
            //}
            //handle at ABScene.cs

            holder.finishedSet = true;
        }


        public void LoadTexture(string texName, Action<Texture> callback, bool sync)
        {
            loadTexture(texName, callback, sync);
        }

        void loadTexture(string texName, Action<Texture> callback, bool sync)
        {
            Texture tex = null;
            var cache = this.GetCachedRes(ABType.Texture, texName);
            if (cache == null)
            {
                //Debug.Log($"get texture:{texName}  from ab");
                ABTexture.Load(texName, callback, sync);
            }
            else
            {
                //Debug.Log($"get texture:{texName}  from cache");
                tex = cache.GetRes<Texture>(texName);
                callback?.Invoke(tex);

            }
        }

        public void LoadMatWithCheck(string name, Action<Material> callback, Transform refTran)
        {
            if (name == "")
            {
                Debug.LogError("error matName is nil:" + refTran.GetHierarchy());
            }
            this.LoadMat(name, callback);
        }

        public void LoadMat(string name, Action<Material> callback)
        {
            loadMat(name, callback);
        }

        private void loadMat(string matName, Action<Material> callback)
        {
            Material mat = null;
            var cache = this.GetCachedRes(ABType.Material, matName);
            if (cache == null)
            {
                ABMaterial.Load(matName, callback, true);
            }
            else
            {
                mat = cache.GetRes<Material>(matName);
                callback?.Invoke(mat);
            }
        }
        public void LoadMesh(string meshName, Action<Mesh> callback, bool sync)
        {
            this.loadMesh(meshName, callback, sync);
        }
        void loadMesh(string meshName, Action<Mesh> callback, bool sync)
        {
            Mesh mesh = null;
            var cache = this.GetCachedRes(ABType.Mesh, meshName);
            if (cache == null)
            {
                ABMesh.Load(meshName, callback, sync);
            }
            else
            {
                mesh = cache.GetRes<Mesh>(meshName);
                callback?.Invoke(mesh);
            }
        }

        public void LoadSprite(string atlasName, string spriteName, Action<Sprite> callback, bool sync)
        {
            loadSprite(atlasName, spriteName, callback, sync);
        }
        void loadSprite(string atlasName, string spriteName, Action<Sprite> callback, bool sync)
        {
            Sprite target = null;
            var cache = this.GetCachedRes(ABType.Sprite, atlasName);
            if (cache == null)
            {
                ABSprite.Load(atlasName, spriteName, callback, sync);
            }
            else
            {
                target = cache.GetRes<Sprite>(spriteName);
                callback?.Invoke(target);
            }
        }

        public void LoadTextureToMat(Transform targetTrs, string texName, string shaderPropName, Material mat, bool sync, Action callback)
        {
            Action<Texture> handleTextureAfterLoad = (tex) =>
            {
                var oldTex = mat.GetTexture(shaderPropName);
                string oldTexName = null;
                if (oldTex != null)
                {
                    oldTexName = oldTex.name;
                }

                mat.SetTexture(shaderPropName, tex);

                //remove old ref
                if (string.IsNullOrEmpty(oldTexName) == false && oldTexName != texName)
                {
                    var oldRes = this.GetCachedRes(ABType.Texture, oldTexName);
                    if (oldRes != null && oldTexName != texName)
                    {
                        oldRes.RemoveRefTrs(targetTrs);
                    }
                }

                //add ref
                if (targetTrs != null)
                {
                    ImgRes imgRes = this.GetCachedRes(ABType.Texture, texName) as ImgRes;
                    imgRes.AddRefTrs(targetTrs);
                }

                callback?.Invoke();
            };

            loadTexture(texName, handleTextureAfterLoad, sync);

        }

        public void LoadTextureToMatTextureSeq(MaterialTextureSequence ts, string texName, int index, bool sync)
        {
            string oldTexName = "";
            Texture oldTex = ts.textures[index];
            if (oldTex != null)
            {
                oldTexName = oldTex.name;
            }


            loadTexture(texName, (tex) =>
            {
                ts.textures[index] = tex;

                //remove old ref
                if (string.IsNullOrEmpty(oldTexName) == false && oldTexName != texName)
                {
                    var oldRes = this.GetCachedRes(ABType.Texture, oldTexName);
                    if (oldRes != null && oldTexName != texName)
                    {
                        oldRes.RemoveRefTrs(ts.transform);
                    }
                }


                //add ref
                TextureRes texRes = this.GetCachedRes(ABType.Texture, texName) as TextureRes;
                texRes.AddRefTrs(ts.transform);
            },
            sync);

        }


        public void LoadSpriteToTarget<T>(T target, string atlasName, string spriteName, bool sync, params object[] objs) where T : UnityEngine.Component
        {
            Action<Sprite> handleSpriteAfterLoad = (s) =>
            {
                Transform trs = target.transform;
                string oldAtlasName = null;

                Type ty = target.GetType();
                if (ty.Equals(typeof(SpriteRenderer)))
                {
                    SpriteRenderer sr = (target as SpriteRenderer);
                    oldAtlasName = sr.sprite != null ? sr.sprite.texture.name : "";
                    sr.sprite = s;
                }
                else if (ty.Equals(typeof(Image)))
                {
                    Image img = (target as Image);
                    oldAtlasName = img.sprite != null ? img.sprite.texture.name : "";
                    img.sprite = s;
                }
                else if (ty.Equals(typeof(SpriteSequence)))
                {
                    SpriteSequence ss = target as SpriteSequence;
                    int index = int.Parse(objs[0].ToString());
                    oldAtlasName = ss.sprites[index] != null ? ss.sprites[index].texture.name : "";
                    ss.sprites[index] = s;
                }
                else if (ty.Equals(typeof(ImageSequence)))
                {
                    ImageSequence imgS = target as ImageSequence;
                    int index = int.Parse(objs[0].ToString());
                    oldAtlasName = imgS.sprites[index] != null ? imgS.sprites[index].texture.name : "";
                    imgS.sprites[index] = s;
                }
                else if (ty.Equals(typeof(SwapSprite)))
                {
                    SwapSprite ss = target as SwapSprite;
                    int index = int.Parse(objs[0].ToString());
                    oldAtlasName = ss.sprites[index] != null ? ss.sprites[index].texture.name : "";
                    ss.sprites[index] = s;

                }
                else if (ty.Equals(typeof(Switch2Button)))
                {
                    Switch2Button s2b = target as Switch2Button;
                    int index = int.Parse(objs[0].ToString());
                    oldAtlasName = s2b.switchSprites[index] != null ? s2b.switchSprites[index].texture.name : "";
                    s2b.switchSprites[index] = s;

                }

                else
                {
                    DebugExt.LogE("type not suit!!");
                }


                //remove old ref
                if (string.IsNullOrEmpty(oldAtlasName) == false)
                {
                    var oldRes = this.GetCachedRes(ABType.Sprite, oldAtlasName);
                    if (oldRes != null && oldAtlasName != atlasName)
                    {
                        oldRes.RemoveRefTrs(trs);
                    }
                }

                //add ref
                ImgRes imgRes = this.GetCachedRes(ABType.Sprite, atlasName) as ImgRes;
                imgRes.AddRefTrs(trs);
            };

            loadSprite(atlasName, spriteName, handleSpriteAfterLoad, sync);

        }



        public void LoadAudioClip(string name, Action<UnityEngine.AudioClip> callback, bool sync)
        {
            AudioClip clip = null;
            var cache = this.GetCachedRes(ABType.Audio, name);
            if (cache == null || cache.resObj == null)
            {
                ABAudio.Load(name, callback, sync);
            }
            else
            {
                clip = cache.GetRes<AudioClip>(name);
                callback?.Invoke(clip);
            }
        }


        public void LoadByte(string name, Action<TextAsset> callback, bool sync)
        {
            TextAsset asset = null;
            var cache = this.GetCachedRes(ABType.Byte, name);
            if (cache == null || cache.resObj == null)
            {
                ABByte.Load(name, callback, sync);
            }
            else
            {
                asset = cache.GetRes<TextAsset>(name);
                callback?.Invoke(asset);
            }
        }

        public void LoadObject(string name, Action<UnityEngine.Object> callback, bool sync)
        {
            var cache = this.GetCachedRes(ABType.Object, name);
            if (cache == null || cache.resObj == null)
            {
                ABObject.Load(name, callback, sync);
            }
            else
            {
                UnityEngine.Object asset = cache.GetRes<UnityEngine.Object>(name);
                callback?.Invoke(asset);
            }
        }

        public void LoadLogic(string name, Action<TextAsset[]> callback)
        {
            ABLogic.LoadAll(name, callback);
        }



        public void UnpackLogicABToMap(string logicABName)
        {
#if XLua
            LuaResLoader.Instance.LoadScriptBundle(logicABName);
#endif
        }

        public override void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EventID.OnABResLoaded, onABResLoaded);
            EventDispatcher.Instance.RemoveListener(EventID.OnRootObjDestroy, onRootObjDestroy);
            EventDispatcher.Instance.RemoveListener(EventID.OnChildObjDestroy, onChildObjDestroy);
            EventDispatcher.Instance.RemoveListener(EventID.OnGameObjectInstantiate, onGameObjectInstantiate);
        }
    }
}