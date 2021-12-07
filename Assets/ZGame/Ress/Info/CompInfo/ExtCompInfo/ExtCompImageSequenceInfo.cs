using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class ExtCompImageSequenceInfo : ExtCompInfo
    {
        public List<SpriteInfo> refSprites;
        public ExtCompImageSequenceInfo(Transform tran, List<SpriteInfo> refSprites)
        {
            this.tran = tran;
            this.refSprites = refSprites;
        }
    }
}