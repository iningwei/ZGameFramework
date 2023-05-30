using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class BuildInCompTextMeshProUGUIInfo : BuildInCompInfo
    {
        
        public BuildInCompTextMeshProUGUIInfo(Transform tran, Material mat, string shaderName)
        {
            this.tran = tran;
            this.mat = mat;
            this.shaderName = shaderName;
        }
    }
}