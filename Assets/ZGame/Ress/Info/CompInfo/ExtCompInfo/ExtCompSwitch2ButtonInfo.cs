using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class ExtCompSwitch2ButtonInfo : ExtCompInfo
    {
        public List<SpriteInfo> refSprites;
        public ExtCompSwitch2ButtonInfo(Transform tran, List<SpriteInfo> refSprites) : base(tran)
        {
            this.refSprites = refSprites;
        }
    }
}