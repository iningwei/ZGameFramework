using System;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Event;
using ZGame.Ress.AB;

namespace ZGame.Ress
{
    public enum ResLoadType
    {
        AssetBundle = 0,
        Resources = 1,
    }

    [Serializable]
    public class Res
    {
        public string resName;
        public UnityEngine.Object resObj;

        public Res(string name, UnityEngine.Object resObj)
        {
            this.resName = name;
            this.resObj = resObj;
        }



        public void SetResObj(UnityEngine.Object resObj)
        {
            this.resObj = resObj;
        }
        public virtual T GetRes<T>(string name)
        {
            return default;
        }


        public List<Transform> refTrsList = new List<Transform>();//依赖该资源的Transform,主要用于inspector界面展示用
        public Dictionary<int, Transform> refTrsDic = new Dictionary<int, Transform>();//key为Transform的ID

        [SerializeField, SetProperty("refTrsCount")]
        int _refTrsCount;

        public int refTrsCount
        {
            get
            {
                return _refTrsCount;
            }
            set
            {
                _refTrsCount = value;
            }
        }


        public void AddRefTrs(Transform refTrs)
        {
            if (!this.refTrsList.Contains(refTrs))
            {
                this.refTrsList.Add(refTrs);
                refTrsCount = this.refTrsList.Count;

                var id = refTrs.GetInstanceID();
                refTrsDic.Add(id, refTrs);
            }
        }

        public void RemoveRefTrs(Transform refTrs)
        {
            if (this.refTrsList.Contains(refTrs))
            {
                this.refTrsList.Remove(refTrs);
                refTrsCount = this.refTrsList.Count;

                refTrsDic.Remove(refTrs.GetInstanceID());

                if (this.refTrsList.Count == 0)
                {
                    //TODO:后续引入驻留机制
                    this.Destroy();
                }
            }
        }

        public bool CheckRefTrs(Transform refTrs)
        {
            var f = this.refTrsDic.ContainsKey(refTrs.GetInstanceID());
            return f;
        }

        public virtual void Destroy()
        {
            if (resObj != null)
            {
                GameObject.DestroyImmediate(resObj, true);

            }
        }
    }
}
