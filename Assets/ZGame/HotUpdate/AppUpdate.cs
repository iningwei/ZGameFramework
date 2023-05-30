using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame;
using ZGame.Window;

public class AppUpdate : Singleton<AppUpdate>
{
    //AppVersion为3位，如1.0.0，1.1.2 
    //ResVersion为一位，如1，2，3    
    public void Enter()
    {
        var localVer = Config.appVersion;
        var serverCurMaxVer = ServerList.Instance.CurMaxAppVersion;
        var serverCanRunMinVer = ServerList.Instance.CanRunMinAppVersion;
        if (compareVer(serverCurMaxVer, serverCanRunMinVer) == -1)
        {
            DebugExt.LogE("error, serverCurMaxVer<serverCanRunMinVer");
            return;
        }

        int result1 = compareVer(serverCurMaxVer, localVer);
        int result2 = compareVer(serverCanRunMinVer, localVer);

        if (result1 == -1)
        {
            DebugExt.LogE($"error, serverCurMaxVer<localVer, serverCurMaxVer:{serverCurMaxVer}, localVer:{localVer}");
            return;
        }
        else
        {
            if (result2 == 1)
            {
                DebugExt.Log("app need big version update, open reinstall ui....");
                WindowManager.Instance.SendWindowMessage("HotUpdateWindow", 10, null);
            }
            else
            {
                DebugExt.Log("app do not need big version update，prepare check res update....");
                //检测通过，进入资源更新流程
                ResourceUpdate.Instance.Enter();
                WindowManager.Instance.SendWindowMessage("HotUpdateWindow", 11, null);
            }
        }

    }


    /// <summary>
    /// 比较 ver1 和 ver2 的大小关系
    /// 返回1，说明 ver1>ver2
    /// 返回0，说明 ver1=ver2
    /// 返回-1，说明 ver1<ver2
    /// </summary>
    /// <param name="ver1"></param>
    /// <param name="ver2"></param>
    /// <returns></returns>
    int compareVer(string ver1, string ver2)
    {
        string[] ver1Strs = ver2.Split('.');
        string[] ver2Strs = ver1.Split('.');
        int[] ver1Numbers = new int[] { int.Parse(ver1Strs[0]), int.Parse(ver1Strs[1]), int.Parse(ver1Strs[2]) };
        int[] ver2Numbers = new int[] { int.Parse(ver2Strs[0]), int.Parse(ver2Strs[1]), int.Parse(ver2Strs[2]) };
        if (ver2Numbers[0] > ver1Numbers[0])
        {
            return 1;
        }
        else if (ver2Numbers[0] == ver1Numbers[0])
        {

            //比较第2位
            if (ver2Numbers[1] > ver1Numbers[1])
            {
                return 1;
            }
            else if (ver2Numbers[1] == ver1Numbers[1])
            {

                if (ver2Numbers[2] > ver1Numbers[2])
                {
                    return 1;
                }
                else if (ver2Numbers[2] == ver1Numbers[2])
                {

                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        else
        {
            return -1;
        }


    }
}
