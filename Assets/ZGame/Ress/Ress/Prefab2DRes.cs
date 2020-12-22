using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Ress
{
    public class Prefab2DRes : PrefabRes
    {
        public Prefab2DRes(string name, GameObject obj) : base(name, obj)
        {

        }

        public override T GetRes<T>(string name)
        {
            return base.GetRes<T>(name);
        }
    }
}
