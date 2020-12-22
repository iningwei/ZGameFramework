using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Ress
{
    public class EffectRes : PrefabRes
    {
        public EffectRes(string name, GameObject obj) : base(name, obj)
        {

        }

        public override T GetRes<T>(string name)
        {
            return base.GetRes<T>(name);
        }
    }
}
