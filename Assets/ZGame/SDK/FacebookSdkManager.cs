//////using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.SDK
{
    public class FacebookSdkManager : Singleton<FacebookSdkManager>
    {
        //////    public void Init()
        //////    {
        //////        FB.Init(() =>
        //////        {
        //////            Debug.Log("FB Init success");
        //////            FB.Mobile.SetAutoLogAppEventsEnabled(true);
        //////            FB.ActivateApp();
        //////        });

        //////        AppManager.Instance.RegisterAppPauseAct(b =>
        //////        {
        //////            if (!b)
        //////            {
        //////                if (FB.IsInitialized)
        //////                {
        //////                    FB.ActivateApp();
        //////                }
        //////                else
        //////                {
        //////                    FB.Init(() =>
        //////                    {
        //////                        FB.Mobile.SetAutoLogAppEventsEnabled(true);
        //////                        FB.ActivateApp();
        //////                    });
        //////                }
        //////            }
        //////        });
        //////    }





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