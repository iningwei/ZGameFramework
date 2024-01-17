using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZGame
{
    /// <summary>
    /// 本地存储
    /// </summary>
    public class Storage
    {

        /***********记录用户的登录渠道。-1为未确定，对应第一次登陆；0为游客**************/
        public static int GetAuthChannel()
        {
            var channel = PlayerPrefs.GetInt("CurAuthChannel", -1);
            DebugExt.Log("get AuthChannel:" + channel);
            return channel;
        }

        public static void SetAuthChannel(int id)
        {
            DebugExt.Log("set AuthChannel:" + id);
            PlayerPrefs.SetInt("CurAuthChannel", id);
            PlayerPrefs.Save();
        }


        /*************本地记录的服务器端上次登录成功，返回的identityToken*********************/
        public static string GetServerToken()
        {
            return PlayerPrefs.GetString("ServerToken", "");
        }

        public static void SetServerToken(string token)
        {
            PlayerPrefs.SetString("ServerToken", token);
            PlayerPrefs.Save();
        }


        public static void SetServerType(int s_type)
        {
            PlayerPrefs.SetInt("ServerType", s_type);
            PlayerPrefs.Save();
        }
        public static int GetServerType()
        {
            return PlayerPrefs.GetInt("ServerType", (int)ServerType.InnerDaiDev);
        }

        public static void SetSdkUserId(string userId)
        {
            PlayerPrefs.SetString("SdkUserId", userId);
            PlayerPrefs.Save();
        }
        public static string GetSdkUserId()
        {
            return PlayerPrefs.GetString("SdkUserId", "");
        }

        /// <summary>
        /// 清除用户数据
        /// </summary>
        public static void ClearUserData()
        {
            SetAuthChannel(-1);//恢复到无登录状态
            SetServerToken("");
        }








        /// <summary>
        /// 获得软件的语言设置
        /// </summary>
        /// <returns></returns>
        public static string GetAppLanguage()
        {
            string code = PlayerPrefs.GetString("Language", "");
            if (code == "")
            {
                Debug.Log("Application.systemLanguage:" + Application.systemLanguage);
                if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified)
                {
                    code = "CN";
                }
                else if (Application.systemLanguage == SystemLanguage.ChineseTraditional)
                {
                    code = "TC";
                }
                else if (Application.systemLanguage == SystemLanguage.English)
                {
                    code = "EN";
                }
                else if (Application.systemLanguage == SystemLanguage.Unknown)
                {
                    code = "EN";
                }
                else
                {
                    code = "CN";
                }
                SetAppLanguage(code);
            }

            return code;
        }

        public static void SetAppLanguage(string code)
        {
            PlayerPrefs.SetString("Language", code);
            PlayerPrefs.Save();
            Debug.Log("set language code:" + code);
        }

        public static void SetFCMToken(string token)
        {
            PlayerPrefs.SetString("FCMToken", token);
            PlayerPrefs.Save();
        }
        public static string GetFCMToken()
        {
            return PlayerPrefs.GetString("FCMToken", "");
        }


        public static void SetSoundValue(float v)
        {
            PlayerPrefs.SetFloat("SoundValue", v);
            PlayerPrefs.Save();
        }

        public static float GetSoundValue()
        {
            return PlayerPrefs.GetFloat("SoundValue", 1f);
        }

        public static void SetAppScreenBrightness(int v)
        {
            PlayerPrefs.SetInt("AppScreenBrightness", v);
            PlayerPrefs.Save();
        }

        public static int GetAppScreenBrightness()
        {
            return PlayerPrefs.GetInt("AppScreenBrightness", 190);
        }


    }
}