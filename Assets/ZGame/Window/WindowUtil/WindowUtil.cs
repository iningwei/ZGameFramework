using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZGame;
using ZGame.Window;

public class WindowUtil
{
    public static void ShowNetMask()
    {
        WindowManager.Instance.SendWindowMessage(WindowNames.NetMaskWindow, WindowMsgID.OnShowNetMask);
    }


    public static void ShowNetMaskWithRoll()
    {
        WindowManager.Instance.SendWindowMessage(WindowNames.NetMaskWindow, WindowMsgID.OnShowNetMaskWithRoll);
    }
    public static void HideNetMask()
    {
        WindowManager.Instance.SendWindowMessage(WindowNames.NetMaskWindow, WindowMsgID.OnHideNetMask);
    }

    public static void ShowTip(string tipContent)
    {
        ShowTip(tipContent, TipLevel.Msg);
    }
    public static void ShowTip(string tipContent, TipLevel tipLevel)
    {
        WindowManager.Instance.SendWindowMessage(WindowNames.TipWindow, WindowMsgID.OnAddTip, tipContent, tipLevel);
    }


    public static void ShowMessageBox(string content, Action confirmCallback, bool layoutOnlyConfirm = false, string tipTitle = "提示")
    {
        WindowManager.Instance.ShowWindow(WindowNames.MessageBoxWindow, WindowLayer.Hud2, false, false, true, null, content, confirmCallback, layoutOnlyConfirm, tipTitle);
    }

    public static void SetAppVersionDes(TextMeshProUGUI targetDesTxt)
    {
        var localResVersion = PlayerPrefs.GetString("resversion_" + Config.appVersion, "-1");
        if (localResVersion == "-1")
        {
            localResVersion = Config.resVersion;
        }
        targetDesTxt.text = "v:" + Config.appVersion + "_" + Config.resVersion + "_" + localResVersion;
    }

}
