using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.TimerTween;
using ZGame.Window;


[IgnoreWindowNameGather]
public class NetMaskWindow : Window
{
    Timer rollTimer;
    long rollTimerId;

    public Transform ui_bgTran;
    public Transform ui_rollTran;
    float eulerZ = 0f;

    public NetMaskWindow(GameObject obj, string windowName) : base(obj, windowName)
    {

    }

    public override void Show(string layerName, params object[] datas)
    {
        base.Show(layerName, datas);
        this.ui_bgTran.gameObject.SetActive(false);
        this.ui_rollTran.gameObject.SetActive(false);

    }
    public override void Init(string windowName, GameObject obj)
    {
        base.Init(windowName, obj);
        this.hideRoll();
    }
    void showBg()
    {
        this.ui_bgTran.gameObject.SetActive(true);
    }
    void hideBg()
    {
        this.ui_bgTran.gameObject.SetActive(false);
    }
    void showRoll()
    {
        ui_rollTran.gameObject.SetActive(true);

        TimerTween.Cancel(rollTimer, rollTimerId);

        rollTimer = TimerTween.Repeat(0.03f, () =>
        {
            if (eulerZ - 0.03f <= -360f)
            {
                eulerZ += 360f;
            }
            eulerZ -= 2f;

            ui_rollTran.localEulerAngles = new Vector3(0, 0, eulerZ);
            return true;
        }, true).SetTag("NetMaskRollTimer");
        rollTimer.Start(out rollTimerId);

    }
    void hideRoll()
    {
        TimerTween.Cancel(rollTimer, rollTimerId);
        ui_rollTran.gameObject.SetActive(false);
    }

    public override void HandleMessage(int msgId, params object[] paras)
    {
        if (msgId == WindowMsgID.OnShowNetMask)
        {
            this.showBg();
        }
        else if (msgId == WindowMsgID.OnHideNetMask)
        {
            this.hideBg();
            this.hideRoll();
        }
        else if (msgId == WindowMsgID.OnShowNetMaskWithRoll)
        {
            this.showBg();
            this.showRoll();
        }
    }
    public override void Destroy()
    {
        base.Destroy();
        TimerTween.Cancel(rollTimer, rollTimerId);
    }
}
