﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZGame.Ress.AB.Holder
{
    /// <summary>
    /// UGUI预制件，记录材质球，贴图依赖关系
    /// </summary>
    public class UGUISpriteHolder : MonoBehaviour
    {
        [System.Serializable]
        public enum UIType
        {
            Image,
            RawImage,
        }
        [System.Serializable]
        public class SpriteEntity
        {
            public UIType uiType;
            public Transform target;
            public string atlasName;//图片所在图集的名称
            public string spriteName;//图片的名称
            public Material mat;
            public string shaderName;//mat使用的shader名称，用于在editor模式下，赋shader
            public SpriteEntity(UIType t, Transform target, string atlasName, string spriteName, Material mat, string shaderName)
            {
                this.uiType = t;
                this.target = target;
                this.atlasName = atlasName;
                this.spriteName = spriteName;
                this.mat = mat;
                this.shaderName = shaderName;
            }

        }

        public List<SpriteEntity> entities;

    }
}