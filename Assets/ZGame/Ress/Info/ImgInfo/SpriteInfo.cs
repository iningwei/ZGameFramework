using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class SpriteInfo : ImgInfo
    {
        /// <summary>
        /// ͼ����
        /// </summary>
        public string atlasName;
        /// <summary>
        /// ������
        /// </summary>
        public string spriteName;

        public SpriteInfo(string atlasName, string spriteName)
        {
            this.atlasName = atlasName;
            this.spriteName = spriteName;
        }
    }
}