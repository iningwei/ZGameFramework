using MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZGame;
using ZGame.Event;
using ZGame.Obfuscation;
using ZGame.TimerTween;
using ZGame.Window;


//需支持断点下载
//服务器资源可以有多个版本，每个版本都有个filelist，每个flielist都有完整的所有资源清单（有序的文件名，md5，包括配置表（如果需要更新配置表的话））

public class ResourceUpdate : Singleton<ResourceUpdate>
{

    int downloadSize = 0;
    int curDownLoadIndex = -1;

    //当前要下载的某个版本资源列表
    List<DownLoadZipResFileInfo> resServerFileInfos = new List<DownLoadZipResFileInfo>();
    //当前要下载的某个版本资源的总大小, 单位byte
    public long curHandleResVersionResTotalSize = 0;
    //当前要下载的某个版本资源  已经下载的文件大小
    public long curHandleResVersionResDownloadedSize = 0;

    //服务端资源版本号
    public int serverResVer;
    //当前正在操作的资源版本
    public int curHandleResVersion;
    //开始下载，首次操作的资源版本
    public int startHandleResVersion = -1;
    /// <summary>
    /// 
    /// </summary>
    public void Enter()
    {
        curDownLoadIndex = -1;


        //检测资源是否需要更新
        var localResVer = int.Parse(GetLocalResVersion());
        serverResVer = int.Parse(ServerList.Instance.GetAppMaxResVersion(Config.appVersion));


        ReadABMsgToDic();



        if (serverResVer > localResVer)
        {
            DebugExt.Log("res need update,sverResVer:" + serverResVer + ", localResVer：" + localResVer);

            curHandleResVersion = localResVer + 1;
            if (startHandleResVersion == -1)
            {
                startHandleResVersion = curHandleResVersion;
            }
            DownloadFileList(curHandleResVersion);
        }
        else if (serverResVer == localResVer)
        {
            DebugExt.Log($"res already update to newest res version:{serverResVer},enterGame！");

            EnterGame();
        }
        else
        {
            //sverver<localver也直接进入游戏
            //造成这种情况的原因，比如某用户通过商店下载个资源版本为10的版本，但是ftp那边配置的最大版本是9，并没有及时切换到10
            Debug.LogWarning("Warning：sverver<localver, sverVer:" + serverResVer + ", localVer:" + localResVer);
            EnterGame();
        }
    }


