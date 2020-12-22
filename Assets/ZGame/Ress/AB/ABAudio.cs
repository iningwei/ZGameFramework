using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Event;

namespace ZGame.Ress.AB
{
    public class ABAudio
    {
        /// <summary>
        /// name不带后缀，不带前缀
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static AssetBundle LoadAB(string name)
        {
            string nameNew = ABTypeUtil.GetPreFix(ABType.Audio) + name;
            return AB.Load(nameNew);
        }


        /// <summary>
        /// name不带后缀，不带前缀
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static AudioClip Load(string name)
        {
            AudioClip clip = null;
            AssetBundle ab = LoadAB(name);
            clip = ab.LoadAsset(name) as AudioClip;
            ab.Unload(false);
            if (clip == null)
            {
                Debug.LogError("load audioclip " + name + " null");
            }
            AudioRes res = new AudioRes(name, clip);
            EventDispatcher.Instance.DispatchEvent(EventID.OnABResLoaded, res);
            return clip;
        }
    }
}