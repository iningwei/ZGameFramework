using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Event;
using ZGame.Ress.AB.Holder;

namespace ZGame.Ress.AB
{
    public class OnObjDestroyNotice : MonoBehaviour
    {
        private void OnDestroy()
        {
            if (this.GetComponent<MatTextureHolder>() != null)
            {
                //通过父物体向子物体广播。可以避免有些子物体无法触发OnDestroy事件导致的问题
                Transform[] childs = this.GetComponentsInChildren<Transform>(true);//包括父物体，和所有未激活的子物体
                for (int i = 0; i < childs.Length; i++)
                {
                    if (childs[i].gameObject == this.gameObject)
                    {
                        EventDispatcher.Instance.DispatchEvent(EventID.OnRootObjDestroy, childs[i].gameObject);
                    }
                    else
                    {
                        EventDispatcher.Instance.DispatchEvent(EventID.OnChildObjDestroy, childs[i].gameObject);
                    }
                }
            }
        }
    }
}