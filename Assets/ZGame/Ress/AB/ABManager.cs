using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ZGame.Event;
using ZGame.Ress.AB.Holder;
using ZGame.Ress.Info;
using ZGame.UGUIExtention;


namespace ZGame.Ress.AB
{
    public class ABManagerMono : SingletonMonoBehaviour<ABManagerMono>
    {
        public List<SpriteRes> spriteCacheList = new List<SpriteRes>();
        public List<TextureRes> textureCacheList = new List<TextureRes>();
        public List<WindowRes> windowCacheList = new List<WindowRes>();
        public List<OtherPrefabRes> otherPrefabCacheList = new List<OtherPrefabRes>();
        public List<EffectRes> effectCacheList = new List<EffectRes>();
        public List<AudioRes> audioCacheList = new List<AudioRes>();

        public Dictionary<string, InstanceCache> instanceCacheDic = new Dictionary<string, InstanceCache>();

        void Update()
        {
            foreach (var key in instanceCacheDic.Keys)
            {
                instanceCacheDic[key].Update();
            }
        }

        public GameObject GetInstanceCache(string name)
        {
            if (instanceCacheDic.ContainsKey(name))
            {
                return instanceCacheDic[name].Get();
            }
            return null;
        }

        public void ReleaseInstance(string name, GameObject obj)
        {
            InstanceCache cache = null;
            if (instanceCacheDic.ContainsKey(name))
            {
                cache = instanceCacheDic[name];
            }
            else
            {
                cache = new InstanceCache(name, this.transform);
                instanceCacheDic.Add(name, cache);
            }

            if (null != cache)
            {
                cache.Release(obj);
            }
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
                        //Debug.LogError("------------>" + objs[i].name + ", type:" + objs[i].GetType().ToString());
                        //if (objs[i].name == "arial SDF")
                        //{
                        //    arialSDF = objs[i] as TMP_FontAsset;
                        //    var id = arialSDF.creationSettings.sourceFontFileGUID;
                        //    var name = arialSDF.creationSettings.sourceFontFileName;


                        //    Debug.LogError("@@@@@ arial sdk sourcefont guid:" + id + ", name:" + name);
                        //}

                        //if (objs[i].name == "arial")
                        //{
                        //    originArialFont = objs[i] as Font;
                        //}



                        if (objs[i].name == "ShaderVariants")
                        {
                            //Debug.Log("warmup shader variants");
                            (objs[i] as ShaderVariantCollection).WarmUp();//WarmUp() all variants,in case of stuck while to complie variant at run time
                        }
                    }


                    EventDispatcher.Instance.AddListener(EventID.OnABResLoaded, onABResLoaded);
                    EventDispatcher.Instance.AddListener(EventID.OnRootObjDestroy, onRootObjDestroy);
                    EventDispatcher.Instance.AddListener(EventID.OnChildObjDestroy, onChildObjDestroy);

