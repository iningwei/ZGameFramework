using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class BuildInCompSpriteRendererInfo : BuildInCompInfo
    {
        public List<SpriteInfo> refSprites;
        public BuildInCompSpriteRendererInfo(Transform tran, Material mat, string shaderName, List<SpriteInfo> refSprites)
        {
            this.tran = tran;
            this.mat = mat;

            this.shaderName = shaderName;
            this.refSprites = refSprites;
        }
    }
}