using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class ExtCompSwapSpriteInfo : ExtCompInfo
    {
        public List<SpriteInfo> refSprites;
        public ExtCompSwapSpriteInfo(Transform tran, List<SpriteInfo> refSprites)
        {
            this.tran = tran;
            this.refSprites = refSprites;
        }
    }
}