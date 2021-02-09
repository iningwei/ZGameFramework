using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniJSON;

namespace ZGame.HotUpdate
{
    public class FTPConfig
    {
        //自己搭建的FTP服务器
        public string innerUrl;
        //开发用的FTP服务器
        public string devUrl;
        //发布用的FTP服务器
        public string pubUrl;
    }
    public class PostCMDConfig
    {
        /// <summary>
        /// 代哥开发服
        /// </summary>
        public string daiDevUrl;
        /// <summary>
        /// 外网测试地址
        /// </summary>
        public string outerTestUrl;
        /// <summary>
        /// 线上地址
        /// </summary>
        public string onLineUrl;
    }

    public class SubGameConfig
    {
        public string name;
        public string abName;
        public long size;
        public string md5;
        public bool install;
    }

    public class Config
    {
        public static string appVersion;
        public static string appBundleVersion;

        public static string resBigVersion;
        public static string resSmallVersion;
        public static int channelId;

        public static FTPConfig ftpConfig;
        public static PostCMDConfig postCMDConfig;
        static List<SubGameConfig> subGameConfigs;

        static Config()
        {
            string configStr = "";
#if UNITY_ANDROID
            configStr = Resources.Load<TextAsset>("Config/android_cfg").text;
#elif UNITY_IOS
        configStr = Resources.Load<TextAsset>("Config/ios_cfg").text; 
#elif UNITY_STANDALONE_WIN
        configStr = Resources.Load<TextAsset>("Config/pc_win_cfg").text;
#elif UNITY_STANDALONE_OSX
        configStr= Resources.Load<TextAsset>("Config/pc_mac_cfg").text; 
#else
        configStr= Resources.Load<TextAsset>("Config/other_cfg").text; 
#endif
            if (configStr == "")
            {
                Debug.LogError("error, configStr is empty!");
            }

            var dic = Json.Deserialize(configStr) as Dictionary<string, object>;
            appVersion = (string)dic["appversion"];
            appBundleVersion = (string)dic["appbundleversion"];
            resBigVersion = (string)dic["resbigversion"];
            resSmallVersion = (string)dic["ressmallversion"];
            channelId = (int)(long)dic["channelid"];

            ftpConfig = new FTPConfig();
            var ftpDic = dic["ftpurllist"] as Dictionary<string, object>;
            ftpConfig.innerUrl = (string)ftpDic["ftpinnerurl"];
            ftpConfig.devUrl = (string)ftpDic["ftpdevurl"];
            ftpConfig.pubUrl = (string)ftpDic["ftppuburl"];

            postCMDConfig = new PostCMDConfig();
            var postCmdDic = dic["postcmdurllist"] as Dictionary<string, object>;
            postCMDConfig.daiDevUrl = (string)postCmdDic["daidevurl"];
            postCMDConfig.outerTestUrl = (string)postCmdDic["outertesturl"];
            postCMDConfig.onLineUrl = (string)postCmdDic["onlineurl"];

            var subgameList = (List<object>)dic["subgamelist"];
            subGameConfigs = new List<SubGameConfig>();
            for (int i = 0; i < subgameList.Count; i++)
            {
                var subgameDic = subgameList[i] as Dictionary<string, object>;
                SubGameConfig sgc = new SubGameConfig();
                sgc.name = (string)subgameDic["name"];
                sgc.abName = (string)subgameDic["abname"];
                sgc.size = (long)subgameDic["size"];
                sgc.md5 = (string)subgameDic["md5"];
                sgc.install = (bool)subgameDic["install"];
                subGameConfigs.Add(sgc);
            }
        }
    }
}