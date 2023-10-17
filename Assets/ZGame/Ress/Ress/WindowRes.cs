using System;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.Ress
{
    [Serializable]
    public class WindowRes : PrefabRes
    {
        public WindowRes(string name, UnityEngine.GameObject resObj) : base(name, resObj)
        {
        }


        public override T GetRes<T>(string name)
        {
            return base.GetRes<T>(name);
        }

        public override void Destroy()
        {
            base.Destroy();
             
            ABManager.Instance.RemoveCachedRes(ABType.Window, this);
        }
    }
}