using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ZGame.Ress
{

    /// <summary>
    /// 精灵atlas
    /// </summary>
    [Serializable]
    public class SpriteRes : ImgRes
    {
        [SerializeField]
        string atlasName;
        [SerializeField]
        public Sprite[] allSprites;

        public SpriteRes(string resName, Texture2D resObj, Sprite[] allSprites) : base(resName, resObj)
        {
            this.atlasName = resName;
            this.allSprites = allSprites;
        }

        public override T GetRes<T>(string name)
        {
            T result = default(T);
            if (!(typeof(T).Equals(typeof(Sprite))))
            {
                Debug.LogError("类型不匹配Sprite");
            }
            else
            {
                for (int i = 0; i < allSprites.Length; i++)
                {
                    if (allSprites[i].name == name)
                    {
                        result = (T)((object)allSprites[i]);
                        break;
                    }
                }
            }
            if (result == null)
            {
                Debug.LogError("GetRes fail, atlasName:" + this.atlasName + "没有精灵:" + name);
            }

            return result;
        }
    }
}