using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using ZGame.Event;
using ZGame.Ress.AB.Holder;
using ZGame.UGUIExtention;

namespace ZGame.Ress.AB
{
    public class ABManagerMono : SingletonMonoBehaviour<ABManagerMono>
    {
        //以下List为辅助inspector显示的
        public List<WindowRes> windowResList = new List<WindowRes>();
        public List<EffectRes> effectResList = new List<EffectRes>();
        public List<OtherPrefabRes> opResList = new List<OtherPrefabRes>();

        public List<SpriteRes> spriteResList = new List<SpriteRes>();
        public List<TextureRes> texResList = new List<TextureRes>();
        public List<AudioRes> audioResList = new List<AudioRes>();
    }


    public class ABManager : Singleton<ABManager>
    {

        bool isInit = false;

        public ABManager()
        {
            if (!isInit)
            {
                Debug.LogError("ab load: common");
                AssetBundle commonAB = AB.Load("common");
                commonAB.LoadAllAssets();

                EventDispatcher.Instance.AddListener(EventID.OnABResLoaded, onABResLoaded);
                EventDispatcher.Instance.AddListener(EventID.OnRootObjDestroy, onRootObjDestroy);
                EventDispatcher.Instance.AddListener(EventID.OnChildObjDestroy, onChildObjDestroy);

                EventDispatcher.Instance.AddListener(EventID.OnGameObjectInstantiate, onGameObjectInstantiate);
                isInit = true;
            }
        }



        /// <summary>
        /// Key为AB类型，Value为该类型下所有的资源（Value中的Key为资源名，Value中的Value为具体资源信息）
        /// TODO:美术端需要提供工具保证每种类型内的资源，名字要唯一
        /// </summary>
        Dictionary<ABType, Dictionary<string, Res>> resCacheDic = null;

        public Dictionary<ABType, Dictionary<string, Res>> ResCacheDic
        {
            get
            {
                if (resCacheDic == null)
                {
                    resCacheDic = new Dictionary<ABType, Dictionary<string, Res>>();

                    resCacheDic[ABType.Sprite] = new Dictionary<string, Res>();
                    resCacheDic[ABType.Texture] = new Dictionary<string, Res>();

                    resCacheDic[ABType.Effect] = new Dictionary<string, Res>();
                    resCacheDic[ABType.Window] = new Dictionary<string, Res>();
                    resCacheDic[ABType.OtherPrefab] = new Dictionary<string, Res>();


                    resCacheDic[ABType.Scene] = new Dictionary<string, Res>();

                    resCacheDic[ABType.Audio] = new Dictionary<string, Res>();
                    resCacheDic[ABType.Video] = new Dictionary<string, Res>();
                    resCacheDic[ABType.Model] = new Dictionary<string, Res>();
                    resCacheDic[ABType.Material] = new Dictionary<string, Res>();


                }

                return resCacheDic;
            }
        }


        public void RemoveRes(ABType abType, string name)
        {
            if (!ResCacheDic.ContainsKey(abType))
            {
                return;
            }
            if (ResCacheDic[abType].ContainsKey(name))
            {
                ResCacheDic[abType].Remove(name);
            }
        }


        public Res GetRes(ABType abType, string name)
        {
            Res res = null;
            if (ResCacheDic.ContainsKey(abType))
            {
                if (ResCacheDic[abType].ContainsKey(name))
                {
                    res = ResCacheDic[abType][name];
                }
            }
            return res;
        }







        private void onGameObjectInstantiate(string evtId, object[] paras)
        {
            GameObject obj = paras[0] as GameObject;

            this.fillReferenceOfDynamicCompInfoHolder(obj);
            this.addDestroyNotice(obj);
        }

        private void onChildObjDestroy(string evtId, object[] paras)
        {
            GameObject obj = paras[0] as GameObject;
            removeSpriteReference(obj.transform);
            removeTextureReference(obj.transform);
        }

