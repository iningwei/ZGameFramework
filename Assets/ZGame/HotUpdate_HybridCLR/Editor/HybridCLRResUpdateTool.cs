using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZGame;
using ZGame.Obfuscation;

public class HybridCLRResUpdateTool
{
    [MenuItem("HybridCLRHotUpdate/移动待更新DLL到ResEX目录")]
    static void MoveDll2ResEx()
    {
        //TODO:针对其它平台的处理

        string fileName = "Assembly-CSharp.dll";
        string fileNameFinal = DES.EncryptStrToHex(fileName, Config.abResNameCryptoKey);
        string sourcePath = Application.dataPath + "/../HybridCLRData/HotUpdateDlls/StandaloneWindows64/" + fileName;

        string destPath = Application.dataPath + "/../ResEx/pc_win/" + fileNameFinal + ".bytes";
        File.Copy(sourcePath, destPath, true);
        Debug.Log($"copy {sourcePath} to {destPath}");
    }

    [MenuItem("HybridCLRHotUpdate/移动内置DLL到StreamingAssets目录")]
    static void MoveInnerDll2StreamingAssets()
    {
        //mscorlib.dll
        //System.dll
        //System.Core.dll
        List<string> sourcePaths = new List<string>();
        sourcePaths.Add(Application.dataPath + "/../HybridCLRData/AssembliesPostIl2CppStrip/StandaloneWindows64/mscorlib.dll");
        sourcePaths.Add(Application.dataPath + "/../HybridCLRData/AssembliesPostIl2CppStrip/StandaloneWindows64/System.dll");
        sourcePaths.Add(Application.dataPath + "/../HybridCLRData/AssembliesPostIl2CppStrip/StandaloneWindows64/System.Core.dll");

        List<string> desPaths = new List<string>();
        desPaths.Add(Application.streamingAssetsPath + "/mscorlib.dll.bytes");
        desPaths.Add(Application.streamingAssetsPath + "/System.dll.bytes");
        desPaths.Add(Application.streamingAssetsPath + "/System.Core.dll.bytes");
        for (int i = 0; i < sourcePaths.Count; i++)
        {
            File.Copy(sourcePaths[i], desPaths[i], true);
            Debug.Log($"copy {sourcePaths[i]} to {desPaths[i]}");
        }
    }

}
