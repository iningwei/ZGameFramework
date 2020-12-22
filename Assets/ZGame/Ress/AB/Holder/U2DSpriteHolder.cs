using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Ress.AB.Holder
{


    public class U2DSpriteHolder : MonoBehaviour
    {
        [System.Serializable]
        public class SpriteEntity
        {
            public Transform target;
            public string atlasName;//图片所在图集的名称
            public string spriteName;//图片的名称
            public Material mat;
            public string shaderName;//mat使用的shader名称，用于在editor模式下，赋shader
            public SpriteEntity(Transform target, string atlasName, string spriteName, Material mat, string shaderName)
            {

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