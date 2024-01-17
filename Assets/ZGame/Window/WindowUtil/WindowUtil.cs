using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public static void ShowTip(string tipContent, TipLevel tipLevel)
    {
        WindowManager.Instance.SendWindowMessage(WindowNames.TipWindow, 1, tipContent, tipLevel);
    }
}
