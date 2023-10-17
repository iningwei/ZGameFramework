using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class BuildInCompRawImageInfo : BuildInCompInfo
    {
        public RawImage concreteCompRawImage;
        public string texName;
        public BuildInCompRawImageInfo(Transform tran, RawImage refRawImg, string texName, string meshName, int matIndex, string matName, string shaderName) : base(tran, meshName, matIndex, matName, shaderName)
        {
            this.concreteCompRawImage = refRawImg;
            this.texName = texName;
        }
    }
}