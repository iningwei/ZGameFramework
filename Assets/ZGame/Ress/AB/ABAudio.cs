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
        /// name:no prefix,no suffix
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static void Load(string name, Action<UnityEngine.AudioClip> callback, bool sync)
        {
            Action<UnityEngine.Object[]> loadFinishHandle = (objs) =>
            {
                AudioClip clip = objs[0] as AudioClip;
                if (clip == null)
                {
                    DebugExt.LogE("load audioclip " + name + " null");
                }
                if (callback != null)
                {
                    callback(clip);
                }

                AudioRes res = new AudioRes(name, clip);
                EventDispatcher.Instance.DispatchEvent(EventID.OnABResLoaded, res, sync);
            };


            if (sync)
            {
                AB.Load(name, ABType.Audio, (objs) =>
                {
                    loadFinishHandle(objs);
                });
            }
            else
            {
                AB.LoadAsync(name, ABType.Audio, (objs) =>
                {
                    loadFinishHandle(objs);
                });
            }


        }
    }
}