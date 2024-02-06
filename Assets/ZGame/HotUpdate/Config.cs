using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniJSON;
using System.IO;
using ZGame.Ress;
using System;

namespace ZGame
{
    public class SubGameConfig
    {
        public string name;
        public string abName;
        public long size;
        public string md5;
        public bool install;
    }


    public class LoginData
    {
        public string loginType;
        public string url;
    }

    //打包(热更)相关信息
    public class PackData
    {
        public string packType;
        public string ftpTxtFileUrl;
        public string ftpZipFileUrl;
    }

    public enum ScreenOrientation
    {
        Landscape = 0,
        Portrait = 1,
    }


    public class Config
    {
        public static string productName;

        /// <summary>
        /// 和Project Settings中的Version一致
        /// </summary>
        public static string appVersion;
        /// <summary>
        /// 和Project Setting中的Bundle Version Code / Version对应
        /// GP和AppleStore新版本提交该值都需要增加
        /// </summary>
        public static string appBundleVersion;

        public static string resVersion;
        public static string packTimeStamp;

        /// <summary>
        /// 游戏上线渠道
        /// 100为安卓中的 GP
        /// 200为ios中的 appstore
        /// </summary>
        public static int gameChannelId;

        /// <summary>
        /// 游戏支付渠道
        /// 1为applestore
        /// 2为googleplay
        /// </summary>
        public static int paymentChannelId;

        /// <summary>
        /// AB资源名是否加密了
        /// </summary>
        public static bool isABResNameCrypto;
        /// <summary>
        /// AB资源名加密密钥
        /// </summary>
        public static string abResNameCryptoKey;
        /// <summary>
        /// AB资源二进制偏移位数（二进制前增加的位数）
        /// </summary>
        public static int abResByteOffset;

        /// <summary>
        /// 游戏设计分辨率
        /// </summary>
        public static Vector2 gameDesignRatio;

        /// <summary>
        /// Landscape:0; Portrait:1
        /// </summary>
        public static int screenOrientation;


        /// <summary>
        /// 游戏内操作类型
        /// 1为鼠标键盘、2为触屏        
        /// </summary>
        public static int gameInputType;


        /// <summary>
        /// 见ResLoadType枚举
        /// </summary>
        public static int resLoadType;

        public static string firstOpenWindowName;

        public static List<ProtobufMsgID> socketIgnoreLogMsgIds = new List<ProtobufMsgID>();

        public static List<LoginData> loginDataList = new List<LoginData>();


        public static List<PackData> packDataList = new List<PackData>();

        /// <summary>
        /// 登录方式：PUB;DEV
        /// </summary>
        public static string loginType;

        /// <summary>
        /// 热更资源CDN地址类型：PUB;DEV;TEST
        /// </summary>
        public static string packType;
        //--------------->下面这几个参数，游戏内提供了机制进行修改，方便调试

        /// <summary>
        /// 是否走真实购买流程
        /// </summary>
        public static bool isRealPurchase;
        /// <summary>
        /// 是否显示消息协议日志
        /// </summary>
        public static bool isShowProtoMsgLog;
        /// <summary>
        /// 是否显示Debug上传按钮
        /// </summary>
        public static bool isShowDebugBtn;
        /// <summary>
        /// 是否开启日志追踪，开启后，点击debug上传按钮，才会有上报的日志   ,（目前开启后，在ios上会遇到崩溃     ）
        /// </summary>
        public static bool isEnableLogTrace;
        /// <summary>
        /// IsEnableLogTrace为true的情况下，开启实时写入的话，会把日志实时写入本地
        /// </summary>
        public static bool isEnableLogRealtimeWriteToLocal;
        /// <summary>
        /// IsEnableLogUpdate2Server为true时，才允许客户端上报日志到服务端
        /// </summary>
        public static bool isEnableLogUpdate2Server;
        /// <summary>
        /// 是否显示Reporter信息提示插件
        /// </summary>
        public static bool isShowReporter;


        public static string cfgFileName;
        //内置Resources目录下cfg文件的全路径
        public static string resConfigFilePath;
        //动态生成的cfg文件全路径
        static string dynamicConfigFilePath;
        static Config()
        {
            readCfgData();
        }

