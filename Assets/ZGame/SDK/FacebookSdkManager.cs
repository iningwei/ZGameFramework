using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Event;
using MiniJSON;
using System.IO;

namespace ZGame.SDK
{
    public class FacebookSdkManager : Singleton<FacebookSdkManager>
    {
        public void Init()
        {
            FB.Init(() =>
            {
                Debug.Log("FB Init success");
                FB.Mobile.SetAutoLogAppEventsEnabled(true);
                FB.ActivateApp();
            });

            AppManager.Instance.RegisterAppPauseAct(b =>
            {
                if (!b)
                {
                    if (FB.IsInitialized)
                    {
                        FB.ActivateApp();
                    }
                    else
                    {
                        FB.Init(() =>
                        {
                            FB.Mobile.SetAutoLogAppEventsEnabled(true);
                            FB.ActivateApp();
                        });
                    }
                }
            });
        }




        public void Login()
        {
            if (!FB.IsInitialized)
            {
                Debug.LogError("fb can not login，for it is not initialized");
                return;
            }

            Debug.Log("do fb login--->");
            //var perms = new List<string>() { "public_profile", "email" };
            //使用gaming_user_picture权限，在游戏类型app中才能获得fb帐号的头像，否则public_profile权限获得的是fb游戏账户的头像
            //https://developers.facebook.com/docs/games/social-and-retention/features/login-for-gaming
            var perms = new List<string>() { "gaming_user_picture", "email" };

            FB.LogInWithReadPermissions(perms, authCallback);

        }

        public void Logout()
        {
            FB.LogOut();
        }

        Action shareSuccessCallback;
        Action shareFailCallback;
        public void Share(string contentURL, Action successCallback, Action failCallback)
        {
            shareSuccessCallback = successCallback;
            shareFailCallback = failCallback;
            FB.ShareLink(new Uri(contentURL), callback: shareCallback);
        }

        private void shareCallback(IShareResult result)
        {
            if (result.Cancelled)
            {
                Debug.LogError("share cancelled");
                if (shareFailCallback != null)
                {
                    shareFailCallback();
                    shareFailCallback = null;
                }

            }
            else if (!String.IsNullOrEmpty(result.Error))
            {
                Debug.LogError("share error:" + result.Error);
                if (shareFailCallback != null)
                {
                    shareFailCallback();
                    shareFailCallback = null;
                }
            }
            else if (!String.IsNullOrEmpty(result.PostId))
            {
                Debug.LogError("share error, postId:" + result.PostId);
                if (shareFailCallback != null)
                {
                    shareFailCallback();
                    shareFailCallback = null;
                }
            }
            else
            {
                Debug.LogError("share success!");
                if (shareSuccessCallback != null)
                {
                    shareSuccessCallback();
                    shareSuccessCallback = null;
                }

            }
        }

        string userId;
        string sdkToken;

