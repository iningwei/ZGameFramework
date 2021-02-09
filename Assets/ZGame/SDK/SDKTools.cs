using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;


namespace ZGame.SDK
{
    public class AuthChannelID
    {
        public static int NONE = -1;
        public static int VISITOR = 0;
        public static int FB = 1;
        public static int GP = 2;
        public static int APPLE = 3;
    }
    public class SDKTools
    {
        public static void Init()
        {
            //////FirebaseSdkManager.Instance.Init();
            //////FacebookSdkManager.Instance.Init();
            //////AnalyticsSdkManager.Instance.Init();
            //////AdSdkManager.Instance.Init();
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
    }
}