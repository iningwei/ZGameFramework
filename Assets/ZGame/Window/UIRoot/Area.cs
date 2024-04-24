using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress;

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
        public bool initVisible = false;
        public Area(GameObject obj, Window window, bool initVisible, params object[] paras) : base(obj)
        {
            this.id = IdAssginer.GetID(IdAssginer.IdType.Area);

            this.parentWindow = window;
            this.initVisible = initVisible;
            AutoLinkUI(this);
            Init(paras);
            AddEventListener();
            this.FillTextContent();
            window.AddArea(this);
        }
        public virtual void Init(params object[] paras)
        {

        }


        public virtual void Show(params object[] paras)
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