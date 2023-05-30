using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class BuildInCompRendererInfo : BuildInCompInfo
    {

        public List<TextureInfo> refTextures;

        public BuildInCompRendererInfo(Transform tran, Material mat, string shaderName, List<TextureInfo> refSprites)
        {
            this.tran = tran;
            this.mat = mat;

            this.shaderName = shaderName;
            this.refTextures = refSprites;
        }
    }
}