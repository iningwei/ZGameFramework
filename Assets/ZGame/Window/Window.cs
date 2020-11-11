﻿using System;
using System.Reflection;
using UnityEngine;

namespace ZGame.Window
{
    public class Window : UIRoot
    {
        public string name;
        public string windowLayer;

        public bool isCache = false;



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
        }

        public virtual void HandleMessage(int msgId, params object[] paras)
        {

        }

        public virtual void Update()
        {
        }

        public virtual void Hide()
        {
            this.rootObj.SetActive(false);
        }
        public virtual void Destroy()
        {
            RemoveUIEventListener();
            GameObject.Destroy(this.rootObj);
        }
    }
}