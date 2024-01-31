using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using MiniJSON;
using ZGame;
using ZGame.Obfuscation;

public class ResUpdateTool : ScriptableWizard
{
     
    [MenuItem("HotUpdate/游戏热更/一键整理热更代码和资源")]
    public static void GenUpdateResList()
    {
        ScriptableWizard.DisplayWizard("一键整理热更代码和资源", typeof(ResUpdateTool), "确定", "取消");

    }

    void OnWizardUpdate()
    {

    }

    //点击 确认 按钮
    void OnWizardCreate()
    {
        Debug.Log("ResUpdateTool OnWizardCreate!");
        new HotResCollector().Build();
    }


    //点击取消按钮
    void OnWizardOtherButton()
    {
        Close();
    }
}
