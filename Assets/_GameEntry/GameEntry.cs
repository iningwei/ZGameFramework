#if HybridCLR_INSTALLED
using HybridCLR;
#endif 
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class GameEntry : MonoBehaviour
{
    public Transform normalTipTran;
    public Text normalTipTxt;


    public Transform errorTipTran;
    public Text errorTipTxt;

    public Transform progressTran;
    public Text progressTxt;

    string dyStartToken = "";
    string dyScreenFullScreen = "0";
    string dyScreenWidth = "1080";
    string dyScreenHeight = "1920";
    string dyCloudGame = "0";
    string dyMobile = "0";

    string dyStartTokenHeader = "-token=";
    string dyFullScreenHeader = "-screen-fullscreen";
    string dyScreenHeightHeader = "-screen-height";
    string dyScreenWidthHeader = "-screen-width";
    string dyScreenCloudGameHeader = "-cloud-game";
    string dyPlatformMobileHeader = "-mobile";
    void Start()
    {
        var param = System.Environment.GetCommandLineArgs();




        var argsDic = this.collectArgs();
        argsDic.TryGetValue(dyStartTokenHeader, out dyStartToken);
        argsDic.TryGetValue(dyFullScreenHeader, out dyScreenFullScreen);
        argsDic.TryGetValue(dyScreenWidthHeader, out dyScreenWidth);
        argsDic.TryGetValue(dyScreenHeightHeader, out dyScreenHeight);
        argsDic.TryGetValue(dyScreenCloudGameHeader, out dyCloudGame);
        argsDic.TryGetValue(dyPlatformMobileHeader, out dyMobile);


        PlayerPrefs.SetString("dyStartToken", dyStartToken);
        PlayerPrefs.SetString("dyCloudGame", dyCloudGame);
        PlayerPrefs.SetString("dyMobile", dyMobile);
        PlayerPrefs.Save();
        Debug.Log("set dyStartToken:" + dyStartToken);
        Debug.Log("set dyCloudGame:" + dyCloudGame);
        Debug.Log("set dyMobile:" + dyMobile);
        if (dyCloudGame == "1")//云游戏
        {
            //////移动端启动
            ////if (dyMobie == "1")//移动端启动
            ////{

            ////}
            ////else//伴侣启动
            ////{ 
            ////}

            int realWidth;
            if (int.TryParse(dyScreenWidth, out realWidth) == false)
            {
                realWidth = 1080;
            }

            int realHeight;
            if (int.TryParse(dyScreenHeight, out realHeight) == false)
            {
                realHeight = 1920;
            }

            bool isFullScreen = dyScreenFullScreen == "1" ? true : false;

            Screen.SetResolution(realWidth, realHeight, isFullScreen);
            Debug.Log($"GameEntry dyCloudGame, setResolution, width:{realWidth},height:{realHeight},isFullScreen:{isFullScreen}");
        }
        else
        {
            //int width = PlayerPrefs.GetInt("DefaultWidth", 1080);
            //int height = PlayerPrefs.GetInt("DefaultHeight", 1920);

            //Screen.SetResolution(width, height, false);
            //Debug.Log($"GameEntry, setResolution, width:{width},height:{height}");

        }

        normalTipTran.gameObject.SetActive(false);
        errorTipTran.gameObject.SetActive(false);
        progressTran.gameObject.SetActive(false);

#if HybridCLR_HOTUPDATE 
        HybridCLRServerListDownload.Instance.Download(this);
#else
        this.EnterGame();
#endif 
    }


    /// <summary>
    /// 收集启动参数
    /// </summary>
    /// <returns>启动参数的键值表</returns>
    private Dictionary<string, string> collectArgs()
    {
        Dictionary<string, string> keyValues = new();

        HashSet<string> argKeys = new();
        argKeys.Add("-token=");
        argKeys.Add("-screen-fullscreen");
        argKeys.Add("-screen-height");
        argKeys.Add("-screen-width");
        argKeys.Add("-cloud-game");
        argKeys.Add("-mobile");

        //
        Debug.Log("-------------->");
        string[] args = System.Environment.GetCommandLineArgs();
        foreach (string arg in args)
        {
            Debug.Log("arg:" + arg);
        }
        Debug.Log("<--------------");


        for (int i = 0; i < args.Length; i++)
        {
            //token
            if (args[i].Contains(dyStartTokenHeader))
            {
                dyStartToken = args[i].Substring(dyStartTokenHeader.Length);
                keyValues[dyStartTokenHeader] = dyStartToken;
            }


            //云游戏全屏分辨率
            if (args[i].Contains(dyFullScreenHeader))
            {
                dyScreenFullScreen = args[i + 1];
                if (dyScreenFullScreen == "1")
                {
                    dyScreenWidth = "1080";
                    dyScreenHeight = "1920";
                }
                keyValues[dyFullScreenHeader] = dyScreenFullScreen;
                keyValues[dyScreenWidthHeader] = dyScreenWidth;
                keyValues[dyScreenHeightHeader] = dyScreenHeight;
            }

            //云游戏分辨率高度
            if (args[i].Contains(dyScreenHeightHeader))
            {
                dyScreenHeight = args[i + 1];
                keyValues[dyScreenHeightHeader] = dyScreenHeight;
            }

            //云游戏分辨率宽度
            if (args[i].Contains(dyScreenWidthHeader))
            {
                dyScreenWidth = args[i + 1];
                keyValues[dyScreenWidthHeader] = dyScreenWidth;
            }

            //云游戏屏蔽分辨率UI
            if (args[i].Contains(dyScreenCloudGameHeader))
            {
                dyCloudGame = args[i + 1];
                keyValues[dyScreenCloudGameHeader] = dyCloudGame;
            }

            //云游戏Mobile平台
            if (args[i].Contains(dyPlatformMobileHeader))
            {
                dyMobile = args[i + 1];
                keyValues[dyPlatformMobileHeader] = dyMobile;
            }
        }

        return keyValues;
    }

    public void ShowNormalTip(string tip)
    {
        normalTipTran.gameObject.SetActive(false);
        normalTipTran.gameObject.SetActive(true);
        normalTipTxt.text = tip;
    }

    public void ShowErrorTip(string tip)
    {
        errorTipTran.gameObject.SetActive(true);
        errorTipTxt.text = tip;

    }

    public void ShowProgress(float ratio)
    {
        if (progressTran.gameObject.activeInHierarchy == false)
        {
            progressTran.gameObject.SetActive(true);
        }
        progressTxt.text = "解压资源:" + (ratio * 100).ToString("0.0") + "%";
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            WinPCLogUpload.UploadToServer("http://log.qiyoogame.com/Gm/save-file/auto", () =>
            {

                this.ShowNormalTip("日志上传成功");

            }, () =>
            {
                this.ShowNormalTip("日志上传失败");
            });
        }
    }


    public void EnterGame()
    {
        // Editor环境下，热更dll已经被自动加载，不需要加载，重复加载反而会出问题。
#if !UNITY_EDITOR && HybridCLR_HOTUPDATE
        Debug.Log("not editor enviroment load hot assembly");
        // 先补充元数据
        LoadMetadataForAOTAssemblies();

        //加载热更dll
        string dllName = "Assembly-CSharp.dll";
        dllName = (HybridCLRConfig.isABResNameCrypto ? HybridCLRDES.EncryptStrToHex(dllName, HybridCLRConfig.abResNameCryptoKey) : dllName) + ".bytes";

        string path = HybridCLRIOTools.GetFilePath(dllName);
        Debug.Log("begin load dll:" + path);
        byte[] dllDatas = loadData(path);
        Assembly hotUpdateAss = Assembly.Load(dllDatas);
#else
        Debug.Log("editor load hot assembly");
        // Editor下无需加载，直接查找获得HotUpdate程序集
        Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "Assembly-CSharp");
