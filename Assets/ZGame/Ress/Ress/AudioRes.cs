using System;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.Ress
{
    [Serializable]
    public class AudioRes : Res
    {
        string name;
        AudioClip clip;

        public AudioRes(string name, AudioClip resObj) : base(name, resObj)
        {
            this.name = this.resName;
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

        public override void Destroy()
        {
            base.Destroy();
            ABManager.Instance.RemoveCachedRes(ABType.Audio, this);
        }
    }
}