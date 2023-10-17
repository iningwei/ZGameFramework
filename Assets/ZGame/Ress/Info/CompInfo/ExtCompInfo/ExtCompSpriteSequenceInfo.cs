using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class ExtCompSpriteSequenceInfo : ExtCompInfo
    {
        public List<SpriteInfo> refSprites;
        public ExtCompSpriteSequenceInfo(Transform tran, List<SpriteInfo> refSprites) : base(tran)
        { 
            this.refSprites = refSprites;
        }
    }
}