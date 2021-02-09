using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace ZGame.Window
{
    public class UIRoot
    {
        public GameObject rootObj;

        public void AutoLinkUI(UIRoot uiRoot)
        {
            Type trueType = uiRoot.GetType();
            //Debug.Log("AutoLinkUI, trueType:" + trueType.Name.ToString());

            FieldInfo[] fields = trueType.GetFields();//get all public fields
            foreach (var field in fields)
            {
                if (!field.Name.Contains("ui_"))
                {
                    continue;
                }

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
}
