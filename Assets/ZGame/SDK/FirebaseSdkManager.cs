//////using Firebase;
//////using Firebase.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ZGame.SDK
{
    public class FirebaseSdkManager : Singleton<FirebaseSdkManager>
    {
//////        bool isInit = false;
//////        Dictionary<string, string> remoteCfgCache = new Dictionary<string, string>();

//////        public void Init()
//////        {
//////            if (isInit)
//////            {
//////                return;
//////            }
//////            Debug.Log("Firebase sdk init");
//////            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
//////           {
//////               var dependencyStatus = task.Result;
//////               if (dependencyStatus == Firebase.DependencyStatus.Available)
//////               {
//////                   Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
//////                   isInit = true;

//////                   //拉远程配置
//////#if !UNITY_EDITOR
//////                    Debug.Log("fetch remote firebase config");
//////                    Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync(new TimeSpan(0));
//////                    fetchTask.ContinueWith(onFetchRemoteConfig);
//////#endif

//////#if !UNITY_EDITOR
//////                    //Debug.Log("register firebase message receive");
//////                    //Firebase.Messaging.FirebaseMessaging.MessageReceived += FirebaseMessaging_MessageReceived;
//////#endif
//////               }
//////               else
//////               {
//////                   Debug.LogError(System.String.Format(
//////                     "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
//////               }
//////           });
//////        }
//////        public bool CanUse()
//////        {
//////            return isInit;
//////        }

//////        void onFetchRemoteConfig(Task fetchTask)
//////        {
//////            if (fetchTask.IsCanceled)
//////            {
//////                Debug.Log("RemoteConfig Fetch canceled.");
//////            }
//////            else if (fetchTask.IsFaulted)
//////            {
//////                Debug.Log("RemoteConfig Fetch encountered an error.");
//////            }
//////            else if (fetchTask.IsCompleted)
//////            {
//////                Debug.Log("RemoteConfig Fetch completed successfully!");
//////            }

//////            switch (Firebase.RemoteConfig.FirebaseRemoteConfig.Info.LastFetchStatus)
//////            {
//////                case Firebase.RemoteConfig.LastFetchStatus.Success:
//////                    Debug.Log("Remote data loaded and ready.");
//////                    Firebase.RemoteConfig.FirebaseRemoteConfig.ActivateFetched();
//////                    foreach (var k in Firebase.RemoteConfig.FirebaseRemoteConfig.Keys)
//////                    {
//////                        remoteCfgCache[k] = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(k).StringValue;
//////                        Debug.Log("RemoteData:" + k + " , " + remoteCfgCache[k]);
//////                    }
//////                    break;
//////                case Firebase.RemoteConfig.LastFetchStatus.Failure:
//////                    switch (Firebase.RemoteConfig.FirebaseRemoteConfig.Info.LastFetchFailureReason)
//////                    {
//////                        case Firebase.RemoteConfig.FetchFailureReason.Error:
//////                            Debug.Log("Fetch failed for unknown reason");
//////                            break;
//////                        case Firebase.RemoteConfig.FetchFailureReason.Throttled:
//////                            Debug.Log("Fetch throttled until " +
//////                            Firebase.RemoteConfig.FirebaseRemoteConfig.Info.ThrottledEndTime);
//////                            break;
//////                    }
//////                    break;
//////                case Firebase.RemoteConfig.LastFetchStatus.Pending:
//////                    Debug.Log("Latest Fetch call still pending.");
//////                    break;
//////            }
//////        }




//////        public int GetRemoteConfigPara(string key, int defaultValue)
//////        {
//////            if (remoteCfgCache.TryGetValue(key, out var v))
//////            {
//////                return int.Parse(v);
//////            }
//////            return defaultValue;
//////        }

//////        public long GetRemoteConfigPara(string key, long defaultValue)
//////        {
//////            if (remoteCfgCache.TryGetValue(key, out var v))
//////            {
//////                return long.Parse(v);
//////            }
//////            return defaultValue;
//////        }


//////        public float GetRemoteConfigPara(string key, float defaultValue)
//////        {
//////            if (remoteCfgCache.TryGetValue(key, out var v))
//////            {
//////                return float.Parse(v);
//////            }
//////            return defaultValue;
//////        }


//////        public string GetRemoteConfigPara(string key, string defaultValue)
//////        {
//////            if (remoteCfgCache.TryGetValue(key, out var v))
//////            {
//////                return v;
//////            }
//////            return defaultValue;
//////        }

//////        public bool GetRemoteConfigPara(string key, bool defaultValue)
//////        {
//////            if (remoteCfgCache.TryGetValue(key, out var v))
//////            {
//////                return bool.Parse(v);
//////            }
//////            return defaultValue;
//////        }

//////        public object GetRemoteConfigPara(string key, object defaultValue)
//////        {
//////            if (remoteCfgCache.TryGetValue(key, out var v))
//////            {
//////                return (object)v;
//////            }

//////            return defaultValue;
//////        }

//////        public void LogEvent(string name)
//////        {
//////            if (CanUse())
//////            {
//////                FirebaseAnalytics.LogEvent(name);
//////            }
//////        }
//////        public void LogEvent(string name, string parameterName, string parameterValue)
//////        {
//////            if (CanUse())
//////            {
//////                FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
//////            }
//////        }

//////        public void LogEvent(string name, string parameterName1, string parameterValue1, string parameterName2, string parameterValue2)
//////        {
//////            if (CanUse())
//////            {
//////                FirebaseAnalytics.LogEvent(name,new Parameter( parameterName1, parameterValue1),new Parameter(parameterName2, parameterValue2));
//////            }
//////        }

//////        public void LogEvent(string name, string parameterName, int parameterValue)
//////        {
//////            if (CanUse())
//////            {
//////                FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
//////            }
//////        }
//////        public void LogEvent(string name, string parameterName, long parameterValue)
//////        {
//////            if (CanUse())
//////            {
//////                FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
//////            }
//////        }
//////        public void LogEvent(string name, string parameterName, float parameterValue)
//////        {
//////            if (CanUse())
//////            {
//////                FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
//////            }
//////        }
//////        public void LogEvent(string name, string parameterName, double parameterValue)
//////        {
//////            if (CanUse())
//////            {
//////                FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
//////            }
//////        }

//////        public void LogEvent(string name, params Parameter[] parameters)
//////        {
//////            if (CanUse())
//////            {
//////                FirebaseAnalytics.LogEvent(name, parameters);
//////            }
//////        }


//////        //private static void FirebaseMessaging_MessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
//////        //{
//////        //    //Debug.LogError("received message from:" + e.Message.From + ", data:" + e.Message.Data + ", rawData:" + e.Message.RawData);
//////        //    //注意Notification的内容并不在e.Message.Data内

//////        //    //////Debug.LogError("title:" + e.Message.Notification.Title);
//////        //    //////Debug.LogError("body:" + e.Message.Notification.Body);
//////        //    //格式：{"title": "t1", "body": "b1"}
//////        //    string toastStr = "{\"title\": \"" + e.Message.Notification.Title + "\",\"body\": \"" + e.Message.Notification.Body + "\"}";
//////        //    Debug.LogError("toastStr:" + toastStr);
//////        //    //////BTManager.instance.ToastMessage(toastStr, 0);
//////        //}
    }
}