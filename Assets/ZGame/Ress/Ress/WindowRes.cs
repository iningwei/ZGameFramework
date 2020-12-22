using System;
using UnityEngine;

namespace ZGame.Ress
{
    [Serializable]
    public class WindowRes : PrefabRes
    {
        public WindowRes(string resName, UnityEngine.GameObject resObj) : base(resName, resObj)
        {
        }


        public override T GetRes<T>(string name)
        {
            return base.GetRes<T>(name);
        }
    }
}