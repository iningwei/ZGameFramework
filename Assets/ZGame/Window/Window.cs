using System;
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
        public bool isCache = false;
        public bool neverClose = false;

        public Transform ui_AniBg;


        //Action<GameObject> callbackOnWindowCreated; 
        Action callbackOnWindowHide;
        Action callbackOnWindowDestroy;

        Window upperWindow;//该窗体的上一层窗体，多用于该窗体关闭后，再打开上一层窗体的需求
        public void SetUpperWindow(Window window)
        {
            this.upperWindow = window;
        }
        public Window GetUpperWindow()
        {
            return this.upperWindow;
        }

        public void RegisterCallbackOnWindowHide(Action callback)
        {
            callbackOnWindowHide += callback;
        }

        //public void RegisterCallbackOnWindowCreate

        public void RegisterCallbackOnWindowDestroy(Action callback)
        {
            callbackOnWindowDestroy += callback;
        }

        public Window(GameObject obj, string windowName)
        {
            this.rootObj = obj;
            this.name = windowName;

            AutoLinkUI(this);

            Init(windowName, obj);
            AddUIEventListener();
        }

        public virtual void AddUIEventListener()
        {
        }

        public virtual void RemoveUIEventListener()
        {
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
        }

        //linkwindow缩放为0后重新显示
        public virtual void ReShowForLink()
        {
            var rt = this.rootObj.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;
        }

        //scaleZero后的回调
        public virtual void ScaleZeroForLink()
        {
            Debug.LogError(name + " scaleZeroForLink");
            var rt = this.rootObj.GetComponent<RectTransform>();
            rt.localScale = Vector3.zero;
        }


        public virtual void HandleMessage(int msgId, params object[] paras)
        {

        }

        public virtual void Update()
        {

        }

        public virtual void Hide()
        {
            if (callbackOnWindowHide != null)
            {
                callbackOnWindowHide.Invoke();
            }
            this.rootObj.SetActive(false);
        }
        public virtual void Destroy(bool destroyImmediate)
        {
            RemoveUIEventListener();
            if (callbackOnWindowDestroy != null)
            {
                callbackOnWindowDestroy.Invoke();
            }
            callbackOnWindowHide = null;
            callbackOnWindowDestroy = null;

            if (destroyImmediate)
            {
                GameObjectHelper.DestroyImmediate(this.rootObj);
            }
            else
            {
                GameObjectHelper.Destroy(this.rootObj, 0);
            }


        }

        public virtual void DoShowAniamtion()
        {
            if (ui_AniBg != null)
            {
                //////ui_AniBg.localScale = new Vector3(0.95f, 0.95f, 1f);

                ui_AniBg.gameObject.RunTween(new ScaleTo(1f, Vector3.one).From(new Vector3(0.95f, 0.95f, 1f)).Easing(Ease.OutElastic).IgnoreTimeScale(true));
            }
        }

    }
}