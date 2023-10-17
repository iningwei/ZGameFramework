using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class BuildInCompTextMeshProUGUIInfo : BuildInCompInfo
    {
        public TextMeshProUGUI concreteCompTextMeshProUGUI;
        public BuildInCompTextMeshProUGUIInfo(Transform tran, TextMeshProUGUI refTextMeshProUGUI, string meshName, string matName, string shaderName) : base(tran, meshName, 0, matName, shaderName)
        {
            this.concreteCompTextMeshProUGUI = refTextMeshProUGUI;
        }
    }
}