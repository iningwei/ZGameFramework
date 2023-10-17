using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ZGame;

public class ResUpdateTool2 : Editor
{
    [MenuItem("HotUpdate/游戏热更/获得当前资源版本信息")]
    public static void GetResVersion()
    {
        var localResVersion = PlayerPrefs.GetString("resversion_" + Config.appVersion, "-1");
        if (localResVersion == "-1")
        {
            localResVersion = Config.resVersion;
        }

        Debug.Log("resversion_" + Config.appVersion + ", " + " localResVersion:" + localResVersion.ToString());
    }


    [MenuItem("HotUpdate/游戏热更/获得当前资源版本 zipped信息")]
    public static void GetMaxZippedIndex()
    {
        var localResVersion = PlayerPrefs.GetString("resversion_" + Config.appVersion, "-1");
        if (localResVersion == "-1")
        {
            localResVersion = Config.resVersion;
        }


        int index = PlayerPrefs.GetInt("maxzippedindex_" + Config.appVersion + "_" + localResVersion, -1);

        Debug.Log("maxzippedindex：" + index.ToString());
    }
    [MenuItem("HotUpdate/游戏热更/重置当前资源版本信息和zipped信息")]
    public static void DoReset()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("重置完成!");
    }

    [MenuItem("HotUpdate/游戏热更/重置当前资源版本信息")]
    public static void DoResetResVersion()
    {
        PlayerPrefs.SetString("resversion_" + Config.appVersion, "-1");
        PlayerPrefs.Save();
        Debug.Log("设置完成!");
    }
}