        private void authCallback(IResult result)
        {
            if (result == null)
            {
                Debug.LogError("fb authCallback Null response");
                EventDispatcher.Instance.DispatchEvent(EventID.SDKLoginFail, "Facebook auth have no response");
                return;
            }

            if (!string.IsNullOrEmpty(result.Error))
            {
                Debug.LogError("fb login fail:" + result.Error);
                EventDispatcher.Instance.DispatchEvent(EventID.SDKLoginFail, result.Error);
            }
            else if (result.Cancelled)
            {
                Debug.Log("user cancelled fb login");
                EventDispatcher.Instance.DispatchEvent(EventID.SDKLoginFail, "User Cancelled facebook login");
            }
            else if (!string.IsNullOrEmpty(result.RawResult))
            {
                Debug.Log("fb login success");
                //Debug.Log("RawResult:" + result.RawResult);

                var dic = result.ResultDictionary;
                foreach (KeyValuePair<string, object> item in dic)
                {
                    Debug.LogError("11111 key:" + item.Key + ",  value:" + item.Value.ToString());
                }
                //ResultDictionary 存储的就是RawResult中的键值对




                dic.TryGetValue("user_id", out userId);
                dic.TryGetValue("access_token", out sdkToken);
                Debug.Log("user_id:" + userId);
                Debug.Log("access_token:" + sdkToken);


                //TODO:要测试一下不通过二次请求，能否这里解析出name,picture等信息



                ///me?fields=first_name,last_name,gender,email,location{location{city,country,country_code}}
                //FB.API("/me?fields=first_name,last_name,gender,email", HttpMethod.GET, graphCallback);



                FB.API("/me?fields=first_name,last_name,gender,email,picture", HttpMethod.GET, graphCallback);


                //----------------->玩家头像获得方法汇总
                //法一
                //该方法也可获得头像url，url保存到server端，需要时根据url下载头像，但是根据该url再下载的图片默认尺寸是600*600的，因此比较耗时
                // FB.API("/me?fields=first_name,last_name,picture", HttpMethod.GET, graphCallback);

                //法二
                //获得头像也可使用下面接口，但是获得的并不是头像的url，而是 texture。且只能获得玩家自身头像。其他玩家无法处理
                //笔者测试，设置的width,height也不生效，图片还是原始尺寸
                //FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, pictureGraphCallback);

                //法三
                //也可通过玩家user_id和自己的access_token进行拼接url的方式，形如: http://graph.facebook.com/user_id/picture?width=240&height=240&type=square&access_token=xxx  user_id需要获取，传入;access_token需要传入
                //该url在网页中打开可以直接下载图片(图片尺寸也是600*600，设置的尺寸无效)， 但是通过unity的webrequest无法下载

                //法四
                //通过构建 https://graph.facebook.com/user_id/?fields=picture&width=240&height=240&type=square&access_token=XXX
                //可以获得一个json字符串
                //形如：
                //{ "picture":{ "data":{ "height":600,"is_silhouette":false,"url":"https:\/\/platform-lookaside.fbsbx.com\/platform\/profilepic\/?asid=102473365568759&gaming_photo_type=unified_picture&ext=1640317531&hash=AeSOOQjV1NjyT5HUu8A","width":600} },"id":"102473365568759"}
                //可见图片尺寸也是600,无法自定义尺寸， 通过json中的url，使用unitywebrequest可以下载图片
            }
            else
            {
                Debug.LogError("fb empty response");
                EventDispatcher.Instance.DispatchEvent(EventID.SDKLoginFail, "Facebook response have no data");
            }
        }

        private void pictureGraphCallback(IGraphResult result)
        {
            if (result != null && result.Error == null)
            {
                var tex = result.Texture;
                Debug.LogError("222,get picture，size:" + tex.width);
                var bytes = tex.EncodeToJPG();
                var filePath = Application.persistentDataPath + "/testzzzzzzzzz11111.jpg";
                File.WriteAllBytes(filePath, bytes);
            }
            else
            {
            }
        }

        private void graphCallback(IGraphResult result)
        {
            string firstname = "";
            string lastname = "";
            string gender = "";
            string email = "";
            //string location = "";
            object pictureData = null;
            string pictureUrl = "";
            if (result == null)
            {
                Debug.LogError("fb graphCallback Null response");
            }
            else
            {
                if (!string.IsNullOrEmpty(result.Error))
                {
                    Debug.LogError("fb got user message failed:" + result.Error);
                    EventDispatcher.Instance.DispatchEvent(EventID.SDKLoginFail, "Get user message failed!");
                }
                else
                {
                    //fb gender, location需要额外申请权限

                    result.ResultDictionary.TryGetValue("first_name", out firstname);
                    result.ResultDictionary.TryGetValue("last_name", out lastname);
                    result.ResultDictionary.TryGetValue("gender", out gender);
                    result.ResultDictionary.TryGetValue("email", out email);
                    //result.ResultDictionary.TryGetValue("location", out location);

                    result.ResultDictionary.TryGetValue("picture", out pictureData);
                    object pictureData2 = null;
                    (pictureData as Dictionary<string, object>).TryGetValue("data", out pictureData2);
                    (pictureData2 as Dictionary<string, object>).TryGetValue("url", out pictureUrl);
                    //Debug.LogError("pictureUrl:" + pictureUrl);
                }
            }


            string nick_name = (firstname + " " + lastname).Trim();

            EventDispatcher.Instance.DispatchEvent(EventID.SDKLoginCMP, AuthChannelID.FB + "|" + userId + "|" + sdkToken + "|" +
                nick_name + "|" + gender + "|" + email+"|"+pictureUrl);
        }

