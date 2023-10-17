using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class BuildInCompRendererInfo : BuildInCompInfo
    {
        public Renderer concreteCompRenderer;
        public List<TextureInfo> refTextures;
        public BuildInCompRendererInfo(Transform tran, Renderer refRenderer, List<TextureInfo> refSprites, string meshName, int matIndex, string matName, string shaderName) : base(tran, meshName, matIndex, matName, shaderName)
        {
            this.concreteCompRenderer = refRenderer;
            this.refTextures = refSprites;
        }
    }
}