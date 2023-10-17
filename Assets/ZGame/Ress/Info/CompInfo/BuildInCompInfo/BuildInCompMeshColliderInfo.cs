using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.Info;
namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class BuildInCompMeshColliderInfo : BuildInCompInfo
    {
        public MeshCollider concreteCompMeshCollider;
        public BuildInCompMeshColliderInfo(Transform tran, MeshCollider refMeshCollider, string meshName, int matIndex, string matName, string shaderName) : base(tran, meshName, matIndex, matName, shaderName)
        {
            concreteCompMeshCollider = refMeshCollider;
        }
    }
}