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
            this.id = IdAssginer.GetID(IdAssginer.IdType.Area);
            this.rootObj = obj;
            this.parentWindow = window;
            AutoLinkUI(this);
            Init(paras);
            AddEventListener();

            window.AddArea(this);
        }
        public virtual void Init(params object[] paras)
        {

        }




        public virtual void Show()
        {
            this.rootObj.SetActive(true);
        }

        public virtual void Hide()
        {
            this.rootObj.SetActive(false);
        }

        public virtual void Destroy()
        {
            RemoveEventListener();
            GameObject.Destroy(this.rootObj);
        }

    }
}