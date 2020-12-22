using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.SDK
{
    public class EditorAdSdkManager : AdSdkBase
    {
        public override void Init()
        {
            Debug.Log("EditorAdSdk init....");
        }

        public override void ShowInterstitial(Action onNotAvailable, Action onShowSuccess, Action onShowFail, Action<bool> onAdClose = null)
        {
            onAdClose?.Invoke(true);
        }

        public override void ShowRewardedVideo(Action onNotAvailable, Action onShowSuccess, Action onShowFail, Action<bool> onAdClose = null)
        {
            if (this.IsRewardedVideoAvailable())
            {
                onAdClose?.Invoke(true);
            }
            else
            {
                onNotAvailable?.Invoke();
            }
        }

        public override bool IsRewardedVideoAvailable()
        {
            return true;
        }

    }
}