using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Event;
using ZGame.Ress.AB;
using ZGame.Ress.AB.Holder;

namespace ZGame.Ress
{
    public class GameObjectHelper
    {

        public static GameObject Instantiate(GameObject target)
        {
            if (target.GetComponent<DynamicCompInfoHolder>() == null)
            {
                Debug.LogError("The instantiate target have no component DynamicCompInfoHolder attached:" + target.GetHierarchy());
                return null;
            }
            GameObject resultObj = GameObject.Instantiate(target);

            EventDispatcher.Instance.DispatchEvent(EventID.OnGameObjectInstantiate, resultObj);

            return resultObj;
        }


        public static void Destroy(UnityEngine.Object target, float time)
        {
            if (target is GameObject)
            {
                var targetGameObj = target as GameObject;
                if (targetGameObj == null)
                {
                    return;
                }
                CompInfoHolder compInfoHolder = targetGameObj.GetComponent<CompInfoHolder>();
                if (compInfoHolder == null)
                {
                    if (Config.resLoadType == (int)ResLoadType.AssetBundle)
                    {
                        Debug.LogError("error, target have no CompInfoHolder , can not destroy, please check:" + targetGameObj.GetHierarchy());
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(target, time);
                    }

                }
                else
                {
                    if (compInfoHolder is RootCompInfoHolder)
                    {
                        UnityEngine.Object.Destroy(target, time);
                    }
                    else if (compInfoHolder is DynamicCompInfoHolder)
                    {
                        UnityEngine.Object.Destroy(target, time);
                    }
                }
            }
        }

        public static void DestroyImmediate(UnityEngine.Object target)
        {
            if (target is GameObject)
            {
                var targetGameObj = target as GameObject;
                if (targetGameObj == null)
                {
                    return;
                }
                CompInfoHolder compInfoHolder = targetGameObj.GetComponent<CompInfoHolder>();
                if (compInfoHolder == null)
                {
                    Debug.LogError("error, target have no compInfoHolder , can not destroy, please check:" + targetGameObj.GetHierarchy());
                }
                else
                {
                    if (compInfoHolder is RootCompInfoHolder)
                    {
                        UnityEngine.Object.DestroyImmediate(target);
                    }
                    else if (compInfoHolder is DynamicCompInfoHolder)
                    {
                        UnityEngine.Object.DestroyImmediate(target);
                    }
                }
            }

        }

    }
}