using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ZGame.Net.Tcp;
using ZGame.Window;
using UnityEngine.UI;
using ZGame.cc;
using ScreenOrientation = ZGame.ScreenOrientation;
using System.Globalization;

namespace ZGame
{
    public class AppManager : SingletonMonoBehaviour<AppManager>
    {
        public string languageType = "EN";
        [HideInInspector]
        public int curUsedServerType = 0;
        [HideInInspector]
        public bool isLogEventToSDK = false;



        Action<bool> appPause = (b) =>
        {
        };
        Action appExit = () =>
        {
        };
        Action<bool> appFocus = (b) =>
        {
        };


        public Transform launcherRootNode;
        public void Init(Transform launcherRootNode)
        {
            this.launcherRootNode = launcherRootNode;
            SDK.SDKTools.Init();


            //Physics.autoSimulation = false;//若项目中未用到物理模拟，可关闭。避免无谓的物理模拟开销, https://answer.uwa4d.com/question/5cf22493d27511377098274b

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.runInBackground = true;//这个只对pc有效， ios、安卓无效  
            Application.targetFrameRate = 60;//设置最大60帧，方便开发阶段观测不同机器的性能

            var resolution = Screen.currentResolution;
            var refreshRate = resolution.refreshRate;
            DebugExt.Log("this device's refreshRate is:" + refreshRate);

            //force lower ratio aspect
            //if (Application.platform == RuntimePlatform.Android)
            //{
            //    float rotio = Screen.width * 1f / Screen.height;
            //    if (Screen.width > 1280)
            //        Screen.SetResolution(1280, (int)(1280 / rotio), true);
            //}

            //RenderSettings.fog = false;

            bool hdrSupport = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.DefaultHDR);
            DebugExt.Log("platform support hdr:" + hdrSupport);

            CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");//设置全局文化标准，避免德标下float.Parse转换失败

            var reporterObj = launcherRootNode.Find("Reporter").gameObject;
            if (Config.isShowReporter)
            {
                reporterObj.gameObject.SetActive(true);
            }
            else
            {
                reporterObj.gameObject.SetActive(false);
            }

            WindowManager.Instance.Init(launcherRootNode);

            WindowManager.Instance.ShowWindow(WindowNames.NetMaskWindow, WindowLayer.NetMask, false, true, true, null);
            WindowManager.Instance.ShowWindow(WindowNames.TipWindow, WindowLayer.Msg, false, true, true, null);

            Debug.Log("show firstOpenWindowName:" + Config.firstOpenWindowName);
            WindowManager.Instance.ShowWindow(Config.firstOpenWindowName, WindowLayer.Hud, false, false, true, null);



#if UNITY_ANDROID || UNITY_IOS
            //屏幕适配相关 
            if (Config.screenOrientation == (int)ScreenOrientation.Portrait)
            {
                ////WindowManager.Instance.UIVerticalFitPad();
                WindowManager.Instance.UIVerticalFitSafeArea();
            }
            else if (Config.screenOrientation == (int)ScreenOrientation.Landscape)
            {
                //Debug.Log("TODO:");
                //WindowManager.Instance.UIHorizentalFitSafeArea();
            }
            else
            {
                Debug.LogError("unsupported screenOrientation:" + Config.screenOrientation);
            }
#endif

        }

        //GC.Collect();can only remove managered memory, do nothing with native memory such as mats、shaders、textures...
        //Resources.UnloadUnusedAssets();can work with native memory,also it's said that in this method's inner it called GC.Collect();        
        float memeryClearDuration = 120f;
        private void Update()
        {

            //////this.memeryClearDuration -= Time.deltaTime;
            //////if (this.memeryClearDuration < 0)
            //////{
            //////    this.memeryClearDuration = 120f;
            //////    Resources.UnloadUnusedAssets();
            //////    //GC.Collect();
            //////}
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