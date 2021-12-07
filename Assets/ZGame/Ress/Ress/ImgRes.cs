using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.Ress
{
    /// <summary>
    /// 图片类型(Sprite,Texture)资源的基类
    /// </summary>
    public class ImgRes : Res
    {
        public ImgRes(string resName, UnityEngine.Object resObj) : base(resName, resObj)
        {

        }
        //依赖该图片的所有物体
        public List<Transform> refTrs = new List<Transform>();//依赖该图片的Transform


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
            if (!this.refTrs.Contains(refTrs))
            {
                this.refTrs.Add(refTrs);
            }

            refTrsCount = this.refTrs.Count;
        }
        public void RemoveRefTrs(Transform refTrs)
        {

            if (this.refTrs.Contains(refTrs))
            {
                this.refTrs.Remove(refTrs);
                refTrsCount = this.refTrs.Count;

                if (this.refTrs.Count == 0)
                {
                    this.Destroy();
                }
            }
        }

        public bool CheckRefTrs(Transform refTrs)
        {
            var f = this.refTrs.Contains(refTrs);
            return f;
        }


    }
}
