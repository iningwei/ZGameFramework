using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class ExtCompSwapSpriteInfo : ExtCompInfo
    {
        public List<SpriteInfo> refSprites;
        public ExtCompSwapSpriteInfo(Transform tran, List<SpriteInfo> refSprites) : base(tran)
        { 
            this.refSprites = refSprites;
        }
    }
}