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
    public override void Init(string windowName, GameObject obj)
    {
        base.Init(windowName, obj);
        this.hideRoll();
    }
    void showRoll()
    {
        this.ui_bgTran.gameObject.SetActive(true);

        TimerTween.Cancel(rollTimer, rollTimerId);

        rollTimer = TimerTween.Repeat(0.03f, () =>
        {
            if (eulerZ - 0.03f <= -360f)
            {
                eulerZ += 360f;
            }
            eulerZ -= 0.03f;

            ui_rollTran.localEulerAngles = new Vector3(0, 0, eulerZ);
            return true;
        }, true).SetTag("NetMaskRollTimer");
        rollTimer.Start(out rollTimerId);

    }
    void hideRoll()
    {
        TimerTween.Cancel(rollTimer, rollTimerId);

        this.ui_bgTran.gameObject.SetActive(false);
    }

    public override void HandleMessage(int msgId, params object[] paras)
    {
        if (msgId == 1)
        {
            this.showRoll();
        }
        else if (msgId == 0)
        {
            this.hideRoll();
        }
    }
    public override void Destroy(bool destroyImmediate)
    {
        base.Destroy(destroyImmediate);
        TimerTween.Cancel(rollTimer, rollTimerId);
    }
}
