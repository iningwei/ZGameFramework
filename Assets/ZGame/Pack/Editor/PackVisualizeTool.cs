using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame;
using ScreenOrientation = ZGame.ScreenOrientation;

public class PackVisualizeTool : EditorWindow
{
    public static string productName;
    public static string appVersion;
    public static string appBundleVersion;
    public static string resVersion;
    public static string packTimeStamp;

    public static int gameChannelId;
    public static int paymentChannelId;
    public static bool isABResNameCrypto;
    public static string abResNameCryptoKey;
    public static int abResByteOffset;
    public static ScreenOrientation screenOrientation;
    public static int gameInputType;
    public static int resLoadType;
    public static string firstOpenWindowName;

    public static Vector2 gameDesignRatio;

    public static List<ServerData> serverDataList = new List<ServerData>();
    public static List<PackData> packDataList = new List<PackData>();


    public static string packType;
    public static bool isRealPurchase;
    public static bool isShowProtoMsgLog;
    public static bool isShowDebugBtn;
    public static bool isEnableLogTrace;
    public static bool isEnableLogRealtimeWriteToLocal;
    public static bool isEnableLogUpdate2Server;
    public static bool isShowReporter;
    public static bool isDIYScrip;
    public bool[] DIYScrips;
    public string[] ScripsNames = { "XLua", "OriginLuaFile", "HOTUPDATE", "UNITY_POST_PROCESSING_STACK_V2", "MOBILE_INPUT", "BAKERY_INCLUDED" };



    Vector2 scrollPos;
    bool configFoldOut = true;
    bool packFoldOut = true;
    bool ScriptOut = true;

    [MenuItem("工具/打包/打包预览")]
    static void PackVisual()
    {
        PackVisualizeTool visualizeTool = EditorWindow.GetWindow(typeof(PackVisualizeTool)) as PackVisualizeTool;
        //visualizeTool.Show();
        init();
    }

    private static void init()
    {
        productName = Config.productName;

        appVersion = Config.appVersion;
        appBundleVersion = Config.appBundleVersion;
        resVersion = Config.resVersion;
        packTimeStamp = Config.packTimeStamp;

        gameChannelId = Config.gameChannelId;
        paymentChannelId = Config.paymentChannelId;
        isABResNameCrypto = Config.isABResNameCrypto;
        abResNameCryptoKey = Config.abResNameCryptoKey;
        abResByteOffset = Config.abResByteOffset;
        screenOrientation = (ScreenOrientation)Config.screenOrientation;
        gameInputType = Config.gameInputType;
        resLoadType = Config.resLoadType;
        firstOpenWindowName = Config.firstOpenWindowName;
        gameDesignRatio = Config.gameDesignRatio;


        serverDataList = Config.serverDataList;
        packDataList = Config.packDataList;

        packType = Config.packType;
        isRealPurchase = Config.isRealPurchase;
        isShowProtoMsgLog = Config.isShowProtoMsgLog;
        isShowDebugBtn = Config.isShowDebugBtn;
        isEnableLogTrace = Config.isEnableLogTrace;
        isEnableLogRealtimeWriteToLocal = Config.isEnableLogRealtimeWriteToLocal;
        isEnableLogUpdate2Server = Config.isEnableLogUpdate2Server;
        isShowReporter = Config.isShowReporter;
    }

    private void OnGUI()
    {

        configFoldOut = EditorGUILayout.BeginFoldoutHeaderGroup(configFoldOut, "配置");
        if (configFoldOut)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            GUILayout.Label("产品名", EditorStyles.boldLabel);
            productName = EditorGUILayout.TextField("ProductName:", productName);
            GUILayout.Label("版本信息：", EditorStyles.boldLabel);
            appVersion = EditorGUILayout.TextField("AppVersion:", appVersion);
            appBundleVersion = EditorGUILayout.TextField("AppBundleVersion:", appBundleVersion);
            resVersion = EditorGUILayout.TextField("ResVersion:", resVersion);
            GUILayout.BeginHorizontal();
            packTimeStamp = EditorGUILayout.TextField("PackTimeStamp", packTimeStamp);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("刷新时间戳"))
            {
                packTimeStamp = TimeTool.GetyyyyMMddHHmm(DateTime.Now, "").Substring(2);
            }
            GUILayout.EndHorizontal();


