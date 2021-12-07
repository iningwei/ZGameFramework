using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Ress
{

    [Serializable]
    public class Res
    {
        public string resName;
        public UnityEngine.Object resObj;

        public Res(string resName, UnityEngine.Object resObj)
        {
            this.resName = resName;
            this.resObj = resObj;

        }

        public virtual T GetRes<T>(string name)
        {
            return default;
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
