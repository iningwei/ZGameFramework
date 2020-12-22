using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ZGame.Net
{
    public class NetTool
    {
        public static void CheckNetConnectivity(bool overWall, Action successCallback, Action failCallback)
        {
            //////这种方式获得的不准
            //////https://docs.unity3d.com/ScriptReference/Application-internetReachability.html
            /////https://stackoverflow.com/questions/57929106/how-to-properly-check-for-internet-connection-in-unity/57931517#57931517
            /////https://issuetracker.unity3d.com/issues/android-application-dot-internetreachability-returns-notreachable-when-trying-to-receive-unitywebrequest?_ga=2.30583801.770569249.1579078184-493465460.1572247986
            /////公司测试手机遇到第一次Check的时候总是返回false的情况
            //if (Application.internetReachability == NetworkReachability.NotReachable)
            //{
            //    return false;
            //}
            //return true;

            string targetUrl = overWall ? "https://google.com" : "https://www.baidu.com";
            CoroutineManager.Singleton.AddCoroutine(CheckInternetConnection(targetUrl, successCallback, failCallback));
        }

        static IEnumerator CheckInternetConnection(string echoServer, Action successCallback, Action failCallback)
        {
            bool result;
            using (var request = UnityWebRequest.Head(echoServer))
            {
                request.timeout = 5;
                yield return request.SendWebRequest();
                result = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;
                if (result == false)
                {
                    Debug.LogError("---------------------->request.error:" + request.error + "，request.responseCode：" + request.responseCode);
                }
            }
            if (result == true)
            {
                Debug.Log("internet connected：" + echoServer);
                if (successCallback != null)
                {
                    successCallback();
                }
            }
            else
            {
                Debug.Log("internet not connected：" + echoServer);
                if (failCallback != null)
                {
                    failCallback();
                }
            }
        }
    }
}