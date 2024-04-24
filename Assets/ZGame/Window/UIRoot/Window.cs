using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ZGame.cc;
using ZGame.Ress;

namespace ZGame.Window
{
    public class Window : UIRoot
    {
        public string name;
        public string windowLayer;
        public bool neverClose = false;
        public bool isLinkWindow = false;

        public Transform ui_AniBg;

        Action callbackOnWindowClose;
        Action callbackOnWindowHide;
        Action callbackOnWindowHideForLink;
        Action callbackOnWindowBack;

        Action callbackOnWindowDestroy;


        Dictionary<long, Area> areaDic = new Dictionary<long, Area>();
        Dictionary<long, NodeHolder> nodeHolderDic = new Dictionary<long, NodeHolder>();
        public void AddArea(Area area)
        {
            if (areaDic.ContainsKey(area.id) == false)
            {
                areaDic.Add(area.id, area);
            }
        }
        public void RemoveArea(Area area)
        {
            if (areaDic.ContainsKey(area.id))
            {
                areaDic.Remove(area.id);
                area.Destroy();
            }
        }
        public void AddNodeHolder(NodeHolder holder)
        {
            if (nodeHolderDic.ContainsKey(holder.id) == false)
            {
                nodeHolderDic.Add(holder.id, holder);
            }
        }
        public void RemoveNodeHolder(NodeHolder holder)
        {
            if (nodeHolderDic.ContainsKey(holder.id))
            {
                nodeHolderDic.Remove(holder.id);
                holder.Destroy();
            }
        }


        public void RegisterCallbackOnWindowClose(Action callback)
        {
            callbackOnWindowClose += callback;
        }
        public void RegisterCallbackOnWindowHide(Action callback)
        {
            callbackOnWindowHide += callback;
        }
        public void RegisterCallbackOnWindowHideForLink(Action callback)
        {
            callbackOnWindowHideForLink += callback;
        }
        public void RegisterCallbackOnWindowBack(Action callback)
        {
            callbackOnWindowBack += callback;
        }
        public void RegisterCallbackOnWindowDestroy(Action callback)
        {
            callbackOnWindowDestroy += callback;
        }

        public Window(GameObject obj, string windowName) : base(obj)
        {
            this.id = IdAssginer.GetID(IdAssginer.IdType.Window);

            this.name = windowName;

            AutoLinkUI(this);
            Init(windowName, obj);
            AddEventListener();
            this.FillTextContent();
        }



        public virtual void Init(string windowName, GameObject obj)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="datas"> </param>
        public virtual void Show(string layerName, params object[] datas)
        {
            Debug.Log("show window:" + name);
            this.windowLayer = layerName;
            var rt = this.rootObj.GetComponent<RectTransform>();
            //rt.offsetMin = Vector2.zero;
            //rt.offsetMax = Vector2.zero;
            //rt.localPosition = Vector3.zero;


            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition3D = Vector3.zero;

            rt.localScale = Vector3.one;
            this.rootObj.SetActive(true);


            if (this.rootObj.GetComponent<WindowUniversalSafeAreaAdaptive>() == null)
            {
                this.rootObj.AddComponent<WindowUniversalSafeAreaAdaptive>();
            }


            if (ui_AniBg != null)
            {
                DoShowAniamtion();
            }
            foreach (var area in areaDic)
            {
                //显示area
                if (area.Value.initVisible)
                {
                    area.Value.Show();
                }
                else
                {
                    area.Value.Hide();
                }
            }

            foreach (var nodeHolder in nodeHolderDic)
            {
                nodeHolder.Value.Show();
            }
        }


        //linkwindow缩放为0后重新显示
        public virtual void ReShowForLink()
        {
            var rt = this.rootObj.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;
        }




        public virtual void HandleMessage(int msgId, params object[] paras)
        {

        }

        public virtual new void Update()
        {
            foreach (var area in areaDic)
            {
                if (area.Value.rootObj.activeInHierarchy)
                {
                    area.Value.Update();
                }
            }
            foreach (var node in nodeHolderDic)
            {
                if (node.Value.rootObj.activeInHierarchy)
                {
                    node.Value.Update();
                }
            }
        }
        public virtual new void LateUpdate()
        {
            foreach (var area in areaDic)
            {
                if (area.Value.rootObj.activeInHierarchy)
                {
                    area.Value.LateUpdate();
                }
            }
            foreach (var node in nodeHolderDic)
            {
                if (node.Value.rootObj.activeInHierarchy)
                {
                    node.Value.LateUpdate();
                }
            }
        }
        public virtual new void FixedUpdate()
        {
            foreach (var area in areaDic)
            {
                if (area.Value.rootObj.activeInHierarchy)
                {
                    area.Value.FixedUpdate();
                }
            }
            foreach (var node in nodeHolderDic)
            {
                if (node.Value.rootObj.activeInHierarchy)
                {
                    node.Value.FixedUpdate();
                }
            }
        }

        public void Back()
        {
            callbackOnWindowBack?.Invoke();
        }

        public void Close()
        {
            if (this.isLinkWindow)
            {
                WindowManager.Instance.ClearLinkedWindows();
            }
            callbackOnWindowClose?.Invoke();
        }
        public virtual void Hide()
        {
            if (this.isLinkWindow)
            {
                Debug.LogError(this.name + "  is link window,not allow hide");
            }
            else
            {
                callbackOnWindowHide?.Invoke();
            }


        }
        public virtual void HideForLink()
        {
            this.callbackOnWindowHideForLink?.Invoke();
        }


        public virtual void Destroy()
        {
            Debug.Log("destroy window:" + name);

            RemoveEventListener();

            callbackOnWindowDestroy?.Invoke();
            callbackOnWindowHide = null;
            callbackOnWindowHideForLink = null;
            callbackOnWindowBack = null;
            callbackOnWindowDestroy = null;

            foreach (var area in areaDic)
            {
                area.Value.Destroy();
            }
            foreach (var nodeHolder in nodeHolderDic)
            {
                nodeHolder.Value.Destroy();
            }


            GameObjectHelper.DestroyImmediate(this.rootObj);

        }

        public virtual void DoShowAniamtion()
        {
            if (ui_AniBg != null)
            {
                ui_AniBg.gameObject.RunTween(new ScaleTo(1f, Vector3.one).From(new Vector3(0.95f, 0.95f, 1f)).Easing(Ease.OutElastic).IgnoreTimeScale(true));
            }
        }

    }
}