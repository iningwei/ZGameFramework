using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class BuildInCompTextInfo : BuildInCompInfo
    {



        public BuildInCompTextInfo(Transform tran, Material mat, string shaderName)
        {
            this.tran = tran;
            this.mat = mat;
            this.shaderName = shaderName;
        }
    }
}