using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
 
using ZGame.Obfuscation;
using ZGame.Pack;

public class BuildLuaBundle
{
    public static BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle |
        BuildAssetBundleOptions.ChunkBasedCompression |
        BuildAssetBundleOptions.ForceRebuildAssetBundle;


    static string abPath = Application.dataPath + "/../ResEx/" + IOTools.PlatformFolderName;
    static string luaABTMP_Path = Application.dataPath + "/../LUA_AB_TMP";

    static Dictionary<string, Dictionary<string, string>> editorLuaScripts = new Dictionary<string, Dictionary<string, string>>();



    [MenuItem("ZGame/打包/打ab包/打lua AB")]
    public static void build()
    {

        abPath = IOTools.CreateFolder(abPath);
        luaABTMP_Path = IOTools.CreateFolder(luaABTMP_Path);


        //先把所有lua脚本拷贝到Assets/TempLuaScript目录下
        string tmpPath = IOTools.CreateFolder(Application.dataPath + "/TempLuaScript");
        IOTools.DeleteAllFiles(tmpPath, true);

        editorLuaScripts.Clear();
        getAllLuaFiles(Application.dataPath + "/../LuaScript");


        Dictionary<string, List<string>> assets = new Dictionary<string, List<string>>();
        foreach (var v in editorLuaScripts)
        {
            foreach (var v2 in v.Value)
            {
                if (!assets.ContainsKey(v.Key))
                {
                    assets[v.Key] = new List<string>();
                }
                assets[v.Key].Add("Assets/TempLuaScript/" + v2.Key + ".bytes");
                File.Copy(v2.Value, tmpPath + "/" + v2.Key + ".bytes");
            }
        }


        AssetBundleBuild[] buildMap = new AssetBundleBuild[editorLuaScripts.Count];
        int index = 0;
        foreach (var v in editorLuaScripts)
        {
            buildMap[index].assetBundleName = v.Key + IOTools.abSuffix;
            string[] assetArray = assets[v.Key].ToArray();
            buildMap[index].assetNames = assetArray;
            index++;

            Debug.Log($"---->打Lua AB：{v.Key}{IOTools.abSuffix},包含资源数：{assetArray.Length}");
        }


        IOTools.DeleteAllFiles(luaABTMP_Path, true);
        BuildPipeline.BuildAssetBundles(luaABTMP_Path, buildMap, options, EditorUserBuildSettings.activeBuildTarget);

        //删除打包产生的不必要的文件（平台文件和平台manifest文件，ab包的manifest文件）
        var allFiles = Directory.GetFileSystemEntries(luaABTMP_Path);
        foreach (var v in allFiles)
        {
            if (!v.EndsWith(IOTools.abSuffix))
            {
                File.Delete(v);
            }
        }
        AssetDatabase.Refresh();



        if (PackConfig.ABResEncrypt)
        {
            Debug.LogError("对lua ab res 加密");
            //对lua ab加密
            foreach (var v in editorLuaScripts)
            {
                string logicPath = luaABTMP_Path + "/" + v.Key + IOTools.abSuffix;
                XOR.EncryptFile(logicPath, Application.identifier);
                Debug.Log($"{v.Key}{IOTools.abSuffix} 加密完毕");
            }
        }



        string[] files = Directory.GetFiles(luaABTMP_Path);
        for (int i = 0; i < files.Length; i++)
        {
            string fileName = Path.GetFileNameWithoutExtension(files[i]);
            string fileExt = Path.GetExtension(files[i]);

            string newFileName = PackConfig.ABNameConfuse ? ZGame.Obfuscation.DES.EncryptStrToHex(fileName, Application.identifier) : fileName;
            string targetFile = abPath + "/" + newFileName + fileExt;
            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
            }
            File.Copy(files[i], targetFile);
        }


        //UnityEngine.UI.ScrollView x;
        //x.onItemRender.AddListener






        Debug.Log("打包 lua bundle完毕:" + abPath);
    }

    static void getAllLuaFiles(string folderPath)
    {
        DirectoryInfo directory = new DirectoryInfo(folderPath);
        DirectoryInfo[] directoryArray = directory.GetDirectories();
        FileInfo[] fileInfoArray = directory.GetFiles();
        if (fileInfoArray.Length > 0)
        {
            foreach (var f in fileInfoArray)
            {
                if (f.Name.EndsWith(".lua"))
                {
                    //UnityEngine.Debug.Log("add lua file path:" + f.Name);


                    string lowFullName = f.FullName.ToLower();
                    string key = "";
                    if (lowFullName.Contains("subgamealchemist"))
                    {
                        key = "logic_subgamealchemist";
                    }
                    else if (lowFullName.Contains("subgamebountyhunter"))
                    {
                        key = "logic_subgamebountyhunter";
                    }
                    else if (lowFullName.Contains("subgamediamond777"))
                    {
                        key = "logic_subgamediamond777";
                    }
                    else if (lowFullName.Contains("subgamefishdragon"))
                    {
                        key = "logic_subgamefishdragon";
                    }
                    else if (lowFullName.Contains("subgamefruitboom"))
                    {
                        key = "logic_subgamefruitboom";
                    }
                    else if (lowFullName.Contains("subgamegoldminer"))
                    {
                        key = "logic_subgamegoldminer";
                    }
                    else if (lowFullName.Contains("subgamemega777"))
                    {
                        key = "logic_subgamemega777";
                    }
                    else if (lowFullName.Contains("subgamebuffalo"))
                    {
                        key = "logic_subgamebuffalo";
                    }
                    else
                    {
                        key = "logic";
                    }

                    if (!editorLuaScripts.ContainsKey(key))
                    {
                        editorLuaScripts[key] = new Dictionary<string, string>();
                    }
                    editorLuaScripts[key].Add(f.Name.ToLower(), f.FullName);
                }
            }
        }
        foreach (DirectoryInfo _directoryInfo in directoryArray)
        {
            getAllLuaFiles(_directoryInfo.FullName);//递归遍历  
        }

    }
}
