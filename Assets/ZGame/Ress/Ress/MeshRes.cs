using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.Ress
{
    [Serializable]
    public class MeshRes : Res
    {
        UnityEngine.Mesh mesh;
        public MeshRes(string name, UnityEngine.Mesh resObj) : base(name, resObj)
        {
            this.mesh = resObj;
        }

        public override T GetRes<T>(string name)
        {
            T result = default(T);
            if (!(typeof(T).Equals(typeof(Mesh))))
            {
                Debug.LogError("类型不匹配 Mesh");
            }

            if (this.resName != name)
            {
                Debug.LogError("error,name不匹配");
            }

            result = (T)(object)this.mesh;

            if (result == null)
            {
                Debug.LogError("error, get res fail,mesh is null， meshName:" + name);
            }

            return result;
        }

        public override void Destroy()
        {
            base.Destroy();

            ABManager.Instance.RemoveCachedRes(ABType.Mesh, this);
        }
    }
}