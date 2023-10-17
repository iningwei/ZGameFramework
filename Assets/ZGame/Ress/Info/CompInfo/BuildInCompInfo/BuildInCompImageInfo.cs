using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    /// <summary>
    /// 内置组件Image
    /// Image承载的图片资源类型是Sprite，不支持对Texture的直接使用
    /// </summary>
    public class BuildInCompImageInfo : BuildInCompInfo
    {
        public Image concreteCompImage;
        public List<SpriteInfo> refSprites;

        public BuildInCompImageInfo(Transform tran, Image refImage, List<SpriteInfo> refSprites, string meshName, string matName, string shaderName) : base(tran, meshName, 0, matName, shaderName)
        {
            this.concreteCompImage = refImage;
            this.refSprites = refSprites;
        }
    }
}