    void EnterGame()
    {
        if (CheckUpdateDirABCompletion())
        {
            //资源更新完毕，开始进入游戏
            TimerTween.Delay(0.5f, () =>
            {
                if (WindowManager.Instance.IsWindowOpen("HotUpdateWindow"))
                {
                    WindowManager.Instance.CloseWindow("HotUpdateWindow");
                }
                ScriptManager.Instance.Init();
            }).Start();


        }
        else
        {
            WindowManager.Instance.SendWindowMessage("HotUpdateWindow", 100, null);
        }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetLocalResVersion()
    {
        //TODO:资源更新的版本号写到PlayerPrefs中，如果用户覆盖安装了一个低版本的呢，会造成什么影响？？


        //先判断playerprefs是否有版本号，如果没有则从包配置里读取
        var localResVersion = PlayerPrefs.GetString("resversion_" + Config.appVersion, "-1");
        if (localResVersion == "-1")
        {
            localResVersion = Config.resVersion;
        }

        return localResVersion.ToString();
    }

    public void SetLocalResVersion(string version)
    {
        PlayerPrefs.SetString("resversion_" + Config.appVersion, version);
        PlayerPrefs.Save();
    }

    public int GetLocalResMaxZippedIndex(int resVersion)
    {
        return PlayerPrefs.GetInt("maxzippedindex_" + Config.appVersion + "_" + resVersion, -1);
    }

    public void SetLocalResMaxZippedIndex(int resVersion, int index)
    {
        PlayerPrefs.SetInt("maxzippedindex_" + Config.appVersion + "_" + resVersion, index);
        PlayerPrefs.Save();
    }


    void DownloadFileList(int resVersion)
    {
        DebugExt.Log("begin download versionlist, resVersion:" + resVersion);
        //获得 versionlist.json中的内容
        var vlUrl = Config.GetPackData(Config.packType).ftpTxtFileUrl + "/" + IOTools.PlatformFolderName + "/channel_" + Config.gameChannelId + "/" + Config.appVersion + "/" + resVersion + "/versionlist.json";

        HttpTool.Get(vlUrl, IdAssginer.GetID(IdAssginer.IdType.WWW), VersionListDownloadCmp, VersionListDownloadFail);
    }

    private void VersionListDownloadCmp(long id, byte[] datas)
    {
        int zipFileCount = 0;
        int localZippedIndex = GetLocalResMaxZippedIndex(curHandleResVersion);


        resServerFileInfos.Clear();
        var versionList = System.Text.Encoding.Default.GetString(datas);
        DebugExt.Log("versionlist download finish:" + versionList);
        var flData = Json.Deserialize(versionList) as Dictionary<string, object>;
        zipFileCount = int.Parse(flData["zipfilecount"].ToString());

        DebugExt.Log("local zippedindex:" + localZippedIndex + ",zipFileCount:" + zipFileCount);


        //----------->

        //////resServerFileInfos.Clear();
        //////var resfileList = System.Text.Encoding.Default.GetString(datas);
        //////DebugExt.Log("resfilelist download finished:" + resfileList); 
        curHandleResVersionResTotalSize = 0;//单位为Byte
        curHandleResVersionResDownloadedSize = 0;

        var list = flData["ziplist"] as List<object>;
        for (int i = 0; i < list.Count; i++)
        {
            var itemDic = list[i] as Dictionary<string, object>;
            var name = itemDic["name"].ToString();
            var index = int.Parse(name.Replace(".zip", "").Split('_')[3]);
            if (index <= localZippedIndex || index > zipFileCount)
            {
                continue;
            }



            var info = new DownLoadZipResFileInfo();
            info.name = name;
            info.index = int.Parse(name.Replace(".zip", "").Split('_')[3]);
            info.md5 = itemDic["md5"].ToString().ToLower();
            info.size = System.Convert.ToInt32(itemDic["size"].ToString());
            curHandleResVersionResTotalSize += info.size;
            info.needDownload = true;
            resServerFileInfos.Add(info);
        }

        //根据索引排序
        resServerFileInfos.Sort((t1, t2) => { return t1.index.CompareTo(t2.index); });

#if UNITY_EDITOR
        DebugExt.Log("need download res count:" + resServerFileInfos.Count + ",list：");
        for (int i = 0; i < resServerFileInfos.Count; i++)
        {
            DebugExt.Log(resServerFileInfos[i].name);
        }
#endif
        EventDispatcher.Instance.DispatchEvent(EventID.OnBeginDownloadHotResFiles, curHandleResVersionResTotalSize.ToString());

        DownLoadNext();
    }




    /// <summary>
    /// 检测更新目录下AB资源的完整性
    /// </summary>
    /// <returns></returns>
    bool CheckUpdateDirABCompletion()
    {
        DebugExt.Log("--------> begin check AB Completion");
        //第一步，根据allABMsgDic的数据来逐一比对AB文件数据。
        //第二步，再检测所有的AB文件是否存在于allABMsgDic中
        //两遍检测保证一一对应
        string[] allABFiles = IOTools.getAllABFilesInUpdateDir();
        DebugExt.Log("--------> total ab res count in hotupdate folder：" + allABFiles.Length);

        //第一步检测
        DebugExt.Log("--------> first step check");
        foreach (KeyValuePair<string, string> item in allABMsgDic)
        {
            var fileData = IOTools.GetResFileDataFromUpdateDir(item.Key);
            if (fileData == null)
            {
                DebugExt.LogE("--------> res not complete,no：" + item.Key);
                return false;
            }
            if (MD5.Get(fileData) != item.Value)
            {
                DebugExt.LogE("--------> res md5 not the same：" + item.Key);
                return false;
            }
        }

        //第二步检测
        DebugExt.Log("--------> second step check");
        for (int j = 0; j < allABFiles.Length; j++)
        {
            string name = Path.GetFileName(allABFiles[j]);
            if (allABMsgDic.ContainsKey(name) == false)
            {
                DebugExt.LogE("--------> abmsg.txt not contain：" + name);
                return false;
            }
        }


        DebugExt.Log("--------> res is complete！！");
        return true;
    }

    void ReadABMsgToDic()
    {
        string[] lines = IOTools.readAllLinesFromUpdateDir("abmsg.txt");
        if (lines != null)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                var datas = lines[i].Split('|');
                allABMsgDic[datas[0]] = datas[1];
            }
        }
    }


    public void UpdateABMsgTxtFile()
    {
        string allStrs = "";
        var keys = new List<string>(allABMsgDic.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            allStrs += (keys[i] + "|" + allABMsgDic[keys[i]]);
            if (i < keys.Count - 1)
            {
                allStrs += "\r\n";
            }
        }

        IOTools.WriteStringToUpdateDir("abmsg.txt", allStrs);
    }

    /// <summary>
    /// 
    /// </summary>
    void DownLoadNext()
    {
        curDownLoadIndex++;
        if (curDownLoadIndex >= resServerFileInfos.Count)
        {
            //写入最新资源版本号            
            SetLocalResVersion(curHandleResVersion.ToString());
            SetLocalResMaxZippedIndex(curHandleResVersion, curDownLoadIndex);
            //更新ABMsgDic中数据到本地
            UpdateABMsgTxtFile();

            DebugExt.Log("zip hotupdate res downloaded!!!");
            Enter();
        }
        else
        {
            var info = resServerFileInfos[curDownLoadIndex];
            if (!info.needDownload)
            {
                DownLoadNext();
            }
            else
            {
                var url = Config.GetPackData(Config.packType).ftpZipFileUrl + "/" + IOTools.PlatformFolderName + "/channel_" + Config.gameChannelId + "/" + Config.appVersion + "/" + curHandleResVersion + "/res/" + info.name;

                DebugExt.Log("begindown：" + info.name);

                HttpTool.Get(url, IdAssginer.GetID(IdAssginer.IdType.WWW), fileDownloadCmp, fileDownloadFail, fileDownloadUpdate);
            }
        }
    }


    ///////// <summary>
    ///////// 当下载失败后，继续下载时调用
    ///////// </summary>
    //////public void ResumeDownload()
    //////{
    //////    DebugExt.Log("ResumeDownload");
    //////    curDownLoadIndex--;
    //////    DownLoadNext();
    //////}



    /// <summary>
    /// 
    /// </summary>
    /// <param name="downloaded">某文件已下载大小</param>
    /// <param name="downloadLength">某文件总大小</param>
    private void fileDownloadUpdate(long downloaded, long downloadLength)
    {
        float p = ((float)downloaded) / downloadLength;
        var info = resServerFileInfos[curDownLoadIndex];
        //////DebugExt.Log("file:" + info.name + ",progress: " + p + ", downloaded:" + downloaded + ", total:" + downloadLength);

        //在当前资源版本中已下载的比例
        float resVersionRatio = (curHandleResVersionResDownloadedSize + downloaded) / ((float)curHandleResVersionResTotalSize);
        EventDispatcher.Instance.DispatchEvent(EventID.OnHotResFileDownloading, resVersionRatio);
    }


    /// <summary>
    /// 记录ab信息
    /// </summary>
    Dictionary<string, string> allABMsgDic = new Dictionary<string, string>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="www"></param>
    void fileDownloadCmp(long id, byte[] datas)
    {
        var info = resServerFileInfos[curDownLoadIndex];
        DebugExt.Log("res file download finish:" + info.name);
        curHandleResVersionResDownloadedSize += info.size;
        if (info.md5 != MD5.Get(datas))
        {
            DebugExt.LogE("error, md5 not the same：" + info.name);
            return;
        }


        //解压
        ResFileCombo cb = new ResFileCombo(datas);
        var files = cb.ReadAllFile();
        foreach (var v in files)
        {
            DebugExt.Log("unzip file：" + v.name);
            allABMsgDic[v.name] = MD5.Get(v.data);
            IOTools.WriteFileToUpdateDir(v.name, v.data);
        }


        EventDispatcher.Instance.DispatchEvent(EventID.OnHotResFileDownloaded, info.size.ToString());
        //下载下一个文件
        DownLoadNext();
    }

    public void AddKVToABMsgDic(string k, string v)
    {
        allABMsgDic[k] = v;
    }

    /// <summary>
    /// 
    /// </summary>
    void fileDownloadTimeOut()
    {
        var info = resServerFileInfos[curDownLoadIndex];
        Debug.Log("res file download timeout :" + info.name);
    }

    /// <summary>
    /// 
    /// </summary>
    void fileDownloadFail(long id, string errorMsg)
    {
        var info = resServerFileInfos[curDownLoadIndex];
        DebugExt.LogE("res file download fail:" + info.name);
        //EventDispatcher.Instance.DispatchEvent(EventID.OnHotResFileDownloadFail, null);

        WindowManager.Instance.SendWindowMessage("HotUpdateWindow", 50);
    }

    /// <summary>
    /// 
    /// </summary>
    void FileListDownloadTimeOut()
    {
        DebugExt.Log("fileList download timeout");

    }

    /// <summary>
    /// 
    /// </summary>
    void VersionListDownloadFail(long id, string errorMsg)
    {
        DebugExt.LogE("fileList download fail,errorMsg:" + errorMsg);
        WindowManager.Instance.SendWindowMessage("HotUpdateWindow", 50);
    }
}