            GUILayout.Label("渠道信息：", EditorStyles.boldLabel);
            gameChannelId = EditorGUILayout.IntField("GameChannelId:", gameChannelId);
            paymentChannelId = EditorGUILayout.IntField("paymentChannelId:", paymentChannelId);

            GUILayout.Label("加密信息：", EditorStyles.boldLabel);
            isABResNameCrypto = bool.Parse(EditorGUILayout.TextField("IsABResNameCrypto:", isABResNameCrypto.ToString()));
            abResNameCryptoKey = EditorGUILayout.TextField("ABResNameCryptoKey:", abResNameCryptoKey);
            abResByteOffset = EditorGUILayout.IntField("ABResByteOffset:", abResByteOffset);
            GUILayout.Label("显示信息：", EditorStyles.boldLabel);
            //string[] values = { "Landscape", "Portrait" };
            //int index = EditorGUILayout.Popup(new GUIContent("ScreenOrientation"), Array.IndexOf(values, screenOrientation), new string[2] { "Landscape", "Portrait" });
            //screenOrientation = values["01".Contains(index.ToString()) ? index : 0];

            screenOrientation = (ScreenOrientation)EditorGUILayout.EnumPopup("横屏or竖屏", screenOrientation);

            GUILayout.Label("操作类型，1：鼠键，2：触屏", EditorStyles.boldLabel);
            gameInputType = EditorGUILayout.IntField("GameInputType:", gameInputType);
            GUILayout.Space(10);

            GUILayout.Label("资源加载类型，1：AB，2：Resources", EditorStyles.boldLabel);
            resLoadType = EditorGUILayout.IntField("ResLoadType:", resLoadType);
            GUILayout.Space(10);

            firstOpenWindowName = EditorGUILayout.TextField("FirstOpenWindowName:", firstOpenWindowName);
            GUILayout.Space(10);

            gameDesignRatio = EditorGUILayout.Vector2Field("GameDesignRatio", gameDesignRatio);

            GUILayout.Space(10);
            GUILayout.Label("服务器信息：", EditorStyles.boldLabel);
            if (serverDataList != null)
            {
                for (int i = 0; i < serverDataList.Count; i++)
                {
                    var tmp = serverDataList[i];
                    GUILayout.Label("服务器" + (i + 1) + " ----------->");
                    tmp.serverType = EditorGUILayout.IntField("ServerType:", tmp.serverType);
                    tmp.serverName = EditorGUILayout.TextField("ServerName:", tmp.serverName);
                    tmp.isLogEventToSDK = bool.Parse(EditorGUILayout.TextField("IsLogEventToSDK:", tmp.isLogEventToSDK.ToString()));
                }
            }

            GUILayout.Space(10);
            GUILayout.Label("打包信息：", EditorStyles.boldLabel);

            if (packDataList != null)
            {
                for (int i = 0; i < packDataList.Count; i++)
                {
                    var tmp = packDataList[i];
                    GUILayout.Label("打包" + (i + 1) + " ----------->");
                    tmp.packType = EditorGUILayout.TextField("PackType:", tmp.packType);
                    tmp.ftpTxtFileUrl = EditorGUILayout.TextField("FtpTxtFileUrl:", tmp.ftpTxtFileUrl);
                    tmp.ftpZipFileUrl = EditorGUILayout.TextField("FtpZipFileUrl:", tmp.ftpZipFileUrl);
                }
            }
            GUILayout.Space(10);
            GUILayout.Label("包体配置信息：", EditorStyles.boldLabel);
            packType = EditorGUILayout.TextField("PackType(PUB、DEV):", packType);
            isRealPurchase = bool.Parse(EditorGUILayout.TextField("IsRealPurchase:", isRealPurchase.ToString()).ToString());

