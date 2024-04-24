using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress;
using ZGame.Window;

namespace ZGame.Window
{
    public class Node : UIRoot
    {
        Transform parentTran;


        public Node(GameObject obj, Transform parent, params object[] paras) : base(obj)
        {
            this.id = IdAssginer.GetID(IdAssginer.IdType.Node);
            this.parentTran = parent;

            this.AutoLinkUI(this);
            this.Init(paras);
            this.AddEventListener();
            this.FillTextContent();
        }


        public virtual void Init(params object[] paras)
        {

        }


        public virtual void Show(params object[] paras)
        {
            this.rootObj.transform.SetParent(this.parentTran);
            this.rootObj.transform.ResetLocalPRS();
            this.rootObj.SetActive(true);
        }

        public virtual void Destroy()
        {
            RemoveEventListener();
            if (Config.resLoadType == (int)ResLoadType.AssetBundle)
            {
                GameObjectHelper.DestroyImmediate(this.rootObj);
            }
            else
            {
                GameObject.DestroyImmediate(this.rootObj);
            }
        }
    }
}