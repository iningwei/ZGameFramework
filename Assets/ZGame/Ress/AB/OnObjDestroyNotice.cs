using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Event;
using ZGame.Ress.AB.Holder;

namespace ZGame.Ress.AB
{
    public class OnObjDestroyNotice : MonoBehaviour
    {
        bool noticed = false;

        private void OnDestroy()
        {
            this.Notice();
        }


        public void Notice()
        {
            if (noticed)
            {
                return;
            }
            noticed = true;

            Transform[] childs = this.GetComponentsInChildren<Transform>(true);//contain self,and all children(included unactive children)
            Transform child = null;
            var count = childs.Length;
            //Debug.Log("OnObjDestroyNotice,dispatch to childs,count:" + count);

            for (int i = 0; i < count; i++)
            {
                child = childs[i];
                if (child.gameObject == this.gameObject)
                {
                    EventDispatcher.Instance.DispatchEvent(EventID.OnRootObjDestroy, child.gameObject);
                }

                EventDispatcher.Instance.DispatchEvent(EventID.OnChildObjDestroy, child.gameObject);

            }
        }
    }
}