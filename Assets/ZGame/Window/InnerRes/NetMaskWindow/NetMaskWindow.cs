using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.TimerTween;
using ZGame.Window;

public class NetMaskWindow : Window
{
    Timer rollTimer;
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
        if (rollTimer != null)
        {
            rollTimer.Cancel();
            rollTimer = null;
        }
        rollTimer = TimerTween.Repeat(0.03f, () =>
        {

            if (eulerZ - 0.03f <= -360f)
            {
                eulerZ += 360f;
            }
            eulerZ -= 0.03f;

            ui_rollTran.localEulerAngles = new Vector3(0, 0, eulerZ);
            return true;
        }, true);
        rollTimer.Start();

    }
    void hideRoll()
    {
        if (rollTimer != null)
        {
            rollTimer.Cancel();
            rollTimer = null;
        }
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
        if (rollTimer != null)
        {
            rollTimer.Cancel();
            rollTimer = null;
        }
    }
}
