using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class BuildInCompTextInfo : BuildInCompInfo
    {
        public Text concreteCompText;
        public BuildInCompTextInfo(Transform tran, Text refText, string meshName, string matName, string shaderName) : base(tran, meshName, 0, matName, shaderName)
        {
            this.concreteCompText = refText;
        }
    }
}