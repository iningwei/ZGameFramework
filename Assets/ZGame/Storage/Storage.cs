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
        /*******App版本
         * 若为空字符，说明为第一次安装游戏，把cfg中的配置写入
         * 若和cfg中的不一致，说明之前已经安装过，需要清空PersistantDataPath，并清空用户信息*********/
        public static void SetAppVersion(string appVersion)
        {
            PlayerPrefs.SetString("AppVersion", appVersion);
        }


        public static string GetAppVersion()
        {
            return PlayerPrefs.GetString("AppVersion", "");
        }


        /***********记录用户的登录渠道。-1为未确定，对应第一次登陆；0为游客**************/
        public static int GetAuthChannel()
        {
            return PlayerPrefs.GetInt("AuthChannel", -1);
        }

        public static void SetAuthChannel(int id)
        {
            Debug.Log("setAuthChannel:" + id);
            PlayerPrefs.SetInt("AuthChannel", id);
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
            return PlayerPrefs.GetString("Language", "EN");
        }

        public static void SetAppLanguage(string code)
        {
            PlayerPrefs.SetString("Language", code);
            PlayerPrefs.Save();
        }
    }
}