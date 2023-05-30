using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Window
{
    /// <summary>
    /// Area is the inner partial area of a window
    /// Used to help window to handle with complex logic
    /// </summary>
    public class Area : UIRoot
    {
        /// <summary>
        /// The holder window of this page
        /// </summary>
        public Window parentWindow;
        public Area(GameObject obj, Window window, params object[] paras)
        {
            this.rootObj = obj;
            this.parentWindow = window;
            AutoLinkUI(this);
            Init(paras);
            AddUIEventListener();
        }
        public virtual void Init(params object[] paras)
        {

        }

        public virtual void AddUIEventListener()
        {

        }


        public virtual void RemoveUIEventListener()
        {

        }


        public virtual void Show()
        {
            this.rootObj.SetActive(true);
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