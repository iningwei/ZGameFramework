using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ZGame;
using ZGame.Window;

public class ServerListDownload : Singleton<ServerListDownload>
{

    Action checkConnectSuccessCB;
    public void Download()
    {
        DebugExt.Log("begin download serverlist");
        //下载serverlist
        //url后加个时间戳，防止cdn缓存
        var serverlistUrl = Config.GetPackData(Config.packType).ftpTxtFileUrl + "/" + IOTools.PlatformFolderName + "/channel_" + Config.gameChannelId + "/serverlist" + ".json?ts=" + TimeTool.GetNowStamp();

        HttpTool.Get(serverlistUrl, IdAssginer.GetID(IdAssginer.IdType.WWW), onCmp, onFail, fileDownloadUpdate);
    }


    private void fileDownloadUpdate(long downloaded, long downloadLength)
    {
        DebugExt.Log("serverList download msg:" + downloaded + ", downloadLength:" + downloadLength);
    }


    private void onFail(long id, string errorMsg)
    {
        DebugExt.LogE("serverlist download failed, id:" + id + "====>errorMsg:" + errorMsg);

        WindowManager.Instance.SendWindowMessage("HotUpdateWindow", 50, null);
    }

    private void onCmp(long id, byte[] datas)
    {
        string str = System.Text.Encoding.Default.GetString(datas);
        DebugExt.Log("serverlist download success,id:" + id + ", content:" + str);

        ServerList.Instance.Init(str);
        DebugExt.Log("channelId:" + Config.gameChannelId);

        //检测是否需要更新app
        AppUpdate.Instance.Enter();
    }

    private void onTimeout()
    {
        DebugExt.Log("serverlist download timeout");
    }

    private void onUpdate(float p)
    {
        DebugExt.Log("serverlist download progress:" + p);
    }
}
