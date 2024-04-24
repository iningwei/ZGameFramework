using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZGame;
using ZGame.Obfuscation;

public class HybridCLRResUpdateTool
{
    public static string hybridCLRPlatformFolderName
    {
        get
        {
            string name = "";
#if UNITY_ANDROID
            name = "Android";
#elif UNITY_IOS
             name = "iOS";
#elif UNITY_STANDALONE_WIN
        name = "StandaloneWindows64";
#elif UNITY_STANDALONE_OSX
            Debug.LogError("TODO:set name!!");
#else
       Debug.LogError("TODO:set name!!");     
#endif
            return name;
        }

    }
    [MenuItem("HybridCLRHotUpdate/移动待更新DLL到ResEX目录")]
    public static void MoveDll2ResEx()
    {
        string fileName = "Assembly-CSharp.dll";
        string fileNameFinal = DES.EncryptStrToHex(fileName, Config.abResNameCryptoKey);
        string sourcePath = Application.dataPath + string.Format("/../HybridCLRData/HotUpdateDlls/{0}/", hybridCLRPlatformFolderName) + fileName;

        string destPath = Application.dataPath + string.Format("/../ResEx/{0}/", IOTools.PlatformFolderName) + fileNameFinal + ".bytes";
        File.Copy(sourcePath, destPath, true);
        Debug.Log($"copy {sourcePath} to {destPath}");
        AssetDatabase.Refresh();
    }

    [MenuItem("HybridCLRHotUpdate/移动内置匹配当前平台的DLL到StreamingAssets目录")]
    static void MoveInnerDll2StreamingAssets()
    {
        //mscorlib.dll
        //System.dll
        //System.Core.dll
        List<string> sourcePaths = new List<string>();
        sourcePaths.Add(string.Format(Application.dataPath + "/../HybridCLRData/AssembliesPostIl2CppStrip/{0}/mscorlib.dll", hybridCLRPlatformFolderName));
        sourcePaths.Add(string.Format(Application.dataPath + "/../HybridCLRData/AssembliesPostIl2CppStrip/{0}/System.dll", hybridCLRPlatformFolderName));
        sourcePaths.Add(string.Format(Application.dataPath + "/../HybridCLRData/AssembliesPostIl2CppStrip/{0}/System.Core.dll", hybridCLRPlatformFolderName));

        List<string> desPaths = new List<string>();
        desPaths.Add(Application.streamingAssetsPath + "/mscorlib.dll.bytes");
        desPaths.Add(Application.streamingAssetsPath + "/System.dll.bytes");
        desPaths.Add(Application.streamingAssetsPath + "/System.Core.dll.bytes");
        for (int i = 0; i < sourcePaths.Count; i++)
        {
            File.Copy(sourcePaths[i], desPaths[i], true);
            Debug.Log($"copy {sourcePaths[i]} to {desPaths[i]}");
        }
        AssetDatabase.Refresh();
    }

}
