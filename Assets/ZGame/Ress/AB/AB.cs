using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Ress.AB
{
    /// <summary>
    /// AssetBundle资源的基类
    /// </summary>
    public class AB
    {
        /// <summary>
        /// name不带后缀，需要带前缀
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static AssetBundle Load(string name)
        {
            name = name + IOTools.abSuffix;


            name = name.ToLower();//TODO:blog.  StreamingAssets 目录下读取资源，WinEditor下是不区分大小写的，android平台区分大小写


            AssetBundle ab = null;
            string path = "";
            if (IOTools.IsResInPDir(name))
            {
                path = IOTools.GetResPersistantPath(name);
            }
            else
            {
                path = IOTools.GetResStreamingPath(name);
            }


             //Debug.Log("loadAB--->" + name + ",  path:" + path);
            ab = AssetBundle.LoadFromFile(path);
            if (ab == null)
            {
                Debug.LogError("error while loadAb:abName:" + name + ", path:" + path);
            }

            return ab;
        }

    }
}