using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using ZGame.Event;

namespace ZGame.Ress.AB
{
    public class ABByte
    {
        /// <summary>
        /// name:no prefix,no suffix
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static void Load(string name, Action<TextAsset> callback, bool sync)
        {
            Action<UnityEngine.Object[]> loadFinishHandle = (objs) =>
            {
                
                TextAsset asset = objs[0] as TextAsset;

                if (asset == null)
                {
                    DebugExt.LogE("load byte " + name + " null");
                }
                if (callback != null)
                {
                    callback(asset);
                }
           

                ByteRes res = new ByteRes(name, asset);
                EventDispatcher.Instance.DispatchEvent(EventID.OnABResLoaded, res, sync);
            };


            if (sync)
            {
                AB.Load(name, ABType.Byte, (objs) =>
                {
                    loadFinishHandle(objs);
                });
            }
            else
            {
                AB.LoadAsync(name, ABType.Byte, (objs) =>
                {
                    loadFinishHandle(objs);
                });
            }


        }
    }
}