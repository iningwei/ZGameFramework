using System;
using UnityEngine;
using ZGame.Event;

namespace ZGame.Ress.AB
{
    public class ABPrefab
    {
        /// <summary>
        /// name不带后缀，不带前缀
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static AssetBundle LoadAB(string name, ABType abType)
        {

            string nameNew = ABTypeUtil.GetPreFix(abType) + name;
            return AB.Load(nameNew);
        }


        /// <summary>
        /// name不带后缀，不带前缀
        /// </summary>
        /// <param name="name">name为2D场景对应的预制件名称</param>
        /// <returns></returns>
        public static GameObject Load(string name, ABType abType)
        {
            if (abType != ABType.Effect && abType != ABType.Window && abType != ABType.OtherPrefab)
            {
                Debug.LogError("ABPrefab do not support load " + abType.ToString());
            }

            GameObject obj = null;
            AssetBundle ab = LoadAB(name, abType);

            obj = ab.LoadAsset(name) as GameObject;
            ab.Unload(false);
            obj = GameObject.Instantiate(obj);//由于上面获得的 obj，实质是预制件，故这里需要Instantiate

            PrefabRes res = null;
            if (abType == ABType.Effect)
            {
                res = new EffectRes(name, obj);
            }
            else if (abType == ABType.Window)
            {
                res = new WindowRes(name, obj);
            }
            else if (abType == ABType.OtherPrefab)
            {
                res = new OtherPrefabRes(name, obj);
            }
            else
            {
                Debug.LogError("TODO:");
            }

            EventDispatcher.Instance.DispatchEvent(EventID.OnABResLoaded, res);
            return obj;
        }
    }
}