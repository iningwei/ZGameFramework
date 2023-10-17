using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class BuildInCompSpriteRendererInfo : BuildInCompInfo
    {
        public SpriteRenderer concreteCompSpriteRenderer;
        public List<SpriteInfo> refSprites;
        public BuildInCompSpriteRendererInfo(Transform tran, SpriteRenderer refSpriteRenderer, List<SpriteInfo> refSprites, string meshName, string matName, string shaderName) : base(tran, meshName, 0, matName, shaderName)
        {
            this.concreteCompSpriteRenderer = refSpriteRenderer;
            this.refSprites = refSprites;
        }
    }
}