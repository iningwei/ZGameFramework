using System.Collections;
using System.Collections.Generic;
#if UseTMP
using TMPro;
#endif
using UnityEngine;
using UnityEngine.UI;
using ZGame;
using ZGame.TimerTween;
using ZGame.Window;
public enum TipLevel
{
    Msg = 1,
    Warnning = 2,
    Error = 3,
}

[IgnoreWindowNameGather]
public class TipWindow : Window
{
    public Transform ui_TipTran;
#if UseTMP
    public TextMeshProUGUI ui_TipTxt;
#else
    public Text ui_TipTxt;
#endif


    Timer delayHideTimer;
    long delayHideTimerId;
    void showTip(string tip)
    {
        TimerTween.Cancel(this.delayHideTimer, delayHideTimerId);

        this.ui_TipTran.gameObject.SetActive(true);
        this.ui_TipTxt.text = tip;


        this.delayHideTimer = TimerTween.Delay(5f, () =>
        {
            this.ui_TipTran.gameObject.SetActive(false);
        }).SetTag("DelayHideTip");
        this.delayHideTimer.Start(out delayHideTimerId);

    }
    public TipWindow(GameObject obj, string windowName) : base(obj, windowName)
    {
    }


    public override void Init(string windowName, GameObject obj)
    {
        base.Init(windowName, obj);
        ui_TipTran.gameObject.SetActive(false);

    }
    public override void Show(string layerName, params object[] datas)
    {
        base.Show(layerName, datas);
    }

    public override void HandleMessage(int msgId, params object[] paras)
    {
        if (msgId == 1)
        {
            var tipContent = paras[0].ToString();
            var tipLevel = (TipLevel)paras[1];
            this.showTip(tipContent);


            if (tipLevel == TipLevel.Msg)
            {
                Debug.Log("show tip:" + tipContent);
            }
            else if (tipLevel == TipLevel.Warnning)
            {
                Debug.LogWarning("show tip:" + tipContent);
            }
            else if (tipLevel == TipLevel.Error)
            {
                Debug.LogError("show tip:" + tipContent);
            }
        }
    }
}
