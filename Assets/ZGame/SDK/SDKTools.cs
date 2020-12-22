using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZGame.SDK
{
    public class SDKTools
    {
        public static void Init()
        {
            FirebaseSdkManager.Instance.Init();
            FacebookSdkManager.Instance.Init();
            AnalyticsSdkManager.Instance.Init();
            AdSdkManager.Instance.Init();
            IAPManager.instance.Init();
        }
    }
}