using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class ExtCompBoxingScenePetSettingInfo : ExtCompInfo
    {
        public List<TextureInfo> refTextures;
        public ExtCompBoxingScenePetSettingInfo(Transform tran, List<TextureInfo> refTextures)
        {
            this.tran = tran;
            this.refTextures = refTextures;
        }
    }
}