            isShowProtoMsgLog = bool.Parse(EditorGUILayout.TextField("IsShowProtoMsgLog:", isShowProtoMsgLog.ToString()).ToString());
            isShowDebugBtn = bool.Parse(EditorGUILayout.TextField("IsShowDebugBtn:", isShowDebugBtn.ToString()).ToString());
            isEnableLogTrace = bool.Parse(EditorGUILayout.TextField("IsEnableLogTrace:", isEnableLogTrace.ToString()).ToString());
            isEnableLogRealtimeWriteToLocal = bool.Parse(EditorGUILayout.TextField("IsEnableLogRealtimeWriteToLocal:", isEnableLogRealtimeWriteToLocal.ToString()).ToString());
            isEnableLogUpdate2Server = bool.Parse(EditorGUILayout.TextField("IsEnableLogUpdate2Server:", isEnableLogUpdate2Server.ToString()).ToString());
            isShowReporter = bool.Parse(EditorGUILayout.TextField("IsShowReporter:", isShowReporter.ToString()).ToString());

            GUILayout.Space(10);

            if (GUILayout.Button("保存配置"))
            {
                var jsonStr = Config.AssignConfigDataToJson(productName, appVersion, appBundleVersion, resVersion, packTimeStamp, gameChannelId, paymentChannelId, isABResNameCrypto, abResNameCryptoKey, abResByteOffset, (int)screenOrientation, gameInputType, resLoadType, firstOpenWindowName, gameDesignRatio,
                    serverDataList, packDataList,
                    packType, isRealPurchase, isShowProtoMsgLog, isShowDebugBtn, isEnableLogTrace, isEnableLogRealtimeWriteToLocal, isEnableLogUpdate2Server, isShowReporter);
                Config.WriteToResConfig(jsonStr);
                DebugExt.Log("save success!");
            }


            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        ScriptOut = EditorGUILayout.BeginFoldoutHeaderGroup(ScriptOut, "配置");
        if (ScriptOut)
        {
            //获取当前是哪个平台
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            //获得当前平台已有的的宏定义
            var symbols = PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup));

            if (DIYScrips == null)
            {
                DIYScrips = new bool[ScripsNames.Length];
                for (int i = 0; i < ScripsNames.Length; i++)
                {
                    DIYScrips[i] = symbols.Contains(ScripsNames[i]);
                }
            }

            isDIYScrip = EditorGUILayout.BeginToggleGroup("自定义宏定义", isDIYScrip);
            if (isDIYScrip)
            {
                for (int i = 0; i < ScripsNames.Length; i++)
                {
                    DIYScrips[i] = EditorGUILayout.Toggle(ScripsNames[i], DIYScrips[i]);
                }
                if (GUILayout.Button("脚本配置保存"))
                {
                    string str = "";
                    for (int i = 0; i < DIYScrips.Length; i++)
                    {
                        if (DIYScrips[i] == true)
                        {
                            str += ";" + ScripsNames[i];
                        }
                    }
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, str);
                }
            }
            EditorGUILayout.EndToggleGroup();
            EditorPrefs.SetBool("isProjectScrips", EditorGUILayout.ToggleLeft("使用项目宏定义", EditorPrefs.GetBool("isProjectScrips", false)));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        //TODO:改客户端的productname版本号等

        packFoldOut = EditorGUILayout.BeginFoldoutHeaderGroup(packFoldOut, "打包");
        if (packFoldOut)
        {
            if (GUILayout.Button(" 打全量APK包"))
            {
                PackTool.BuildFullAPK();
            }
            if (GUILayout.Button("打热更APK包"))
            {
                PackTool.BuildHotupdateAPK();
            }
            if (GUILayout.Button("打全量Xcode包"))
            {
                PackTool.BuildFullXCodeProj();
            }
            if (GUILayout.Button("打热更Xcode包"))
            {
                PackTool.BuildHotXCodeProj();
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

}
