using FSG.MeshAnimator.ShaderAnimated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.Info;
namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class ExtCompShaderMeshAnimationInfo : ExtCompInfo
    {
        public string defaultAnimation;
        public List<string> animations;

        public ExtCompShaderMeshAnimationInfo(Transform tran, string defaultAnim, List<string> anims) : base(tran)
        {
            this.defaultAnimation = defaultAnim;
            this.animations = anims;
        }
    }
}