        void removeSpriteReference(Transform trans)
        {
            //TODO:这里循环检索，有点耗，需要改进
            SpriteRes sr;
            for (int i = ABManagerMono.Instance.spriteResList.Count - 1; i >= 0; i--)
            {
                sr = ABManagerMono.Instance.spriteResList[i];
                if (sr.CheckRefTrs(trans))
                {
                    sr.RemoveRefTrs(trans);
                }

                if (sr.refTrsCount == 0)
                {
                    ABManagerMono.Instance.spriteResList.Remove(sr);
                }
            }
        }
        void removeTextureReference(Transform trans)
        {
            //TODO:这里循环检索，有点耗，需要改进
            TextureRes tr;
            for (int j = ABManagerMono.Instance.texResList.Count - 1; j >= 0; j--)
            {
                tr = ABManagerMono.Instance.texResList[j];
                if (tr.CheckRefTrs(trans))
                {
                    tr.RemoveRefTrs(trans);
                }

                if (tr.refTrsCount == 0)
                {
                    ABManagerMono.Instance.texResList.Remove(tr);
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
                if (abType == ABType.Window)
                {
                    for (int i = 0; i < ABManagerMono.Instance.windowResList.Count; i++)
                    {
                        if (ABManagerMono.Instance.windowResList[i].resObj == obj)
                        {
                            ABManagerMono.Instance.windowResList.Remove(ABManagerMono.Instance.windowResList[i]);
                            return;
                        }
                    }
                }
                else if (abType == ABType.Effect)
                {
                    for (int i = 0; i < ABManagerMono.Instance.effectResList.Count; i++)
                    {
                        if (ABManagerMono.Instance.effectResList[i].resObj == obj)
                        {
                            ABManagerMono.Instance.effectResList.Remove(ABManagerMono.Instance.effectResList[i]);
                            return;
                        }
                    }
                }
                else if (abType == ABType.OtherPrefab)
                {
                    for (int i = 0; i < ABManagerMono.Instance.opResList.Count; i++)
                    {
                        if (ABManagerMono.Instance.opResList[i].resObj == obj)
                        {
                            ABManagerMono.Instance.opResList.Remove(ABManagerMono.Instance.opResList[i]);
                            return;
                        }
                    }
                }
                else
                {
                    Debug.LogError("TODO:::");
                }
            }


        }

        void addDestroyNotice(GameObject rootObj)
        {
            rootObj.GetOrAddComponent<OnObjDestroyNotice>();
        }
        private void onABResLoaded(string evtId, object[] paras)
        {
            Res res = paras[0] as Res;


            if (res is WindowRes)
            {
                WindowRes wRes = res as WindowRes;
                ResCacheDic[ABType.Window][wRes.resName] = wRes;

                //----->可视化显示              
                var older = ABManagerMono.Instance.windowResList.Find(a => a.resName == wRes.resName);
                if (older != null)
                {
                    ABManagerMono.Instance.windowResList.Remove(older);
                }
                ABManagerMono.Instance.windowResList.Add(wRes);

                GameObject obj = wRes.resObj as GameObject;
                fillReferenceOfRootCompInfoHolder(obj);
                this.addDestroyNotice(obj);
            }
            else if (res is EffectRes)
            {
                EffectRes effRes = res as EffectRes;
                ResCacheDic[ABType.Effect][effRes.resName] = effRes;

                //可视化处理       
                var older = ABManagerMono.Instance.effectResList.Find(a => a.resName == effRes.resName);
                if (older != null)
                {
                    ABManagerMono.Instance.effectResList.Remove(older);
                }
                ABManagerMono.Instance.effectResList.Add(effRes);



                GameObject obj = effRes.resObj as GameObject;
                fillReferenceOfRootCompInfoHolder(obj);
                this.addDestroyNotice(obj);
            }
            else if (res is OtherPrefabRes)
            {
                OtherPrefabRes opRes = res as OtherPrefabRes;
                ResCacheDic[ABType.OtherPrefab][opRes.resName] = opRes;

                //可视化处理       
                var older = ABManagerMono.Instance.opResList.Find(a => a.resName == opRes.resName);
                if (older != null)
                {
                    ABManagerMono.Instance.opResList.Remove(older);
                }
                ABManagerMono.Instance.opResList.Add(opRes);



                GameObject obj = opRes.resObj as GameObject;
                fillReferenceOfRootCompInfoHolder(obj);
                this.addDestroyNotice(obj);
            }
            else if (res is SpriteRes)
            {
                SpriteRes sRes = res as SpriteRes;
                ABManagerMono.Instance.spriteResList.Add(sRes);
                ResCacheDic[ABType.Sprite][sRes.resName] = sRes;
            }
            else if (res is TextureRes)
            {
                TextureRes tRes = res as TextureRes;
                ABManagerMono.Instance.texResList.Add(tRes);
                ResCacheDic[ABType.Texture][tRes.resName] = tRes;
            }
            else if (res is AudioRes)
            {
                AudioRes aRes = res as AudioRes;
                ABManagerMono.Instance.audioResList.Add(aRes);
                ResCacheDic[ABType.Audio][aRes.resName] = aRes;
            }

        }

        private Res getCachedAbRes(ABType abType, string name)
        {
            //Debug.LogError("getCachedAbRes:" + name);
            if (ResCacheDic[abType].ContainsKey(name))
            {
                return ResCacheDic[abType][name];

            }
            //Debug.LogError("get cachedABRes fail," + name);
            return null;
        }


        public GameObject LoadWindow(string name)
        {
            GameObject window = null;

            var cache = getCachedAbRes(ABType.Window, name);
            if (cache == null || cache.resObj == null)//resObj为null说明真正的资源被destroy了。需要重新加载
            {
                window = ABPrefab.Load(name, ABType.Window);
            }
            else
            {
                DebugExt.Log("From AB cache get window:" + name);
                window = cache.GetRes<GameObject>(name);
            }
            return window;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="reUse">对于同一时间只会存在一个的特效，reUse值为true</param>
        /// <returns></returns>
        public GameObject LoadEffect(string name, bool reUse = true)
        {
            GameObject effect = null;

            var cache = getCachedAbRes(ABType.Effect, name);
            if (cache == null || cache.resObj == null)
            {
                effect = ABPrefab.Load(name, ABType.Effect);
            }
            else
            {
                if (reUse)
                {
                    effect = cache.GetRes<GameObject>(name);
                }
                else
                {
                    effect = ABPrefab.Load(name, ABType.Effect);
                }

            }
            return effect;
        }

        public GameObject LoadOtherPrefab(string name, bool reUse = false)
        {
            GameObject op = null;

            var cache = getCachedAbRes(ABType.OtherPrefab, name);
            if (cache == null || cache.resObj == null)
            {
                op = ABPrefab.Load(name, ABType.OtherPrefab);
            }
            else
            {
                if (reUse)
                {
                    op = cache.GetRes<GameObject>(name);
                }
                else
                {
                    op = ABPrefab.Load(name, ABType.OtherPrefab);
                }

            }
            return op;
        }


        void resetEditorShader(Transform tran, Material mat, string shaderName)
        {
            if (mat != null)
            {
                var shader = Shader.Find(shaderName);
                if (shader != null)
                {
                    mat.shader = shader;

                    //bug hack处理
                    //如果是standard shader且使用了Transparent的渲染模式，那么在编辑器模式下RenderQueue有问题，需要重置其RenderQueue
                    //若编辑器下不存在该bug，则无需设置
                    if (shaderName == "My/Standard" || shaderName == "My/Standard (Specular setup)")
                    {
                        if (mat.IsKeywordEnabled("_ALPHAPREMULTIPLY_ON"))
                        {
                            mat.renderQueue = 3000;
                        }
                    }
                }
                else
                {
                    Debug.LogError("can not find shader:" + shaderName);
                }
            }

        }

        void fillReferenceOfDynamicCompInfoHolder(GameObject target)
        {
            DynamicCompInfoHolder holder = target.GetComponent<DynamicCompInfoHolder>();
            if (holder == null)
            {
                Debug.LogError("no dynamicCompInfoHolder attached:" + target.GetHierarchy());
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
                            Debug.LogError("error, please check");
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
                            Debug.LogError("error, please check");
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
                            Debug.LogError("error, please check");
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
                                Debug.LogError("error, please check");
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
                                Debug.LogError("error, please check");
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
                                Debug.LogError("error, please check");
                            }
                        }
                    }
                }
            }
            //--------------->Ext BoxScenePetSetting
            if (holder.extCompBoxingScenePetSettingInfos != null && holder.extCompBoxingScenePetSettingInfos.Count > 0)
            {
                foreach (var item in holder.extCompBoxingScenePetSettingInfos)
                {
                    Transform childTarget = item.tran;
                    if (item.refTextures != null && item.refTextures.Count > 0)
                    {
                        for (int i = 0; i < item.refTextures.Count; i++)
                        {
                            ImgRes imgRes = getCachedAbRes(ABType.Texture, item.refTextures[i].texName) as ImgRes;
                            if (imgRes != null)
                            {
                                imgRes.AddRefTrs(childTarget);
                            }
                            else
                            {
                                Debug.LogError("error, please check");
                            }
                        }
                    }
                }
            }
        }

        void fillReferenceOfRootCompInfoHolder(GameObject target)
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
                        FillSprite<Image>(item.refSprites[0].atlasName, item.refSprites[0].spriteName, childTarget.GetComponent<Image>());

