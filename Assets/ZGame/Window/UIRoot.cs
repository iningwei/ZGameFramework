using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using ZGame;
namespace ZGame.Window
{
    public class UIRoot
    {
        public long id;
        public GameObject rootObj;

        public void AutoLinkUI(UIRoot uiRoot)
        {
            Type trueType = uiRoot.GetType();
            //DebugExt.Log("AutoLinkUI, trueType:" + trueType.Name.ToString());

            FieldInfo[] fields = trueType.GetFields();//get all public fields
            foreach (var field in fields)
            {
                if (!field.Name.Contains("ui_"))
                {
                    continue;
                }
                if (field.FieldType == typeof(GameObject))
                {
                    Component tmpCom = this.rootObj.transform.FindComponent(typeof(Transform), field.Name.Replace("ui_", ""), true);
                    if (tmpCom != null)
                    {
                        field.SetValue(uiRoot, tmpCom.gameObject);
                    }
                }
                else
                {
                    Component tmpCom = this.rootObj.transform.FindComponent(field.FieldType, field.Name.Replace("ui_", ""), true);
                    if (tmpCom == null)
                    {
                        //////Debug.LogWarning("window " + trueType.Name + ",can not find：" + field.Name.Replace("ui_", ""));
                        continue;
                    }
                    field.SetValue(uiRoot, tmpCom);
                }

            }
        }


        public virtual void Update()
        {

        }
        public virtual void AddEventListener()
        {

        }


        public virtual void RemoveEventListener()
        {

        }
    }
}