#endif

        Type gameBridge = hotUpdateAss.GetType("GameBridge");
        gameBridge.GetMethod("LoadLauncherScene").Invoke(null, null);
    }
#if HybridCLR_HOTUPDATE
    private static void LoadMetadataForAOTAssemblies()
    {
        List<string> aotDllList = new List<string>
        {
            "mscorlib.dll",
            "System.dll",
            "System.Core.dll", // 如果使用了Linq，需要这个
            // "Newtonsoft.Json.dll", 
            // "protobuf-net.dll",
        };

        foreach (var aotDllName in aotDllList)
        {
            string fullPath = $"{Application.streamingAssetsPath}/{aotDllName}.bytes";

            var dllBytes = loadData(fullPath);
            if (dllBytes == null)
            {
                continue;
            }

            int err = (int)HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, HomologousImageMode.SuperSet);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. ret:{err}");
        }
    }

    private static byte[] loadData(string fullPath)
    {
        string androidSymbol = ".apk!/assets/";
        byte[] dllBytes;
        if (fullPath.Contains(androidSymbol))
        {
            int index = fullPath.IndexOf(androidSymbol);
            string loaderUsedPath = fullPath.Substring(index + androidSymbol.Length);
            if (!HybridCLRIOAndroidLoader.Instance.IsFileExist(loaderUsedPath))
            {
                Debug.LogError("file not exist:" + fullPath);
                return null;
            }
            dllBytes = HybridCLRIOAndroidLoader.Instance.GetBytes(loaderUsedPath);
        }
        else
        {
            if (!File.Exists(fullPath))
            {
                Debug.LogError("file not exist:" + fullPath);
                return null;
            }

            dllBytes = File.ReadAllBytes(fullPath);
        }
        return dllBytes;
    }
#endif
}
