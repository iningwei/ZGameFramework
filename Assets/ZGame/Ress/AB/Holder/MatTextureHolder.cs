using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Ress.AB.Holder
{
    /// <summary>
    /// 材质球Tex引用Holder
    /// </summary>
    public class MatTextureHolder : MonoBehaviour
    {

        public ABType abType;

        /// <summary>
        /// 程序里 是否完成材质球、贴图的设置
        /// 游戏运行时，设置完毕后，把该值设置为true
        /// </summary>
        public bool finishedSet = false;

        [System.Serializable]
        public class TextureInfo
        {
            public bool isSprite;//用来区分是否是精灵类型
            public string shaderProperty;//isSprite为false的话，才用到该属性
            public string texProperty;//当图片没有依赖mat的时候（比如CheckButtonEx）,图片对应的属性名
            /// <summary>
            /// isSprite为true的时候，才用到该属性
            /// </summary>
            public string atlasName;
            /// <summary>
            /// 如果isSprite为true的话，这里texName就对应精灵图集中精灵的名字
            /// isSprite为false的话，这里texName对应图片名字
            /// </summary>
            public string texName;


            public TextureInfo(bool isSprite, string shaderProperty, string texProperty, string texName, string atlasName)
            {
                this.isSprite = isSprite;
                this.shaderProperty = shaderProperty;
                this.texProperty = texProperty;
                this.texName = texName;
                this.atlasName = atlasName;

            }

            public TextureInfo(bool isSprite, string shaderProperty, string texName, string atlasName)
            {
                this.isSprite = isSprite;
                this.shaderProperty = shaderProperty;
                this.texName = texName;
                this.atlasName = atlasName;
            }
        }

        [System.Serializable]
        public class MatInfo
        {
            [System.Serializable]
            public enum MatType
            {

                Image = 10,//对应UGUI的Image组件，只有一个材质球，使用Sprite格式贴图
                SpriteRenderer,//对应Unity2D的SpriteRenderer组件，只有一个材质球，使用Sprite格式贴图
                NormalRenderer,//对应基本的Renderer组件（诸如 3D obj以及ParticalSystem）
                Text,//对应UGUI的Text组件，只有一个材质球，无贴图

                None,//对应无材质球的情况
            };


            public Material mat;
            public MatType matType;
            public string shaderName;//mat使用的shader名称，用于在editor模式下，赋shader
            public List<TextureInfo> textureInfos;//shader使用的图片信息           public List<TextureInfo> textureInfos;//shader使用的图片信息

            public MatInfo(Material mat, MatType t, string shaderName, List<TextureInfo> texInfos)
            {
                this.mat = mat;
                this.matType = t;
                this.shaderName = shaderName;
                this.textureInfos = texInfos;
            }
        }

        [System.Serializable]
        public class TransformInfo
        {
            public Transform target;
            public List<MatInfo> matInfos;

            public TransformInfo(Transform target, List<MatInfo> matInfos)
            {
                this.target = target;
                this.matInfos = matInfos;
            }

        }

        [System.Serializable]
        public class SpriteSequenceInfo
        {
            public Transform target;

            public List<TextureInfo> texInfos;


            public SpriteSequenceInfo(Transform target, List<TextureInfo> texInfos)
            {
                this.target = target;
                this.texInfos = texInfos;
            }
        }


        public List<TransformInfo> allTransformInfos;
        public List<SpriteSequenceInfo> allSpriteSequenceInfos;

    }
}
