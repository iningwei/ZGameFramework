using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Event;

namespace ZGame.Ress.AB
{   
    public class ABTexture
    {
        /// <summary>
        /// name不带后缀，不带前缀
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static AssetBundle LoadAB(string name)
        {
            string nameNew = ABTypeUtil.GetPreFix(ABType.Texture) + name;
            return AB.Load(nameNew);
        }


        /// <summary>
        /// name不带后缀，不带前缀
        /// </summary>
        /// <param name="name">name为图片名称</param>
        /// <returns></returns>
        public static Texture Load(string name)
        {
            Texture tex = null;
            AssetBundle ab = LoadAB(name);

            tex = ab.LoadAsset(name) as Texture;
            ab.Unload(false);
            TextureRes res = new TextureRes(name, tex);
            EventDispatcher.Instance.DispatchEvent(EventID.OnABResLoaded, res);
            return tex;
        }
    }
}