        static void readCfgData()
        {
            string configStr = "";

#if UNITY_ANDROID
            cfgFileName = "android_cfg";
#elif UNITY_IOS
            cfgFileName="ios_cfg";
#elif UNITY_STANDALONE_WIN
            cfgFileName = "pc_win_cfg";
#elif UNITY_STANDALONE_OSX
            cfgFileName="pc_mac_cfg";
#else
            cfgFileName="other_cfg";
#endif
            resConfigFilePath = Application.dataPath + "/ZGame/HotUpdate/Resources/Config/" + cfgFileName + ".bytes";

            dynamicConfigFilePath = Application.persistentDataPath + "/dynamic_cfg_v" + Application.version + "/" + cfgFileName + ".txt";
            if (File.Exists(dynamicConfigFilePath))
            {
                Debug.Log("get config from dynamic path");
                configStr = File.ReadAllText(dynamicConfigFilePath);
            }
            else
            {
                Debug.Log("get config from Resources path");
                configStr = Resources.Load<TextAsset>("Config/" + cfgFileName).text;
            }

            if (string.IsNullOrEmpty(configStr))
            {
                Debug.LogError("error, configStr is empty!");
            }

            var dic = Json.Deserialize(configStr) as Dictionary<string, object>;
            productName = (string)dic["ProductName"];
            appVersion = (string)dic["AppVersion"];
            appBundleVersion = (string)dic["AppBundleVersion"];
            resVersion = (string)dic["ResVersion"];
            packTimeStamp = (string)dic["PackTimeStamp"];
            gameChannelId = (int)(long)dic["GameChannelId"];
            paymentChannelId = (int)(long)dic["PaymentChannelId"];

            isABResNameCrypto = bool.Parse(dic["IsABResNameCrypto"].ToString());
            abResNameCryptoKey = (string)dic["ABResNameCryptoKey"];
            abResByteOffset = (int)(long)dic["ABResByteOffset"];

            var ratioStrs = ((string)dic["GameDesignRatio"]).Split(',');
            gameDesignRatio = new Vector2(int.Parse(ratioStrs[0]), int.Parse(ratioStrs[1]));
            screenOrientation = (int)(long)dic["ScreenOrientation"];

            gameInputType = (int)(long)dic["GameInputType"];
            resLoadType = (int)(long)dic["ResLoadType"];
            firstOpenWindowName = (string)dic["FirstOpenWindowName"];

            socketIgnoreLogMsgIds.Clear();
            var ignoreIds = dic["SocketIgnoreLogMsgIds"] as List<object>;
            for (int i = 0; i < ignoreIds.Count; i++)
            {
                var msgId = (ProtobufMsgID)Enum.Parse(typeof(ProtobufMsgID), (string)ignoreIds[i]);
                socketIgnoreLogMsgIds.Add(msgId);
            }

            loginDataList.Clear();
            var loginDataArray = dic["LoginData"] as List<object>;
            for (int i = 0; i < loginDataArray.Count; i++)
            {
                var loginDataDic = loginDataArray[i] as Dictionary<string, object>;
                LoginData data = new LoginData();
                data.loginType = (string)loginDataDic["LoginType"];
                data.url = (string)loginDataDic["URL"];

                loginDataList.Add(data);
            }


            packDataList.Clear();
            var packDataArray = dic["PackData"] as List<object>;
            for (int i = 0; i < packDataArray.Count; i++)
            {
                var packDataDic = packDataArray[i] as Dictionary<string, object>;
                PackData data = new PackData();
                data.packType = (string)packDataDic["PackType"];
                data.ftpTxtFileUrl = (string)packDataDic["FtpTxtFileURL"];
                data.ftpZipFileUrl = (string)packDataDic["FtpZipFileURL"];

                packDataList.Add(data);
            }

            loginType = (string)dic["LoginType"];
            packType = (string)dic["PackType"];
            //------------------------> 
            isRealPurchase = bool.Parse(dic["IsRealPurchase"].ToString());
            isShowProtoMsgLog = bool.Parse(dic["IsShowProtoMsgLog"].ToString());
            isShowDebugBtn = bool.Parse(dic["IsShowDebugBtn"].ToString());
            isEnableLogTrace = bool.Parse(dic["IsEnableLogTrace"].ToString());
            isEnableLogRealtimeWriteToLocal = bool.Parse(dic["IsEnableLogRealtimeWriteToLocal"].ToString());
            isEnableLogUpdate2Server = bool.Parse(dic["IsEnableLogUpdate2Server"].ToString());
            isShowReporter = bool.Parse(dic["IsShowReporter"].ToString());
        }