#if UNITY_EDITOR
                        resetEditorShader(childTarget, item.mat, item.shaderName);
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
                        FillSprite<SpriteRenderer>(item.refSprites[0].atlasName,
                                               item.refSprites[0].spriteName,
                                               childTarget.GetComponent<SpriteRenderer>());
#if UNITY_EDITOR
                        resetEditorShader(childTarget, item.mat, item.shaderName);
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
                        resetEditorShader(childTarget, item.mat, item.shaderName);
#endif
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
                        Texture tex = null;
                        LoadTexture(item.refTextures[k].texName, childTarget, out tex);
                        //对于TextMeshPro字体这里item.mat会为null，故这里加以判断，避免报错
                        //字体不用为其mat再赋texture依旧显示正确
                        if (item.mat != null)
                        {
                            item.mat.SetTexture(item.refTextures[k].shaderProp, tex);
                        }

                    }

#if UNITY_EDITOR
                    resetEditorShader(childTarget, item.mat, item.shaderName);
#endif


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
                            LoadSpriteToImageSeq(imageSequence, item.refSprites[i].atlasName, item.refSprites[i].spriteName, i);
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
                            LoadSpriteToSpriteSeq(spriteSequence, item.refSprites[i].atlasName, item.refSprites[i].spriteName, i);
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
                            LoadSpriteToSwapSprite(swapSprite, item.refSprites[i].atlasName, item.refSprites[i].spriteName, i);
                        }
                    }
                }
            }



            holder.finishedSet = true;
        }


        Texture loadTexture(string texName)
        {
            Texture tex = null;
            var cache = getCachedAbRes(ABType.Texture, texName);
            if (cache == null)
            {
                tex = ABTexture.Load(texName);
            }
            else
            {
                tex = cache.GetRes<Texture>(texName);
            }

            return tex;
        }


        Sprite loadSprite(string atlasName, string spriteName)
        {
            Sprite target = null;
            var cache = getCachedAbRes(ABType.Sprite, atlasName);
            if (cache == null)
            {
                target = ABSprite.Load(atlasName, spriteName);
            }
            else
            {
                target = cache.GetRes<Sprite>(spriteName);
            }

            return target;
        }


        public void LoadTexture(string texName, Transform targetTrs, out Texture tex)
        {
            tex = loadTexture(texName);
            ImgRes imgRes = getCachedAbRes(ABType.Texture, texName) as ImgRes;
            if (targetTrs != null)
            {
                imgRes.AddRefTrs(targetTrs);
            }
        }



        public void FillSprite<T>(string atlasName, string spriteName, T targetComponent)
        {
            Sprite sprite = loadSprite(atlasName, spriteName);

            ImgRes imgRes = getCachedAbRes(ABType.Sprite, atlasName) as ImgRes;

            Transform trs = null;
            if (typeof(T).Equals(typeof(SpriteRenderer)))
            {
                SpriteRenderer sr = (targetComponent as SpriteRenderer);
                trs = sr.transform;
                sr.sprite = sprite;
            }
            else if (typeof(T).Equals(typeof(Image)))
            {
                Image img = (targetComponent as Image);
                trs = img.transform;
                img.sprite = sprite;
            }
            else
            {
                Debug.LogError("类型不匹配！！");
            }
            if (trs != null)
            {
                imgRes.AddRefTrs(trs);
            }

        }

        public void LoadSpriteToSpriteSeq(SpriteSequence ss, string atlasName, string spriteName, int index)
        {
            Sprite sprite = loadSprite(atlasName, spriteName);
            ss.sprites[index] = sprite;

            ImgRes imgRes = getCachedAbRes(ABType.Sprite, atlasName) as ImgRes;
            imgRes.AddRefTrs(ss.transform);
        }

        public void LoadSpriteToImageSeq(ImageSequence ss, string atlasName, string spriteName, int index)
        {
            Sprite sprite = loadSprite(atlasName, spriteName);
            ss.sprites[index] = sprite;

            ImgRes imgRes = getCachedAbRes(ABType.Sprite, atlasName) as ImgRes;
            imgRes.AddRefTrs(ss.transform);
        }

        public void LoadSpriteToSwapSprite(SwapSprite ss, string atlasName, string spriteName, int index)
        {
            Sprite sprite = loadSprite(atlasName, spriteName);
            ss.sprites[index] = sprite;

            ImgRes imgRes = getCachedAbRes(ABType.Sprite, atlasName) as ImgRes;
            imgRes.AddRefTrs(ss.transform);
        }


        public void LoadSpriteToSpriteRenderer(string atlasName, string spriteName, SpriteRenderer targetSR)
        {
            Sprite sprite = loadSprite(atlasName, spriteName);

            ImgRes imgRes = getCachedAbRes(ABType.Sprite, atlasName) as ImgRes;

            Transform trs = targetSR.transform;
            targetSR.sprite = sprite;
            imgRes.AddRefTrs(trs);
        }

        public void LoadSpriteToImage(string atlasName, string spriteName, Image targetImg)
        {
            Sprite sprite = loadSprite(atlasName, spriteName);

            ImgRes imgRes = getCachedAbRes(ABType.Sprite, atlasName) as ImgRes;
            Transform trs = targetImg.transform;
            targetImg.sprite = sprite;

            imgRes.AddRefTrs(trs);
        }






        public AudioClip LoadAudioClip(string name)
        {
            AudioClip clip = null;
            var cache = getCachedAbRes(ABType.Audio, name);
            if (cache == null || cache.resObj == null)
            {
                clip = ABAudio.Load(name);
            }
            else
            {
                clip = cache.GetRes<AudioClip>(name);
            }

            return clip;

        }

        public TextAsset[] LoadLogic(string name)
        {
            TextAsset[] ta = ABLogic.LoadAll(name);
            return ta;
        }


        public void UnpackLogicABToMap(string logicABName)
        {
#if XLua
            LuaResLoader.Instance.LoadScriptBundle(logicABName);
#endif
        }





        //TODO:暂时还没有对材质球、shader等其它资源进行引用计数处理，这里使用Unity自带的方法进行内存的清除！！！
        float memeryClearDuration = 60f;
        public override void Update()
        {
            //////this.memeryClearDuration -= Time.deltaTime;
            //////if (this.memeryClearDuration < 0)
            //////{
            //////    this.memeryClearDuration = 60;
            //////    Resources.UnloadUnusedAssets();
            //////    GC.Collect();
            //////}
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EventID.OnABResLoaded, onABResLoaded);
            EventDispatcher.Instance.RemoveListener(EventID.OnRootObjDestroy, onRootObjDestroy);
            EventDispatcher.Instance.RemoveListener(EventID.OnChildObjDestroy, onChildObjDestroy);
            EventDispatcher.Instance.RemoveListener(EventID.OnGameObjectInstantiate, onGameObjectInstantiate);
        }
    }
}