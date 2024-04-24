using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress;
using ZGame.Window;

namespace ZGame.Window
{
    public class NodeHolder : UIRoot
    {
        public Window parentWindow;
        public GameObject nodeItemPrototypeObj;
        public List<Node> nodes = new List<Node>();
        public NodeHolder(GameObject holderObj, Window window, GameObject nodeItemObj, params object[] paras) : base(holderObj)
        {
            nodeItemObj.SetActive(false);
            this.nodeItemPrototypeObj = nodeItemObj;

            this.id = IdAssginer.GetID(IdAssginer.IdType.NodeHolder);
            this.parentWindow = window;
            AutoLinkUI(this);
            Init(paras);
            AddEventListener();
            this.FillTextContent();
            window.AddNodeHolder(this);
        }


        public virtual void Init(params object[] paras)
        {
        }


        public virtual void Show(params object[] paras)
        {
            this.rootObj.SetActive(true);
            this.FillItems();
        }

        public virtual void FillItems()
        {
            
        }

        public virtual T AddNode<T>(GameObject nodePrefab, params object[] datas) where T : Node
        {
            GameObject obj = GameObjectHelper.Instantiate(nodePrefab);
            obj.transform.SetParent(this.rootObj.transform);
            obj.name = nodePrefab.name;//保持名字不变，即去掉(clone)
            T node = (T)Activator.CreateInstance(typeof(T), obj, this.rootObj.transform, datas);
            node.Show();
            this.nodes.Add(node);
            return node;
        }

        void clearNodes()
        {
            if (nodes.Count > 0)
            {
                for (int i = nodes.Count - 1; i >= 0; i--)
                {
                    nodes[i].Destroy();
                }
                nodes.Clear();
            }
        }
        public virtual void Hide()
        {
            this.rootObj.SetActive(false);
        }

        public virtual void Destroy()
        {
            RemoveEventListener();
            this.clearNodes();
            if (Config.resLoadType == (int)ResLoadType.AssetBundle)
            {
                GameObjectHelper.DestroyImmediate(this.rootObj);
            }
            else
            {
                GameObject.DestroyImmediate(this.rootObj);
            }
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].LateUpdate();
            }
        }
    }
}