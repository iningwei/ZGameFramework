using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ZGame.Net.Tcp;
using ZGame.HotUpdate;

namespace ZGame
{
    public class AppManager : SingletonMonoBehaviour<AppManager>
    {
        public string languageType = "EN";
        public LuaSocketManager luaSocketManager = new LuaSocketManager();


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
            //Time.timeScale = b ? 1 : 0;
        };


        [HideInInspector]
        public PackType packType = PackType.DEV;
        [HideInInspector]


        //使用原始Lua脚本的方式
        //当且仅当在Editor和未开启HOTUPDATE宏的情况下才使用
        public bool UseOriginLuaScript = false;

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


            Screen.sleepTimeout = SleepTimeout.NeverSleep;//不息屏
            Application.runInBackground = true;//这个只对pc有效， ios、安卓无效
            Application.targetFrameRate = 30;//限帧

#if DEV
        packType = PackType.DEV;
#elif PUB
        packType = PackType.PUB;
#elif AUDIT
        packType = PackType.AUDIT;
#elif TEST
        packType = PackType.TEST;
#else
            packType = PackType.DEV;
#endif


#if UNITY_EDITOR && !HOTUPDATE
            UseOriginLuaScript = true;
#else
            UseOriginLuaScript = false;
#endif


            ScriptManager.Instance.Init();
        }


        private void Update()
        {
            luaSocketManager.Update();
        }
        private void LateUpdate()
        {
            luaSocketManager.LateUpdate();
        }
        private void OnDestroy()
        {
            luaSocketManager.Destroy();
        }

        public void Quit()
        {
            Application.Quit();
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