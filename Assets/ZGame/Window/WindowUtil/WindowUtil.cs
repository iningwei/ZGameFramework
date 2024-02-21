using System.Collections;
using System.Collections.Generic;
#if UseTMP
using TMPro;
#endif
using UnityEngine;
using UnityEngine.UI;
using ZGame;
using ZGame.Window;

public class WindowUtil
{
    public static void ShowNetMask()
    {
        WindowManager.Instance.SendWindowMessage(WindowNames.NetMaskWindow, 1);
    }
    public static void HideNetMask()
    {
        WindowManager.Instance.SendWindowMessage(WindowNames.NetMaskWindow, 0);
    }

    public static void ShowTip(string tipContent)
    {
        ShowTip(tipContent, TipLevel.Msg);
    }
    public static void ShowTip(string tipContent, TipLevel tipLevel)
    {
        WindowManager.Instance.SendWindowMessage(WindowNames.TipWindow, 1, tipContent, tipLevel);
    }
#if UseTMP
    public static void SetAppVersionDes(TextMeshProUGUI targetDesTxt)
    {
        var localResVersion = PlayerPrefs.GetString("resversion_" + Config.appVersion, "-1");
        if (localResVersion == "-1")
        {
            localResVersion = Config.resVersion;
        }
        targetDesTxt.text = "version:" + Config.appVersion + "_" + Config.resVersion + "_" + localResVersion;
    }
#else
    public static void SetAppVersionDes(Text targetDesTxt)
    {
        var localResVersion = PlayerPrefs.GetString("resversion_" + Config.appVersion, "-1");
        if (localResVersion == "-1")
        {
            localResVersion = Config.resVersion;
        }
        targetDesTxt.text = "version:" + Config.appVersion + "_" + Config.resVersion + "_" + localResVersion;
    }
#endif
}