        private void onHideUnity(bool isUnityShown)
        {
            if (!isUnityShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }

        private void initCallback()
        {
            Debug.Log("fb sdk init finished");
            // Signal an app activation App Event
            //FB.Mobile.SetAutoLogAppEventsEnabled(true);
            FB.ActivateApp();
        }




        //////    public void LogEvent(string name, string parameterName1, object parameterValue1, string parameterName2, object parameterValue2)
        //////    {
        //////        if (!FB.IsInitialized)
        //////            return;
        //////        FB.LogAppEvent(name, null, new Dictionary<string, object>()
        //////    {
        //////        { parameterName1, parameterValue1 },
        //////        { parameterName2, parameterValue2 },
        //////    });
        //////    }

        //////    public void LogEvent(string name, string parameterName1, object parameterValue1, string parameterName2, object parameterValue2, string parameterName3, object parameterValue3)
        //////    {
        //////        if (!FB.IsInitialized)
        //////            return;
        //////        FB.LogAppEvent(name, null, new Dictionary<string, object>()
        //////    {
        //////        { parameterName1, parameterValue1 },
        //////        { parameterName2, parameterValue2 },
        //////        { parameterName3, parameterValue3 },
        //////    });
        //////    }
        //////    public void LogEvent(string name, string parameterName1, object parameterValue1, string parameterName2, object parameterValue2, string parameterName3, object parameterValue3, string parameterName4, object parameterValue4)
        //////    {
        //////        if (!FB.IsInitialized)
        //////            return;
        //////        FB.LogAppEvent(name, null, new Dictionary<string, object>()
        //////    {
        //////        { parameterName1, parameterValue1 },
        //////        { parameterName2, parameterValue2 },
        //////        { parameterName3, parameterValue3 },
        //////        { parameterName4, parameterValue4 },
        //////    });
        //////    }
        //////    public void LogEvent(string name, string parameterName, object parameterValue)
        //////    {
        //////        if (!FB.IsInitialized)
        //////            return;
        //////        FB.LogAppEvent(name, null, new Dictionary<string, object>()
        //////    {
        //////        { parameterName, parameterValue }
        //////    });
        //////    }

        //////    public void LogEvent(string name)
        //////    {
        //////        if (!FB.IsInitialized)
        //////            return;
        //////        FB.LogAppEvent(name, null, null);
        //////    }

        //////    public void LogEvent(string name, params object[] events)
        //////    {
        //////        if (!FB.IsInitialized)
        //////            return;
        //////        int count = events != null ? events.Length / 2 : 0;
        //////        if (count > 0)
        //////        {
        //////            Dictionary<string, object> param = new Dictionary<string, object>();
        //////            for (int i = 0; i < count; i++)
        //////            {
        //////                string key = events[i * 2] as string;
        //////                if (string.IsNullOrEmpty(key)) { Debug.LogError(" fetal error: " + name + ": key is not string:" + events[i * count]); continue; }
        //////                object value = events[i * 2 + 1];
        //////                param[key] = value;
        //////            }

        //////            FB.LogAppEvent(name, null, param);
        //////        }
        //////        else
        //////        {
        //////            FB.LogAppEvent(name, null, null);
        //////        }
        //////    }
    }
}