using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HybridCLRServerListDownload : HybridCLRSingleton<HybridCLRServerListDownload>
{

    GameEntry gameEntry;
    public void Download(GameEntry gameEntry)
    {
        this.gameEntry = gameEntry;
        Debug.Log("begin download serverlist");
        //下载serverlist
        //url后加个时间戳，防止cdn缓存
        //var serverlistUrl = HybridCLRConfig.GetPackData(HybridCLRConfig.packType).ftpTxtFileUrl + "/" + HybridCLRIOTools.PlatformFolderName + "/channel_" + HybridCLRConfig.gameChannelId + "/serverlist" + ".json?ts=" + TimeTool.GetNowStamp();

        var serverlistUrl = HybridCLRConfig.GetPackData(HybridCLRConfig.packType).ftpTxtFileUrl + "/" + HybridCLRIOTools.PlatformFolderName + "/channel_" + HybridCLRConfig.gameChannelId + "/serverlist" + ".json";

        HybridCLRHttpTool.Get(serverlistUrl, IdAssginer.GetID(IdAssginer.IdType.WWW), onCmp, onFail, fileDownloadUpdate);
    }


    private void fileDownloadUpdate(long downloaded, long downloadLength)
    {
        Debug.Log("serverList download msg:" + downloaded + ", downloadLength:" + downloadLength);
    }


    private void onFail(long id, string errorMsg)
    {
        Debug.LogError("serverlist download failed, id:" + id + "====>errorMsg:" + errorMsg);

        //网络错误
        this.gameEntry.ShowErrorTip("网络错误:1");
    }

    private void onCmp(long id, byte[] datas)
    {
        string str = System.Text.Encoding.Default.GetString(datas);
        Debug.Log("serverlist download success,id:" + id + ", content:" + str);

        HybridCLRServerList.Instance.Init(str);
        Debug.Log("channelId:" + HybridCLRConfig.gameChannelId);

        //检测是否需要更新app
        HybridCLRAppUpdate.Instance.Enter(gameEntry);
    }

    private void onTimeout()
    {
        Debug.Log("serverlist download timeout");
    }

    private void onUpdate(float p)
    {
        Debug.Log("serverlist download progress:" + p);
    }
}
