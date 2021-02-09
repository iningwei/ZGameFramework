using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZGame.Ress.AB
{
    /// <summary>
    /// Lua逻辑代码的AB包没有采用和其它类型AB通用的前缀模式，因此涉及到name的地方都需要传全名（但是不需要后缀）
    /// </summary>
    public class ABLogic
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">传全名(不带后缀)</param>
        /// <returns></returns>
        static AssetBundle LoadAB(string name)
        {
            return AB.Load(name);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">传全名(不带后缀)</param>
        /// <returns></returns>
        public static TextAsset[] LoadAll(string name)
        {
            TextAsset[] ta = null;
            AssetBundle ab = LoadAB(name);
            ta = ab.LoadAllAssets<TextAsset>();
            ab.Unload(false);
            return ta;
        }
    }
}