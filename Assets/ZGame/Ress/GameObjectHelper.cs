using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.Ress
{
    public class GameObjectHelper
    {
        public static void Destroy(GameObject target)
        {
            if (target.activeInHierarchy)
            {
                GameObject.Destroy(target);
            }
            else
            {
                var notice = target.GetComponent<OnObjDestroyNotice>();
                if (notice != null)
                {
                    notice.Notice();
                }

                GameObject.Destroy(target);
            }
        }

        public static void DestroyImmediate(GameObject target)
        {
            if (target.activeInHierarchy)
            {
                GameObject.DestroyImmediate(target);
            }
            else
            {
                var notice = target.GetComponent<OnObjDestroyNotice>();
                if (notice != null)
                {
                    notice.Notice();
                }

                GameObject.DestroyImmediate(target);
            }
        }
    }
}