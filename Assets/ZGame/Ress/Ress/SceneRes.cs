using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress;
using UnityEngine.SceneManagement;

namespace ZGame.Ress
{
    public class SceneRes : Res
    {
        public SceneRes(string resName, Object resObj) : base(resName, resObj)
        {
        }

        public override T GetRes<T>(string name)
        {
            T result = default(T);

            if (!(typeof(T).Equals(typeof(Scene))))
            {
                Debug.LogError("类型不匹配 Scene");
            }
            if (this.resName != name)
            {
                Debug.LogError("error,名字不匹配！！");
            }

            result = (T)(object)resObj;
            if (result == null)
            {
                Debug.LogError("Get SceneRes fail :" + name);
            }
            return result;

        }
    }
}