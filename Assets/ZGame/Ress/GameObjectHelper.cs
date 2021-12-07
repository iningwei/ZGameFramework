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

    



        public static void Destroy(GameObject target)
        {
         
            CompInfoHolder compInfoHolder = target.GetComponent<CompInfoHolder>();
            if (compInfoHolder == null)
            {
                Debug.LogError("error, target have no CompInfoHolder , can not destroy, please check:" + target.GetHierarchy());
            }
            else
            {
                if (compInfoHolder is RootCompInfoHolder)
                {
                    GameObject.Destroy(target);
                }
                else if (compInfoHolder is DynamicCompInfoHolder)
                {
                    GameObject.Destroy(target);
                }
            }
        }

        public static void DestroyImmediate(GameObject target)
        {
           
            CompInfoHolder compInfoHolder = target.GetComponent<CompInfoHolder>();
            if (compInfoHolder == null)
            {
                Debug.LogError("error, target have no compInfoHolder , can not destroy, please check:" + target.GetHierarchy());
            }
            else
            {
                if (compInfoHolder is RootCompInfoHolder)
                {
                    GameObject.DestroyImmediate(target);
                }
                else if (compInfoHolder is DynamicCompInfoHolder)
                {
                    GameObject.DestroyImmediate(target);
                }
            }
        }
    }
}