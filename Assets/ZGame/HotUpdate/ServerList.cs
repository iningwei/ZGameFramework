using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MiniJSON;
using ZGame;

public class AppMsgData
{
    public string CurMaxResVersion;
    public bool IsPassed;
    public AppMsgData(string curMaxResVersion, bool isPassed)
    {
        this.CurMaxResVersion = curMaxResVersion;
        this.IsPassed = isPassed;
    }
}

/// <summary>
/// 服务器列表
/// </summary>
public class ServerList : Singleton<ServerList>
{
    public string CurMaxAppVersion;
    public string CanRunMinAppVersion;

    public string JumpStoreKey;       //大版本更新对应的打开商店的地址 

    public Dictionary<string, AppMsgData> AppMsg = new Dictionary<string, AppMsgData>();//具体某个版本app信息

    public void Init(string json)
    {

        var dic = Json.Deserialize(json) as Dictionary<string, object>;

        this.CurMaxAppVersion = dic["CurMaxAppVersion"].ToString();
        this.CanRunMinAppVersion = dic["CanRunMinAppVersion"].ToString();

        this.JumpStoreKey = dic["JumpStoreKey"].ToString();





        var list = dic["AppMsg"] as List<object>;

        for (int i = 0; i < list.Count; i++)
        {
            var appDic = list[i] as Dictionary<string, object>;
            var appVersion = appDic["AppVersion"].ToString();
            var curMaxResVersion = appDic["CurMaxResVersion"].ToString();
            var isPassed = true;
            if (appDic.ContainsKey("IsPassed"))
            {
                isPassed = Boolean.Parse(appDic["IsPassed"].ToString());
            }

            AppMsg.Add(appVersion, new AppMsgData(curMaxResVersion, isPassed));
        }

    }


    public string GetAppMaxResVersion(string appVersion)
    {
        if (AppMsg.ContainsKey(appVersion))
        {
            return AppMsg[appVersion].CurMaxResVersion;
        }
        else
        {
            DebugExt.LogE($"GetAppMaxResVersion error, no appVersion:{appVersion} in serverlist");
            return "0";
        }
    }


    /// <summary>
    /// 审核通过状态
    /// </summary>
    /// <returns></returns>
    public bool GetAuditPassStatus()
    {
        if (AppMsg.ContainsKey(Config.appVersion))
        {
            bool isPassed = AppMsg[Config.appVersion].IsPassed;
            DebugExt.Log("appVersion:" + Config.appVersion + ", isPassed:" + isPassed);
            return isPassed;
        }
        else
        {
            DebugExt.LogW($"GetPassStatus error, no appVersion:{Config.appVersion} in serverlist, so we set true default");
            return true;
        }
    }

}