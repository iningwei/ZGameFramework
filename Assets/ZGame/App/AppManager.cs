using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ZGame
{
    public class AppManager : SingletonMonoBehaviour<AppManager>
    {
        public string LanguageType = "CN";



        Action<bool> appPause = (b) =>
    {
            //Debug.Log("app pause:" + b);
        };
        Action appExit = () =>
        {

        };
        Action<bool> appFocus = (b) =>
        {
            //Debug.Log("app focus:" + b);
            Time.timeScale = b ? 1 : 0;
        };

        public bool IsFirstGame = false;
        public void Init()
        {
            string v = PlayerPrefs.GetString("FirstGame", "0");
            if (v == "0")
            {
                IsFirstGame = true;
                PlayerPrefs.SetString("FirstGame", "1");
                PlayerPrefs.Save();
            }
            else
            {
                IsFirstGame = false;
            }

#if UNITY_EDITOR
            LanguageType = "EN";
           
#elif UNITY_IOS
          LanguageType = "EN";
#else
        LanguageType = "EN";
         // var cc = new AndroidJavaClass("com.zgame.sdk.CountyCode");
         //    string code;
         //    using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
         //    {
         //        object jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
         //        code = cc.CallStatic<string>("Get", jo); 
         //    }
         //   LanguageType = code;
#endif
        }

        public void RegisterAppFocusAct(Action<bool> act)
        {
            this.appFocus += act;
        }
        public void UnRegisterAppFocusAct(Action<bool> act)
        {
            this.appFocus -= act;
        }


        public void RegisterAppExitAct(Action act)
        {
            this.appExit += act;
        }
        public void UnRegisterAppExitAct(Action act)
        {
            this.appExit -= act;
        }


        public void RegisterAppPauseAct(Action<bool> act)
        {
            this.appPause += act;
        }

        public void UnRegisterAppPauseAct(Action<bool> act)
        {
            this.appPause -= act;
        }
        private void OnApplicationQuit()
        {
            appExit();
        }
        private void OnApplicationFocus(bool focus)
        {
            appFocus(focus);
        }
        private void OnApplicationPause(bool pause)
        {
            appPause(pause);
        }
    }
}