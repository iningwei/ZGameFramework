using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Threading;
using ZGame.Ress.AB.Holder;
using ZGame.Ress.AB;
//////using Spine.Unity;

using ZGame.UGUIExtention;
using ZGame.Ress.Info;

namespace ZGame.RessEditor
{
    /// <summary>
    /// 打包基类
    /// </summary>
    public class BuildBase
    {
        public string abPrefix;
        public virtual bool Build(UnityEngine.Object obj)
        {
            Debug.LogError("can not build ab:" + AssetDatabase.GetAssetOrScenePath(obj));
            return false;
        }

        /// <summary>
        /// 对预制件进行统一的检测
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns>有错误则返回false</returns>
        public bool CheckPrefab(GameObject prefab)
        {
            string path = AssetDatabase.GetAssetPath(prefab);
            if (!path.EndsWith(".prefab"))
            {
                Debug.LogError("target not a prefab , path:" + path);
                return false;
            }



            var allChild = prefab.GetComponentsInChildren(typeof(Transform), true);
            Component tmpChild = null;
            for (int i = 0; i < allChild.Length; i++)
            {
                tmpChild = allChild[i];

                //检测是否在子物体上误添加了 RootCompInfoHolder
                if (tmpChild is Transform)
                {
                    var tran = tmpChild as Transform;
                    if (tran != prefab.transform)
                    {
                        if (tran.GetComponent<RootCompInfoHolder>() != null)
                        {
                            Debug.LogError("duplicate RootCompInfoHolder, path:" + tran.GetHierarchy());
                            return false;
                        }
                    }
                }


                if (BuildCommand.CheckValidOfU2D(tmpChild.gameObject) == false)
                {
                    return false;
                }
                if (BuildCommand.CheckValidOfRenderer(tmpChild.gameObject) == false)
                {
                    return false;
                }
                if (BuildCommand.CheckValidOfUGUI(tmpChild.gameObject) == false)
                {
                    return false;
                }
            }

            return true;
        }

    }
}