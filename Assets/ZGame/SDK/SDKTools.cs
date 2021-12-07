using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using ZGame.SDK.IAP;

namespace ZGame.SDK
{

    /// <summary>
    /// 游戏上线渠道
    /// </summary>
    public class GameChannelId
    {
        public static string AppleStore = "99";
        public static string GooglePlay = "100";
        public static string QieZi = "101";
    }

    /// <summary>
    /// 登录渠道
    /// </summary>
    public class AuthChannelID
    {
        public static int NONE = -1;
        public static int VISITOR = 0;
        public static int FB = 1;
        public static int GP = 2;
        public static int APPLE = 3;
    }



    /// <summary>
    /// 支付渠道Id(这里枚举需要和服务端的支付平台类型枚举保持一致)
    /// </summary>
    public class PaymentChannelId
    {
        public static int AppleStore = 1;
        public static int GooglePlay = 2;
    }


    public class SDKTools
    {
        public static void Init()
        {
            //IAPMgr的初始化移到获得商品id后再初始化
            //IAPMgr.Instance.Init();

            FirebaseSdkManager.Instance.Init();
            FacebookSdkManager.Instance.Init();
            //////AnalyticsSdkManager.Instance.Init();
            //////AdSdkManager.Instance.Init();
        }

   

        public static void IAPInit(string[] consumableProductIds, string[] nonConsumableProductIds, string[] subscriptionProductIds)
        {
            IAPMgr.Instance.Init(consumableProductIds, nonConsumableProductIds, subscriptionProductIds);
        }

        public static string GetOSType()
        {
#if UNITY_EDITOR
            return "unity_editor";
#elif UNITY_ANDROID
        return "android";
#elif UNITY_IOS
        return "ios";
#endif
            return "unknown";
        }


        public static string GetDeviceId()
        {
#if  UNITY_EDITOR
            return "0_" + getGUID() + "@@" + getMacAddress();
#elif UNITY_ANDROID
            var uniqueID = new AndroidJavaClass("com.zgame.sdk.DeviceID");
            string id;
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                object jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                id = uniqueID.CallStatic<string>("Get", jo, false);
            }
            id = "1_" + id;
            Debug.Log("get device ID：" + id);
            return id;
#elif UNITY_IOS
         string sid = DeviceUniqueId();
          sid = sid.Replace("-", "");
                sid = sid.Substring(0, 32);
        sid="2_"+sid;
        Debug.Log("get device ID：" + sid);
                return sid;
#endif
            return "NONE";
        }

        public static void LoginFB()
        {
            FacebookSdkManager.Instance.Login();
        }
        public static void LogoutFB()
        {
            FacebookSdkManager.Instance.Logout();
        }

        //用户只要不卸载安装，那么guid就是唯一的（第一次设置的值）
        private static string getGUID()
        {
            string guidStr = PlayerPrefs.GetString("guid", "");

            if (guidStr == "")
            {
                guidStr = System.Guid.NewGuid().ToString();
                PlayerPrefs.SetString("guid", guidStr);
            }
            return guidStr;
        }

        private static string getMacAddress()
        {
            string physicalAddress = "None";

            NetworkInterface[] nice = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adaper in nice)
            {

                Debug.Log(adaper.Description);

                if (adaper.Description == "en0")
                {
                    physicalAddress = adaper.GetPhysicalAddress().ToString();
                    break;
                }
                else
                {
                    physicalAddress = adaper.GetPhysicalAddress().ToString();

                    if (physicalAddress != "")
                    {
                        break;
                    };
                }
            }

            return physicalAddress;
        }





        #region 支付
        public static void PurchaseProduct(string productID, Action onFailed, Action<string> onSuccess)
        {
            IAPMgr.Instance.PurchaseProduct(productID, onFailed, onSuccess);
        }


        public static void AddPayVerifyData(string inner_order_id, string inner_app_id, string receipt_data, int pay_platform)
        {
            PayVerify.Instance.AddPayVerifyData(inner_order_id, inner_app_id, receipt_data, pay_platform);

            PayVerify.Instance.WritePayVerifyDataToLocal();
        }



        public static void DeleteVerifyData(string inner_order_id)
        {
            PayVerify.Instance.DeleteVerifyData(inner_order_id);
            PayVerify.Instance.WritePayVerifyDataToLocal();
        }

        public static void ReadPayVerifyDataFromLocal()
        {
            PayVerify.Instance.ReadPayVerifyDataFromLocal();
        }

        /// <summary>
        /// 没有的话返回null
        /// </summary>
        /// <returns></returns>
        public static PayVerifyData GetFirstPayVerifyDataFromLocal()
        {
            var data = PayVerify.Instance.ReadFirstVerifyDataFromLocal();

            if (data != null)
            {
                Debug.Log("get lost order data, inner_order_id:" + data.inner_order_id);
            }
            else
            {
                Debug.Log("no lost order data！！");
            }
            return data;
        }

        #endregion
    }





}