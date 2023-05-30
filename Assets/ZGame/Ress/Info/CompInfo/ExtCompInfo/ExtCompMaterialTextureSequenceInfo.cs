using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class ExtCompMaterialTextureSequenceInfo : ExtCompInfo
    {
        public List<TextureInfo> refTextures;
        public ExtCompMaterialTextureSequenceInfo(Transform tran, List<TextureInfo> refTextures)
        {
            this.tran = tran;
            this.refTextures = refTextures;
        }
    }
}