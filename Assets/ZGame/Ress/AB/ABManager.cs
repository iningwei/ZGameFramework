using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using ZGame.Event;
using ZGame.Ress.AB.Holder;

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










        private void onChildObjDestroy(string evtId, object[] paras)
        {
            GameObject obj = paras[0] as GameObject;
          
            //TODO:这里循环检索，有点耗，需要改进
            SpriteRes sr;
            for (int i = ABManagerMono.Instance.spriteResList.Count - 1; i >= 0; i--)
            {
                sr = ABManagerMono.Instance.spriteResList[i];
                if (sr.CheckRefTrs(obj.transform))
                {
                    sr.RemoveRefTrs(obj.transform);
                }

                if (sr.refTrsCount == 0)
                {
                    ABManagerMono.Instance.spriteResList.Remove(sr);
                }
            }

            TextureRes tr;
            for (int j = ABManagerMono.Instance.texResList.Count - 1; j >= 0; j--)
            {
                tr = ABManagerMono.Instance.texResList[j];
                if (tr.CheckRefTrs(obj.transform))
                {
                    tr.RemoveRefTrs(obj.transform);
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
            MatTextureHolder holder = obj.GetComponent<MatTextureHolder>();
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
                //var older = windowResList.Find(a => a.resName == wRes.resName);
                //if (older != null)
                //{
                //    older = wRes;
                //}
                //else
                //{
                //    windowResList.Add(wRes);
                //}
                //TODO:用上述方法，不知道是什么原因导致的inspector面板上ResObj没有显示出来

                var older = ABManagerMono.Instance.windowResList.Find(a => a.resName == wRes.resName);
                if (older != null)
                {
                    ABManagerMono.Instance.windowResList.Remove(older);
                }

                ABManagerMono.Instance.windowResList.Add(wRes);

                GameObject obj = wRes.resObj as GameObject;
                fillMatTexHolder(obj);
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
                fillMatTexHolder(obj);
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
                fillMatTexHolder(obj);
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
                Debug.Log("!!!!!!!从AB缓存获得window:" + name);
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

        public GameObject LoadOtherPrefab(string name)
        {
            GameObject op = null;

            var cache = getCachedAbRes(ABType.OtherPrefab, name);
            if (cache == null || cache.resObj == null)
            {
                op = ABPrefab.Load(name, ABType.OtherPrefab);
            }
            else
            {
                op = cache.GetRes<GameObject>(name);
            }
            return op;
        }
        void fillMatTexHolder(GameObject target)
        {
            MatTextureHolder matTexHolder = target.GetComponent<MatTextureHolder>();
            if (matTexHolder.finishedSet)
            {
                //Debug.Log(target + " 已经设置了材质，不可重复设置");
                return;
            }


            if (matTexHolder != null && matTexHolder.allTransformInfos != null && matTexHolder.allTransformInfos.Count > 0)
            {
                foreach (var item in matTexHolder.allTransformInfos)
                {
                    Transform childTarget = item.target;

                    var matInfos = item.matInfos;
                    if (matInfos != null && matInfos.Count > 0)
                    {
                        foreach (var matInfo in matInfos)
                        {
                            switch (matInfo.matType)
                            {
                                case MatTextureHolder.MatInfo.MatType.Image:
                                    if (matInfo.textureInfos == null || matInfo.textureInfos.Count > 1)
                                    {
                                        Debug.LogError("Image 类型的texInfo有错！");
                                    }
                                    if (matInfo.textureInfos.Count == 0)
                                    {

                                    }
                                    else
                                    {
                                        //////childTarget.GetComponent<Image>().sprite = LoadSprite(matInfo.texInfos[0].atlasName, matInfo.texInfos[0].texName);
                                        LoadSprite<Image>(matInfo.textureInfos[0].atlasName,
                                            matInfo.textureInfos[0].texName,
                                               childTarget.GetComponent<Image>());
                                    }
                                    break;
                                case MatTextureHolder.MatInfo.MatType.SpriteRenderer:
                                    if (matInfo.textureInfos == null || matInfo.textureInfos.Count > 1)
                                    {
                                        Debug.LogError("SpriteRenderer 类型的texInfo有错！");
                                    }
                                    //////childTarget.GetComponent<SpriteRenderer>().sprite = LoadSprite(matInfo.texInfos[0].atlasName, matInfo.texInfos[0].texName);
                                    LoadSprite<SpriteRenderer>(matInfo.textureInfos[0].atlasName,
                                                matInfo.textureInfos[0].texName,
                                                childTarget.GetComponent<SpriteRenderer>()
                                                                       );
                                    break;
                                case MatTextureHolder.MatInfo.MatType.NormalRenderer:
                                    MatTextureHolder.TextureInfo texInfo = null;
                                    for (int k = 0; k < matInfo.textureInfos.Count; k++)
                                    {
                                        texInfo = matInfo.textureInfos[k];
                                        if (texInfo.isSprite)
                                        {
                                            Debug.LogError("texInfo有错，项目中规定NormalRenderer不可使用Sprite，objPath:" + childTarget.Hierarchy());
                                        }
                                        Texture tex = null;
                                        LoadTexture(texInfo.texName, childTarget, out tex);
                                        matInfo.mat.SetTexture(matInfo.textureInfos[k].shaderProperty, tex);
                                    }

                                    break;
                                default:
                                    break;
                            }



#if UNITY_EDITOR
                            //editor下，若使用android或者ios的bundle，需要使用本地的shader，否则红色
                            if (matInfo.mat != null)
                            {
                                var shader = Shader.Find(matInfo.shaderName);
                                if (shader != null)
                                {
                                    matInfo.mat.shader = shader;
                                }
                                else
                                {
                                    Debug.LogError("can not find shader:" + matInfo.shaderName);
                                }
                            }
#endif
                        }
                    }


                    if (childTarget.gameObject.activeInHierarchy)//TODO:偶尔会出现有物体不渲染，暂时没有搞明白导致的原因，通过该方式hack
                    {
                        childTarget.gameObject.SetActive(false);
                        childTarget.gameObject.SetActive(true);
                    }
                }
            }




            //处理spriteSequence
            if (matTexHolder != null && matTexHolder.allSpriteSequenceInfos != null && matTexHolder.allSpriteSequenceInfos.Count > 0)
            {
                for (int j = 0; j < matTexHolder.allSpriteSequenceInfos.Count; j++)
                {
                    var ssi = matTexHolder.allSpriteSequenceInfos[j];
                    var childTarget = ssi.target;

                    if (ssi.texInfos != null && ssi.texInfos.Count > 0)
                    {
                        var ss = childTarget.GetComponent<SpriteSequence>();
                        for (int k = 0; k < ssi.texInfos.Count; k++)
                        {
                            var ti = ssi.texInfos[k];
                            LoadSpriteToSeq(ss, ti.atlasName, ti.texName, k);
                        }
                    }


                    if (childTarget.gameObject.activeInHierarchy)//TODO:偶尔会出现有物体不渲染，暂时没有搞明白导致的原因，通过该方式hack
                    {
                        childTarget.gameObject.SetActive(false);
                        childTarget.gameObject.SetActive(true);
                    }
                }
            }




            matTexHolder.finishedSet = true;
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

        public void LoadSprite<T>(string texProperty, string atlasName, string spriteName, T targetComponent)
        {
            Sprite sprite = loadSprite(atlasName, spriteName);
            ImgRes imgRes = getCachedAbRes(ABType.Sprite, atlasName) as ImgRes;
            Transform trs = null;


            if (trs != null)
            {
                imgRes.AddRefTrs(trs);
            }
        }

        public void LoadSprite<T>(string atlasName, string spriteName, T targetComponent)
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

        public void LoadSpriteToSeq(SpriteSequence ss, string atlasName, string spriteName, int index)
        {
            Sprite sprite = loadSprite(atlasName, spriteName);
            ss.sprites[index] = sprite;

            ImgRes imgRes = getCachedAbRes(ABType.Sprite, atlasName) as ImgRes;
            imgRes.AddRefTrs(ss.transform);
        }

#if XLua
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
            targetImg.SetNativeSize();
            imgRes.AddRefTrs(trs);
        }
#endif





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
        float memeryClearDuration = 20f;
        private void Update()
        {
            this.memeryClearDuration -= Time.deltaTime;
            if (this.memeryClearDuration < 0)
            {
                this.memeryClearDuration = 20;
                Resources.UnloadUnusedAssets();
                GC.Collect();
            }
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveListener(EventID.OnABResLoaded, onABResLoaded);
            EventDispatcher.Instance.RemoveListener(EventID.OnRootObjDestroy, onRootObjDestroy);
            EventDispatcher.Instance.RemoveListener(EventID.OnChildObjDestroy, onChildObjDestroy);
        }
    }
}