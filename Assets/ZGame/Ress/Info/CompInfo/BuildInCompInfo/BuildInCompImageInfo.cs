using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Ress.Info
{
    [System.Serializable]
    /// <summary>
    /// 内置组件Image
    /// Image承载的图片资源类型是Sprite，不支持对Texture的直接使用
    /// </summary>
    public class BuildInCompImageInfo : BuildInCompInfo
    {

        public List<SpriteInfo> refSprites;

        public BuildInCompImageInfo(Transform tran, Material mat, string shaderName, List<SpriteInfo> refSprites)
        {
            this.tran = tran;
            this.mat = mat;

            this.shaderName = shaderName;
            this.refSprites = refSprites;
        }
    }
}