using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Ress
{
    public class Prefab3DRes : PrefabRes
    {
        public Prefab3DRes(string name, GameObject obj) : base(name, obj)
        {

        }

        public override T GetRes<T>(string name)
        {
            return base.GetRes<T>(name);
        }
    }
}