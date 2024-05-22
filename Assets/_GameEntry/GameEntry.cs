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
using System.Runtime.InteropServices;


public class GameEntry : MonoBehaviour
{
    public Transform normalTipTran;
    public Text normalTipTxt;


    public Transform errorTipTran;
    public Text errorTipTxt;

    public Transform progressTran;
    public Image progressImg;
    public Text progressTxt;

    public Transform bigUpdateTran;

    public Image splashImage;
    float splashUsedTime = 0f;
    bool isSplashFinished = false;
    void Start()
    {
        normalTipTran.gameObject.SetActive(false);
        errorTipTran.gameObject.SetActive(false);
        progressTran.gameObject.SetActive(false);
        bigUpdateTran.gameObject.SetActive(false);


        splashImage.gameObject.SetActive(true);

    }


    bool isNetworkReachable = false;
    int checkedCount = 0;

    private void Update()
    {
        splashUsedTime += Time.deltaTime;
        if (splashUsedTime > 3 && isSplashFinished == false)
        {
            isSplashFinished = true;
            splashImage.gameObject.SetActive(false);

            SDKExt.TryWebRequest();
        }
        if (this.isSplashFinished)
        {
            Debug.Log("checkedCount:" + checkedCount);
            if (checkedCount > 100)
            {
                ShowNormalTip("当前网络不通！");
                return;
            }
            if (isNetworkReachable == false && IsNetworkReachability())
            {
                isNetworkReachable = true;
                startWork();
            }
        }
    }

    void startWork()
    {
#if HybridCLR_HOTUPDATE
        HybridCLRServerListDownload.Instance.Download(this);
#else
        this.EnterGame();
#endif 
    }

    /// <summary>
    /// 网络可达性
    /// </summary>
    /// <returns></returns>
    public bool IsNetworkReachability()
    {
        checkedCount++;
        switch (Application.internetReachability)
        {
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                Debug.Log("当前使用的是：WiFi ");
                return true;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                Debug.Log("当前使用的是移动网络 ");
                return true;
            default:
                Debug.LogError("当前没有联网 ");
                return false;
        }
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

    public void ShowBigUpdate()
    {
        this.bigUpdateTran.gameObject.SetActive(true);
        Button confirmBtn = this.bigUpdateTran.Find("bg/ConfirmBtn").GetComponent<Button>();
        confirmBtn.onClick.RemoveAllListeners();
        confirmBtn.onClick.AddListener(this.onBigUpdateConfirmBtnClicked);

    }

    private void onBigUpdateConfirmBtnClicked()
    {
#if !UNITY_EDITOR
#if UNITY_IOS
        SDKExt.openAppStore(HybridCLRServerList.Instance.IOSJumpStoreKey);
#endif

 
#if UNITY_ANDROID
        SDKExt.openAndroidDownloadPage(HybridCLRServerList.Instance.AndroidJumpStoreKey);
#endif
#endif 
    }


    public void ShowProgress(float ratio)
    {
        if (progressTran.gameObject.activeInHierarchy == false)
        {
            progressTran.gameObject.SetActive(true);
        }
        progressImg.fillAmount = ratio;
        progressTxt.text = (ratio * 100).ToString("0.0") + "%";
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
