using System;
using UnityEngine;

namespace ZGame.Ress
{
    [Serializable]
    public class AudioRes : Res
    {
        string name;
        AudioClip clip;

        public AudioRes(string resName, AudioClip resObj) : base(resName, resObj)
        {
            this.name = resName;
            this.clip = resObj;
        }

        public override T GetRes<T>(string name)
        {
            T result = default(T);
            if (!(typeof(T).Equals(typeof(AudioClip))))
            {
                Debug.LogError("类型不匹配AudioClip");
            }

            if (this.resName != name)
            {
                Debug.LogError("error,name不匹配");
            }

            result = (T)(object)clip;


            if (result == null)
            {
                Debug.LogError("error, get res fail, audio clip is null");
            }
            return result;
        }
    }
}