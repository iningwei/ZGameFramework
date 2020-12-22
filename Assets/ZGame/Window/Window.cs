using System;
using System.Reflection;
using DG.Tweening;
using UnityEngine;
using ZGame.cc;

namespace ZGame.Window
{
    public class Window : UIRoot
    {
        public string name;
        public string windowLayer;
        public bool isCache = false;

        public Transform ui_AniBg;
        public Window(GameObject obj, string windowName)
        {
            this.rootObj = obj;
            this.name = windowName;

            AutoLinkUI(this);
            AddUIEventListener();

            Init(windowName);
        }

        public virtual void AddUIEventListener()
        {
        }

        public virtual void RemoveUIEventListener()
        {
        }

        public virtual void Init(string windowName)
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
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localPosition = Vector3.zero;
            //////rt.anchorMin = Vector2.zero;
            //////rt.anchorMax = Vector2.one;
            //////rt.sizeDelta = Vector2.zero;
            //////rt.anchoredPosition = Vector2.zero;

            rt.localScale = Vector3.one;
            this.rootObj.SetActive(true);


            if (ui_AniBg != null)
            {
                DoShowAniamtion();
            }
        }

        public virtual void HandleMessage(int msgId, params object[] paras)
        {

        }

        public virtual void Update()
        {
            if (!rootObj.activeSelf)
            {
                return;
            }
            if (ui_AniBg != null && Time.timeScale == 1) // 防止同级页面覆盖或者多级弹窗关闭将游戏启动
            {
                Time.timeScale = 0;
            }
        }

        public virtual void Hide()
        {
            if (ui_AniBg != null && !name.Equals(WindowNames.SkinCollectWindow)) // 三级页面关闭时不恢复timeScale
            {
                Time.timeScale = 1;
            }
            this.rootObj.SetActive(false);
        }
        public virtual void Destroy()
        {
            RemoveUIEventListener();
            if (ui_AniBg != null && !name.Equals(WindowNames.SkinCollectWindow)) // 三级页面关闭时不恢复timeScale
            {
                Time.timeScale = 1;
            }
            GameObject.Destroy(this.rootObj);
        }

        public virtual void DoShowAniamtion()
        {
            if (ui_AniBg != null)
            {
                //////ui_AniBg.localScale = new Vector3(0.95f, 0.95f, 1f);
                // 只有在需要做动画的弹出中才暂停游戏
                Time.timeScale = 0;
                ui_AniBg.gameObject.RunTween(new ScaleTo(1f, Vector3.one).From(new Vector3(0.95f, 0.95f, 1f)).Easing(Ease.OutElastic).IgnoreTimeScale(true));
            }
        }

    }
}