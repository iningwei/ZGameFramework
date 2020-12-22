using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Event;

namespace ZGame.Ress.AB
{
    public class ABSprite
    {
        /// <summary>
        /// name不带后缀，不带前缀
        /// </summary>
        /// <param name="name">name为sprite图集名</param>
        /// <returns></returns>
        static AssetBundle LoadAB(string name)
        {
            string nameNew = ABTypeUtil.GetPreFix(ABType.Sprite) + name;
            return AB.Load(nameNew);
        }


        public static Sprite Load(string atlasName, string spriteName)
        {
            Sprite target = null;

            AssetBundle ab = LoadAB(atlasName);

            Sprite[] childSprites = ab.LoadAllAssets<Sprite>();
            for (int i = 0; i < childSprites.Length; i++)
            {
                if (childSprites[i].name == spriteName)
                {
                    target = childSprites[i];
                    break;
                }
            }

            if (target == null)
            {
                Debug.LogError("atlasName:" + atlasName + "没有精灵:" + spriteName);
            }

            ab.Unload(false);

            SpriteRes res = new SpriteRes(atlasName, childSprites[0].texture, childSprites);
            EventDispatcher.Instance.DispatchEvent(EventID.OnABResLoaded, res);
            return target;
        }

    }


}