        public static void RefreshData()
        {
            readCfgData();
            Debug.Log($"refresh cfg data, appVersion:{appVersion}, resVersion:{resVersion}, channelId:{gameChannelId}");
        }
        public static string AssignConfigDataToJson(string productName, string appVersion, string appBundleVersion, string resVersion, string packTimeStamp, int gameChannelId,
            int paymentChannelId, bool isABResNameCrypto, string abResNameCryptoKey, int abResByteOffset, int screenOrientation, int gameInputType, int resLoadType, string firstOpenWindowName, Vector2 gameDesignRatio, List<LoginData> loginDataList, List<PackData> packDataList, string loginType,
            string packType, bool isRealPurchase, bool isShowProtoMsgLog, bool isShowDebugBtn,
            bool isEnableLogTrace, bool isEnableLogRealtimeWriteToLocal, bool isEnableLogUpdate2Server, bool isShowReporter)
        {

            Dictionary<string, object> configDic = new Dictionary<string, object>();
            configDic["ProductName"] = productName;
            configDic["AppVersion"] = appVersion;
            configDic["AppBundleVersion"] = appBundleVersion;
            configDic["ResVersion"] = resVersion;
            configDic["PackTimeStamp"] = packTimeStamp;
            configDic["GameChannelId"] = gameChannelId;
            configDic["PaymentChannelId"] = paymentChannelId;
            configDic["IsABResNameCrypto"] = isABResNameCrypto;
            configDic["ABResNameCryptoKey"] = abResNameCryptoKey;
            configDic["ABResByteOffset"] = abResByteOffset;
            configDic["GameDesignRatio"] = gameDesignRatio.x + "," + gameDesignRatio.y;
            configDic["ScreenOrientation"] = screenOrientation;
            configDic["GameInputType"] = gameInputType;
            configDic["ResLoadType"] = resLoadType;
            configDic["FirstOpenWindowName"] = firstOpenWindowName;
            configDic["SocketIgnoreLogMsgIds"] = Config.socketIgnoreLogMsgIds;


            //组装LoginData
            List<object> loginList = new List<object>();
            if (loginDataList != null)
            {
                int count = loginDataList.Count;
                for (int i = 0; i < count; i++)
                {
                    var tmpData = loginDataList[i];
                    Dictionary<string, object> loginDataDic = new Dictionary<string, object>();
                    loginDataDic["LoginType"] = tmpData.loginType;
                    loginDataDic["URL"] = tmpData.url;
                    loginList.Add(loginDataDic);
                }
            }
            configDic["LoginData"] = loginList;

            //组装PackData
            List<object> packList = new List<object>();
            if (packDataList != null)
            {
                int count = packDataList.Count;
                for (int i = 0; i < count; i++)
                {
                    var tmpData = packDataList[i];
                    Dictionary<string, object> packDataDic = new Dictionary<string, object>();
                    packDataDic["PackType"] = tmpData.packType;
                    packDataDic["FtpTxtFileURL"] = tmpData.ftpTxtFileUrl;
                    packDataDic["FtpZipFileURL"] = tmpData.ftpZipFileUrl;
                    packList.Add(packDataDic);
                }
            }
            configDic["PackData"] = packList;

            configDic["LoginType"] = loginType;
            configDic["PackType"] = packType;
            //其它  
            configDic["IsRealPurchase"] = isRealPurchase;
            configDic["IsShowProtoMsgLog"] = isShowProtoMsgLog;
            configDic["IsShowDebugBtn"] = isShowDebugBtn;
            configDic["IsEnableLogTrace"] = isEnableLogTrace;
            configDic["IsEnableLogRealtimeWriteToLocal"] = isEnableLogRealtimeWriteToLocal;
            configDic["IsEnableLogUpdate2Server"] = isEnableLogUpdate2Server;
            configDic["IsShowReporter"] = isShowReporter;


            string jsonStr = Json.Serialize(configDic);
            return jsonStr;
        }

        public static void WriteToResConfig(string jsonStr)
        {
            IOTools.WriteString(resConfigFilePath, jsonStr);
        }

        public static void WriteToDynamicConfig(string jsonStr)
        {
            IOTools.WriteString(dynamicConfigFilePath, jsonStr);
        }

        public static void SaveDynamicConfig(string loginType, string packType
            , string isRealPurchase
            , string isShowProtoMsgLog
            , string isShowDebugBtn
            , string isEnableLogTrace
            , string isEnableLogRealtimeWriteToLocal
            , string isEnableLogUpdate2Server
            , string isShowReporter)
        {
            var jsonStr = AssignConfigDataToJson(productName, appVersion, appBundleVersion, resVersion, packTimeStamp, gameChannelId, paymentChannelId, isABResNameCrypto, abResNameCryptoKey, abResByteOffset, screenOrientation, gameInputType, resLoadType, firstOpenWindowName, gameDesignRatio, loginDataList,
                  packDataList,
               loginType, packType, bool.Parse(isRealPurchase), bool.Parse(isShowProtoMsgLog), bool.Parse(isShowDebugBtn), bool.Parse(isEnableLogTrace), bool.Parse(isEnableLogRealtimeWriteToLocal), bool.Parse(isEnableLogUpdate2Server), bool.Parse(isShowReporter)
                  );
            WriteToDynamicConfig(jsonStr);
        }


        public static PackData GetPackData(string packType)
        {
            for (int i = 0; i < packDataList.Count; i++)
            {
                if (packDataList[i].packType == packType)
                {
                    return packDataList[i];
                }
            }
            Debug.LogError("no PackData with PackType:" + packType);
            return null;
        }
    }
}