                    EventDispatcher.Instance.AddListener(EventID.OnGameObjectInstantiate, onGameObjectInstantiate);
                    isInit = true;

                }, true);// load common ab,we can not call ab.Unload(false) , otherwise shader will miss on mobile devices!



            }
        }


        public void RemoveRes(ABType abType, Res res)
        {
            switch (abType)
            {

                case ABType.Sprite:
                    ABManagerMono.Instance.spriteCacheList.Remove(res as SpriteRes);
                    break;
                case ABType.Texture:
                    ABManagerMono.Instance.textureCacheList.Remove(res as TextureRes);
                    break;
                case ABType.Effect:
                    ABManagerMono.Instance.effectCacheList.Remove(res as EffectRes);
                    break;
                case ABType.Window:
                    ABManagerMono.Instance.windowCacheList.Remove(res as WindowRes);
                    break;
                case ABType.OtherPrefab:
                    ABManagerMono.Instance.otherPrefabCacheList.Remove(res as OtherPrefabRes);
                    break;
                case ABType.Audio:
                    ABManagerMono.Instance.audioCacheList.Remove(res as AudioRes);
                    break;

            }
        }

        public void RemoveRes(ABType abType, GameObject targetObj)
        {
            switch (abType)
            {
                case ABType.Effect:
                    EffectRes tmpE = null;
                    for (int i = 0; i < ABManagerMono.Instance.effectCacheList.Count; i++)
                    {
                        tmpE = ABManagerMono.Instance.effectCacheList[i];
                        if (tmpE.resObj == targetObj)
                        {
                            ABManagerMono.Instance.effectCacheList.Remove(tmpE);
                            return;
                        }
                    }
                    break;
                case ABType.Window:
                    WindowRes tmpW = null;
                    for (int i = 0; i < ABManagerMono.Instance.windowCacheList.Count; i++)
                    {
                        tmpW = ABManagerMono.Instance.windowCacheList[i];
                        if (tmpW.resObj == targetObj)
                        {
                            ABManagerMono.Instance.windowCacheList.Remove(tmpW);
                            return;
                        }
                    }
                    break;
                case ABType.OtherPrefab:
                    OtherPrefabRes tmpOP;
                    for (int i = 0; i < ABManagerMono.Instance.otherPrefabCacheList.Count; i++)
                    {
                        tmpOP = ABManagerMono.Instance.otherPrefabCacheList[i];
                        if (tmpOP.resObj == targetObj)
                        {
                            ABManagerMono.Instance.otherPrefabCacheList.Remove(tmpOP);
                            return;
                        }
                    }
                    break;
                default:
                    Debug.Log("not supported yet, abType:" + abType);
                    break;

            }
        }

        public Res GetRes(ABType abType, string name)
        {

            switch (abType)
            {
                case ABType.Sprite:
                    for (int i = 0; i < ABManagerMono.Instance.spriteCacheList.Count; i++)
                    {
                        if (ABManagerMono.Instance.spriteCacheList[i].resName == name)
                        {
                            return ABManagerMono.Instance.spriteCacheList[i];
                        }
                    }
                    break;
                case ABType.Texture:
                    for (int i = 0; i < ABManagerMono.Instance.textureCacheList.Count; i++)
                    {
                        if (ABManagerMono.Instance.textureCacheList[i].resName == name)
                        {
                            return ABManagerMono.Instance.textureCacheList[i];
                        }
                    }
                    break;
                case ABType.Effect:
                    for (int i = 0; i < ABManagerMono.Instance.effectCacheList.Count; i++)
                    {
                        if (ABManagerMono.Instance.effectCacheList[i].resName == name)
                        {
                            return ABManagerMono.Instance.effectCacheList[i];
                        }
                    }
                    break;
                case ABType.Window:
                    for (int i = 0; i < ABManagerMono.Instance.windowCacheList.Count; i++)
                    {
                        if (ABManagerMono.Instance.windowCacheList[i].resName == name)
                        {
                            return ABManagerMono.Instance.windowCacheList[i];
                        }
                    }
                    break;
                case ABType.OtherPrefab:
                    for (int i = 0; i < ABManagerMono.Instance.otherPrefabCacheList.Count; i++)
                    {
                        if (ABManagerMono.Instance.otherPrefabCacheList[i].resName == name)
                        {
                            return ABManagerMono.Instance.otherPrefabCacheList[i];
                        }
                    }
                    break;

                case ABType.Audio:
                    for (int i = 0; i < ABManagerMono.Instance.audioCacheList.Count; i++)
                    {
                        if (ABManagerMono.Instance.audioCacheList[i].resName == name)
                        {
                            return ABManagerMono.Instance.audioCacheList[i];
                        }
                    }
                    break;
            }
            return null;
        }


        private void onGameObjectInstantiate(string evtId, object[] paras)
        {
            GameObject obj = paras[0] as GameObject;

            this.fillReferenceOfDynamicCompInfoHolder(obj);
            this.AddDestroyNotice(obj);
        }

        private void onChildObjDestroy(string evtId, object[] paras)
        {
            GameObject obj = paras[0] as GameObject;
            removeSpriteReference(obj.transform);
            removeTextureReference(obj.transform);

        }

        void removeSpriteReference(Transform trans)
        {
            //TODO:loop check cost too much.Need change!  
            SpriteRes sr;
            for (int i = ABManagerMono.Instance.spriteCacheList.Count - 1; i >= 0; i--)
            {
                sr = ABManagerMono.Instance.spriteCacheList[i];
                if (sr.CheckRefTrs(trans))
                {
                    sr.RemoveRefTrs(trans);
                }
            }
        }
        void removeTextureReference(Transform trans)
        {
            //TODO:loop check cost too much.Need change!  
            TextureRes tr;
            for (int j = ABManagerMono.Instance.textureCacheList.Count - 1; j >= 0; j--)
            {
                tr = ABManagerMono.Instance.textureCacheList[j];
                if (tr.CheckRefTrs(trans))
                {
                    tr.RemoveRefTrs(trans);
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
                RemoveRes(abType, obj);


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
                ABManagerMono.Instance.windowCacheList.Add(wRes);

                GameObject obj = wRes.resObj as GameObject;
                FillReferenceOfRootCompInfoHolder(obj, sync);
                this.AddDestroyNotice(obj);

                wRes.AddRefTrs(obj.transform);
            }
            else if (res is EffectRes)
            {
                EffectRes effRes = res as EffectRes;
                ABManagerMono.Instance.effectCacheList.Add(effRes);

                GameObject obj = effRes.resObj as GameObject;
                FillReferenceOfRootCompInfoHolder(obj, sync);
                this.AddDestroyNotice(obj);

                effRes.AddRefTrs(obj.transform);
            }
            else if (res is OtherPrefabRes)
            {
                OtherPrefabRes opRes = res as OtherPrefabRes;
                ABManagerMono.Instance.otherPrefabCacheList.Add(opRes);

                GameObject obj = opRes.resObj as GameObject;
                FillReferenceOfRootCompInfoHolder(obj, sync);
                this.AddDestroyNotice(obj);

                opRes.AddRefTrs(obj.transform);
            }
            else if (res is SpriteRes)
            {
                SpriteRes sRes = res as SpriteRes;
                if (GetRes(ABType.Sprite, res.resName) == null)
                {
                    ABManagerMono.Instance.spriteCacheList.Add(sRes);
                }
            }
            else if (res is TextureRes)
            {
                TextureRes tRes = res as TextureRes;
                if (GetRes(ABType.Texture, res.resName) == null)
                {
                    ABManagerMono.Instance.textureCacheList.Add(tRes);
                }
            }
            else if (res is AudioRes)
            {
                AudioRes aRes = res as AudioRes;
                if (GetRes(ABType.Audio, res.resName) == null)
                {
                    ABManagerMono.Instance.audioCacheList.Add(aRes);
                }
            }
            else
            {
                Debug.Log("onABResLoaded TODO::::" + res.resName);
            }
        }

        private Res getCachedAbRes(ABType abType, string name)
        {
            switch (abType)
            {
                case ABType.Sprite:
                    for (int i = 0; i < ABManagerMono.Instance.spriteCacheList.Count; i++)
                    {
                        if (ABManagerMono.Instance.spriteCacheList[i].resName == name)
                        {
                            return ABManagerMono.Instance.spriteCacheList[i];
                        }
                    }
                    break;
                case ABType.Texture:
                    for (int i = 0; i < ABManagerMono.Instance.textureCacheList.Count; i++)
                    {
                        if (ABManagerMono.Instance.textureCacheList[i].resName == name)
                        {
                            return ABManagerMono.Instance.textureCacheList[i];
                        }
                    }
                    break;
                case ABType.Effect:
                    for (int i = 0; i < ABManagerMono.Instance.effectCacheList.Count; i++)
                    {
                        if (ABManagerMono.Instance.effectCacheList[i].resName == name)
                        {
                            return ABManagerMono.Instance.effectCacheList[i];
                        }
                    }
                    break;
                case ABType.Window:
                    for (int i = 0; i < ABManagerMono.Instance.windowCacheList.Count; i++)
                    {
                        if (ABManagerMono.Instance.windowCacheList[i].resName == name)
                        {
                            return ABManagerMono.Instance.windowCacheList[i];
                        }
                    }
                    break;
                case ABType.OtherPrefab:
                    for (int i = 0; i < ABManagerMono.Instance.otherPrefabCacheList.Count; i++)
                    {
                        if (ABManagerMono.Instance.otherPrefabCacheList[i].resName == name)
                        {
                            return ABManagerMono.Instance.otherPrefabCacheList[i];
                        }
                    }
                    break;

                case ABType.Audio:
                    for (int i = 0; i < ABManagerMono.Instance.audioCacheList.Count; i++)
                    {
                        if (ABManagerMono.Instance.audioCacheList[i].resName == name)
                        {
                            return ABManagerMono.Instance.audioCacheList[i];
                        }
                    }
                    break;
            }

            return null;
        }


        public void loadWindow(string name, Action<UnityEngine.Object> callback, bool sync)
        {
            var cache = getCachedAbRes(ABType.Window, name);
            if (cache == null || cache.resObj == null)//resObj is null also can make sure need reload.
            {
                ABPrefab.Load(name, ABType.Window, callback, sync);
            }
            else
            {
                DebugExt.Log("From AB cache get window:" + name);
                if (callback != null)
                {
                    callback(cache.resObj);
                }
            }
        }


        public void LoadScene(string name, LoadSceneMode loadSceneMode, Action<UnityEngine.Object> sceneLoadedCallback, Action lightmapAttachedCallback, bool sync)
        {
           // Debug.Log("To load Scene:" + name);
            MapLoader.Instance.Dispose();
            //TODO: cache ?  scene has not cache now!!! 
            ABScene.Load(name, loadSceneMode, sceneLoadedCallback, lightmapAttachedCallback, sync);
        }



        public void UnloadScene(string name, Action callback)
        {
            ABScene.Unload(name, callback);
        }


        public void LoadLargeScene(string name, LoadSceneMode loadSceneMode, Vector3 initPos, Action<UnityEngine.Object> sceneLoadedCallback, Action initChunksLoadedCallback, bool sync)
        {
            MapLoader.Instance.Dispose();
            ABLargeScene.Load(name, loadSceneMode, initPos, sceneLoadedCallback, initChunksLoadedCallback, sync);
        }
        public void UnloadLargeScene(string name, Action callback)
        {
            ABLargeScene.Unload(name, callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="reUse">true means you can reuse it,that is also indicate there is only one in your memory.
        /// false means you can not reuse it,each time you call LoadEffect,it will creat a new one.
        /// If you want to build a memory pool for it,in your pool script you can initial pool objs by LoadEffect and set reUse=false.
        /// </param>
        /// <returns></returns>
        public void LoadEffect(string name, Action<UnityEngine.Object> callback, bool sync, bool reUse = true)
        {
            GameObject effect = null;

            var cache = getCachedAbRes(ABType.Effect, name);
            if (cache == null || cache.resObj == null)
            {
                ABPrefab.Load(name, ABType.Effect, callback, sync);
            }
            else
            {
                if (reUse)
                {
                    effect = cache.GetRes<GameObject>(name);
                    callback?.Invoke(effect);
                }
                else
                {
                    ABPrefab.Load(name, ABType.Effect, callback, sync);
                }
            }
        }

        public void LoadOtherPrefab(string name, Action<UnityEngine.Object> callback, bool sync, bool reUse = true)
        {
            var obj = ABManagerMono.Instance.GetInstanceCache(name);
            if (obj)
            {
                callback.Invoke(obj);
            }
            else
            {
                GameObject op = null;

                var cache = getCachedAbRes(ABType.OtherPrefab, name);
                if (cache == null || cache.resObj == null)
                {
                    ABPrefab.Load(name, ABType.OtherPrefab, callback, sync);
                }
                else
                {
                    if (reUse)
                    {
                        op = cache.GetRes<GameObject>(name);
                        callback.Invoke(op);
                    }
                    else
                    {
                        ABPrefab.Load(name, ABType.OtherPrefab, callback, sync);
                    }
                }
            }
        }

        public void ReleasePrefab(string name, GameObject obj)
        {
            ABManagerMono.Instance.ReleaseInstance(name, obj);
        }


        public void LoadByte()
        {

        }



        public void ResetEditorShader(Transform tran, Material mat, string shaderName)
        {
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
                    if (item.mat != null && item.refSprites != null && item.refSprites.Count > 0)
                    {
                        ImgRes imgRes = getCachedAbRes(ABType.Sprite, item.refSprites[0].atlasName) as ImgRes;
                        if (imgRes != null)
                        {
                            imgRes.AddRefTrs(childTarget);
                        }
                        else
                        {
                            DebugExt.LogE("error, please check Image");
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
                    if (item.mat != null && item.refSprites != null && item.refSprites.Count > 0)
                    {
                        ImgRes imgRes = getCachedAbRes(ABType.Sprite, item.refSprites[0].atlasName) as ImgRes;
                        if (imgRes != null)
                        {
                            imgRes.AddRefTrs(childTarget);
                        }
                        else
                        {
                            DebugExt.LogE("error, please check SpriteRenderer");
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
                        ImgRes imgRes = getCachedAbRes(ABType.Texture, item.refTextures[k].texName) as ImgRes;
                        if (imgRes != null)
                        {
                            imgRes.AddRefTrs(childTarget);
                        }
                        else
                        {
                            DebugExt.LogE("error, please check Renderer");
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
                            ImgRes imgRes = getCachedAbRes(ABType.Sprite, item.refSprites[i].atlasName) as ImgRes;
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
                            ImgRes imgRes = getCachedAbRes(ABType.Sprite, item.refSprites[i].atlasName) as ImgRes;
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
                            ImgRes imgRes = getCachedAbRes(ABType.Sprite, item.refSprites[i].atlasName) as ImgRes;
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
                    if (item.mat != null && item.refSprites != null && item.refSprites.Count > 0)
                    {
                        LoadSpriteToTarget<Image>(childTarget.GetComponent<Image>(), item.refSprites[0].atlasName, item.refSprites[0].spriteName, sync);

#if UNITY_EDITOR
                        ResetEditorShader(childTarget, item.mat, item.shaderName);
#endif
                    }
                }
            }
            //-------------->BuildIn SpriteRenderer
            if (holder.buildInCompSpriteRendererInfos != null && holder.buildInCompSpriteRendererInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompSpriteRendererInfos)
                {
                    Transform childTarget = item.tran;
                    if (item.mat != null && item.refSprites != null && item.refSprites.Count > 0)
                    {
                        LoadSpriteToTarget<SpriteRenderer>(childTarget.GetComponent<SpriteRenderer>(), item.refSprites[0].atlasName,
                                               item.refSprites[0].spriteName, sync);
#if UNITY_EDITOR
                        ResetEditorShader(childTarget, item.mat, item.shaderName);
#endif
                    }
                }
            }
            //-------------->BuildIn Text
            if (holder.buildInCompTextInfos != null && holder.buildInCompTextInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompTextInfos)
                {
                    Transform childTarget = item.tran;
                    if (item.mat != null)
                    {
#if UNITY_EDITOR
                        ResetEditorShader(childTarget, item.mat, item.shaderName);
#endif
                    }
                }
            }
            //-------->BuildIn TextMeshProUGUI
            if (holder.buildInCompTextMeshProUGUIInfos != null && holder.buildInCompTextMeshProUGUIInfos.Count > 0)
            {
                foreach (var item in holder.buildInCompTextMeshProUGUIInfos)
                {
                    Transform childTarget = item.tran;
                    if (item.mat != null)
                    {
#if UNITY_EDITOR
                        ResetEditorShader(childTarget, item.mat, item.shaderName);
#endif
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
                        Transform childTarget = item.tran;
                        for (int k = 0; k < item.refTextures.Count; k++)
                        {
                            if (item.mat != null && item.refTextures != null && item.refTextures.Count > 0)
                            {

                                LoadTextureToMat(childTarget, item.refTextures[k].texName, item.refTextures[k].shaderProp, item.mat, sync, null);
                            }

                        }

#if UNITY_EDITOR
                        ResetEditorShader(childTarget, item.mat, item.shaderName);
#endif
                    }
                    else
                    {
                        BuildInCompRenderInfoSyncOperator.Instance.Add(item);
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
            var cache = getCachedAbRes(ABType.Texture, texName);
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

        public void LoadSprite(string atlasName, string spriteName, Action<Sprite> callback, bool sync)
        {
            loadSprite(atlasName, spriteName, callback, sync);
        }
        void loadSprite(string atlasName, string spriteName, Action<Sprite> callback, bool sync)
        {
            Sprite target = null;
            var cache = getCachedAbRes(ABType.Sprite, atlasName);
            if (cache == null)
            {
                ABSprite.Load(atlasName, spriteName, callback, sync);
            }
            else
            {
                target = cache.GetRes<Sprite>(spriteName);

                //Debug.Log($"get atlas:{atlasName}, sprite:{spriteName} from cache");
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
                    var oldRes = getCachedAbRes(ABType.Texture, oldTexName);
                    if (oldRes != null && oldTexName != texName)
                    {
                        oldRes.RemoveRefTrs(targetTrs);
                    }
                }

                //add ref
                if (targetTrs != null)
                {
                    ImgRes imgRes = getCachedAbRes(ABType.Texture, texName) as ImgRes;
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
                    var oldRes = getCachedAbRes(ABType.Texture, oldTexName);
                    if (oldRes != null && oldTexName != texName)
                    {
                        oldRes.RemoveRefTrs(ts.transform);
                    }
                }


                //add ref
                TextureRes texRes = getCachedAbRes(ABType.Texture, texName) as TextureRes;
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
                    var oldRes = getCachedAbRes(ABType.Sprite, oldAtlasName);
                    if (oldRes != null && oldAtlasName != atlasName)
                    {
                        oldRes.RemoveRefTrs(trs);
                    }
                }

                //add ref
                ImgRes imgRes = getCachedAbRes(ABType.Sprite, atlasName) as ImgRes;
                imgRes.AddRefTrs(trs);
            };

            loadSprite(atlasName, spriteName, handleSpriteAfterLoad, sync);

        }



        public void LoadAudioClip(string name, Action<UnityEngine.AudioClip> callback, bool sync)
        {
            AudioClip clip = null;
            var cache = getCachedAbRes(ABType.Audio, name);
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
            var cache = getCachedAbRes(ABType.Byte, name);
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