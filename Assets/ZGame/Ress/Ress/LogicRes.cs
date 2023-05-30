using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Ress
{
    public class LogicRes : Res
    {
        string name;
        TextAsset textAsset;

        public LogicRes(string resName, TextAsset resObj) : base(resName, resObj)
        {
            this.name = resName;
            textAsset = resObj;
        }

        public override T GetRes<T>(string name)
        {
            T result = default(T);
            if (!(typeof(T).Equals(typeof(TextAsset))))
            {
                Debug.LogError("类型不匹配 TextAsset");
            }

            if (this.resName != name)
            {
                Debug.LogError("error,name不匹配");
            }

            result = (T)(object)this.textAsset;

            if (result == null)
            {
                Debug.LogError("error, get res fail,textAsset is null");
            }

            return result;
        }
    }
}
