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
        public bool isCache = false;
        public bool neverClose = false;

        public Transform ui_AniBg;

        Action callbackOnWindowClose;
        Action callbackOnWindowHide;
        Action callbackOnWindowDestroy;

        Window upperWindow;//该窗体的上一层窗体，多用于该窗体关闭后，再打开上一层窗体的需求
        int showedCount = 0;
        Dictionary<long, Area> areaMap = new Dictionary<long, Area>();

        public void AddArea(Area area)
        {
            if (areaMap.ContainsKey(area.id) == false)
            {
                areaMap.Add(area.id, area);
            }
        }
        public void RemoveArea(Area area)
        {
            if (areaMap.ContainsKey(area.id))
            {
                areaMap.Remove(area.id);
                area.Destroy();
            }
        }


        public void SetUpperWindow(Window window)
        {
            this.upperWindow = window;
        }
        public Window GetUpperWindow()
        {
            return this.upperWindow;
        }
        public void RegisterCallbackOnWindowClose(Action callback)
        {
            callbackOnWindowClose += callback;
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
            this.id = IdAssginer.GetID(IdAssginer.IdType.Window);

            this.rootObj = obj;
            this.name = windowName;

            AutoLinkUI(this);

            Init(windowName, obj);
            AddEventListener();
        }


        public void Close()
        {
            callbackOnWindowClose?.Invoke();
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

            if (showedCount == 0)
            {
                if (ui_AniBg != null)
                {
                    DoShowAniamtion();
                }
                foreach (var area in areaMap)
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
            }
            else
            {
                var animator = this.rootObj.GetComponent<Animator>();
                if (animator != null)
                {
                    Debug.LogError("set animator unable");
                    animator.enabled = false;
                }
            }


            showedCount++;
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

        public virtual new void Update()
        {
            foreach (var area in areaMap)
            {
                if (area.Value.rootObj.activeInHierarchy)
                {
                    area.Value.Update();
                }
            }
        }
        public virtual new void LateUpdate()
        {
            foreach (var area in areaMap)
            {
                if (area.Value.rootObj.activeInHierarchy)
                {
                    area.Value.LateUpdate();
                }
            }
        }
        public virtual new void FixedUpdate()
        {
            foreach (var area in areaMap)
            {
                if (area.Value.rootObj.activeInHierarchy)
                {
                    area.Value.FixedUpdate();
                }
            }
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
            RemoveEventListener();
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

            foreach (var area in areaMap)
            {
                area.Value.Destroy();
            }

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