using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using ZGame;
using ZGame.Event;

namespace ZGame.Window
{
    public class UIRoot
    {
        public long id;
        public GameObject rootObj;


        public UIRoot(GameObject obj)
        {
            this.rootObj = obj;
        }

        public virtual void FillTextContent()
        {

        }
        public void AutoLinkUI(UIRoot uiRoot)
        {
            if (this.rootObj.GetComponent<UIRootTag>() == null)
            {
                Debug.LogError(this.rootObj.transform.GetHierarchy() + " have no UIRootTag attached");
                return;
            }
            Type trueType = uiRoot.GetType();

            FieldInfo[] fields = trueType.GetFields();//get all public fields
            foreach (var field in fields)
            {
                if (!field.Name.Contains("ui_"))
                {
                    continue;
                }
                if (field.FieldType == typeof(GameObject))
                {
                    Component tmpCom = this.rootObj.transform.DFS<Transform>(
                        field.Name.Replace("ui_", ""),
                        (tran) =>
                    {
                        if (tran != this.rootObj.transform && tran.GetComponent<UIRootTag>())
                        {
                            return false;
                        }
                        return true;
                    });

                    if (tmpCom != null)
                    {
                        field.SetValue(uiRoot, tmpCom.gameObject);
                    }
                }
                else
                {
                    Component tmpCom = this.rootObj.transform.DFS(field.FieldType,
                       field.Name.Replace("ui_", ""),
                       (tran) =>
                       {
                           if (tran != this.rootObj.transform && tran.GetComponent<UIRootTag>())
                           {
                               return false;
                           }
                           return true;
                       });

                    if (tmpCom != null)
                    {
                        field.SetValue(uiRoot, tmpCom);
                    }
                }
            }
        }


        public virtual void Update()
        {

        }
        public virtual void FixedUpdate()
        {

        }
        public virtual void LateUpdate()
        {

        }
        public virtual void AddEventListener()
        {
            EventDispatcher.Instance.AddListener(EventID.OnLanguageCodeChange, this.onLanguageCodeChange);
        }

        private void onLanguageCodeChange(string evtId, object[] paras)
        {
            this.FillTextContent();
        }

        public virtual void RemoveEventListener()
        {
            EventDispatcher.Instance.RemoveListener(EventID.OnLanguageCodeChange, this.onLanguageCodeChange);
        }
    }
}
