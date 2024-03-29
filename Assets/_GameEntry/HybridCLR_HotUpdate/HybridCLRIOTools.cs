using UnityEngine;
using System.Collections;
using System.IO;
using ZGame.Obfuscation;

public class HybridCLRIOTools
{
    /// <summary>
    /// 平台差异的文件夹名称
    /// </summary>
    public static string PlatformFolderName
    {
        get
        {
#if UNITY_IOS
            return "ios";
#elif UNITY_ANDROID
            return "android";
#elif UNITY_STANDALONE_WIN
            return "pc_win";
#elif UNITY_STANDALONE_OSX
            return "pc_mac";
#else
            return "other";
#endif
        }
    }

    static string resStreamingPath;

    static string resPersistantPath;

    static string resEditorResExPath;
    /// <summary>
    /// 后缀
    /// </summary>
    public static string abSuffix
    {
        get
        {
#if UNITY_IOS
            return ".iab";
#elif UNITY_ANDROID
            return ".aab";
#elif UNITY_STANDALONE_WIN
            return ".wab";
#elif UNITY_STANDALONE_OSX
            return ".mab";
#else
            return ".oab";
#endif
        }
    }

    /// <summary>
    /// 后缀，不带.
    /// </summary>
    public static string abSuffixWithoutPoint
    {
        get
        {
            return abSuffix.Replace(".", "");
        }
    }

    static HybridCLRIOTools()
    {
        init();
    }

    static void init()
    {
        //--TODO: ResEx和res名字统一一下
        resPersistantPath = Application.persistentDataPath + "/" + HybridCLRConfig.appVersion + "/res";
        resStreamingPath = Application.streamingAssetsPath + "/ResEx";

#if UNITY_EDITOR
        resEditorResExPath = Application.dataPath + "/../ResEx/" + HybridCLRIOTools.PlatformFolderName;
#endif



        //http://www.ihaiu.com/unity-load_android_StreamingAssets/
        //https://blog.csdn.net/iningwei/article/details/89097130
        ////在Unity2020.3.20f1中获得的路径形如，安卓--------> /data/app/com.candy.slot.casino.coin.master.crazy.pet-gVpEGOnhM4LspGbQIMOu6w==/base.apk
        ////在Unity2021.3.3f1中获得的路径形如，安卓--------> /data/app/com.candy.slot.casino.coin.master.crazy.pet-u40z6rK4TAR_wd8HlNrQ9w==/base.apk
        //string testDataPath = Application.dataPath;
        ////在Unity2020.3.20f1中获得的路径形如，安卓--------> /storage/emulated/0/Android/data/com.candy.slot.casino.coin.master.crazy.pet/files
        ////在Unity2021.3.3f1中获得的路径形如，安卓--------> /storage/emulated/0/Android/data/com.candy.slot.casino.coin.master.crazy.pet/files
        //string testPPath = Application.persistentDataPath;
        ////在Unity2020.3.20f1中获得的路径形如，安卓--------> jar:file:///data/app/com.candy.slot.casino.coin.master.crazy.pet-gVpEGOnhM4LspGbQIMOu6w==/base.apk!/assets
        ////在Unity2021.3.3f1中获得的路径形如，安卓-------->  jar:file:///data/app/com.candy.slot.casino.coin.master.crazy.pet-zklTrTkLQZ2tVGvqP1FFwQ==/base.apk!/assets
        //string testSPath = Application.streamingAssetsPath;
        //Debug.LogError("testDataPath:" + testDataPath);
        //Debug.LogError("testPPath:" + testPPath);
        //Debug.LogError("testSPath:" + testSPath);



        if (!Directory.Exists(resPersistantPath))
        {
            Directory.CreateDirectory(resPersistantPath);
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            //经笔者测试，StreamingAssets目录下的AB资源使用AssetBundle.LoadFromFile()加载的话
            //在较老版本unity中，资源路径只能使用：Application.dataPath + "!assets/XXX"的方式
            //在Unity2020.3.20f1中，既支持 Application.streamingAssetsPath +"/XXX"的方式，又支持上述较老版本的路径方式
            //在Unity2021.3.3f1中，只支持  Application.streamingAssetsPath +"/XXX" 的方式

            //resStreamingPath = Application.dataPath + "!assets/ResEx";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {

#if UNITY_IOS
            //设置iOS沙盒不备份该路径
            //iOS上可能会因为这个原因审核不通过
            UnityEngine.iOS.Device.SetNoBackupFlag(Application.persistentDataPath);
#endif
        }

    }


    public static string GetResStreamingPath(string resName)
    {
        return resStreamingPath + "/" + resName;
    }


    public static string GetResPersistantPath(string resName)
    {
        return resPersistantPath + "/" + resName;
    }

    public static string GetResEditorResExPath(string resName)
    {
        return resEditorResExPath + "/" + resName;
    }

    public static byte[] ReadFile(string path)
    {
        var data = File.ReadAllBytes(path);
        return data;
    }

    public static string ReadStringFromUpdateDir(string fileName)
    {
        return GetFileString(resPersistantPath + "/" + fileName);
    }


    public static bool IsResInEditorResExDir(string resName)
    {
        string path = resEditorResExPath + "/" + resName;
        if (File.Exists(path))
        {
            return true;
        }
        return false;
    }

    public static bool IsResInPersistantDir(string resName)
    {
        string path = resPersistantPath + "/" + resName;
        if (File.Exists(path))
        {
            return true;
        }
        return false;
    }

    public static string[] readAllLinesFromUpdateDir(string fileName)
    {
        string path = resPersistantPath + "/" + fileName;
        if (File.Exists(path))
        {
            return File.ReadAllLines(path);
        }
        return null;
    }



    public static void WriteFileToUpdateDir(string name, byte[] data)
    {
        if (data != null && data.Length > 0)
            File.WriteAllBytes(resPersistantPath + "/" + name, data);
    }

    public static void WriteStringToUpdateDir(string fileName, string str)
    {
        File.WriteAllText(resPersistantPath + "/" + fileName, str);
    }

    public static void DeleteFileFromUpdateDir(string name)
    {
        string s = resPersistantPath + "/" + name;
        if (File.Exists(s))
            File.Delete(s);
    }



    /// <summary>
    /// 删除文件夹下所有的文件
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="recursive">是否循环删除子文件夹</param>
    public static void DeleteAllFiles(string dir, bool recursive)
    {
        if (!Directory.Exists(dir))
        {
            Debug.LogWarning("warning, DeleteAllFiles dir not exist：" + dir);
            return;
        }

        DirectoryInfo di = new DirectoryInfo(dir);
        FileSystemInfo[] fsi = di.GetFileSystemInfos();
        foreach (var item in fsi)
        {
            if (item is DirectoryInfo)
            {
                if (recursive)
                {
                    DirectoryInfo subDir = new DirectoryInfo(item.FullName);
                    subDir.Delete(true);//删除该目录以及目录下的文件                    
                }

            }
            else
            {
                File.Delete(item.FullName);
            }
        }
    }






    /// <summary>
    /// 把文件从源文件夹下拷贝到目标文件夹下
    /// </summary>
    /// <param name="originDic"></param>
    /// <param name="desDic"></param>
    public static void MoveFiles(string originDic, string desDic, bool clearDesDicFirst)
    {
        if (!Directory.Exists(originDic))
        {
            Debug.LogError("Error, dir not exist：" + originDic);
            return;
        }
        if (clearDesDicFirst)
        {
            DeleteAllFiles(desDic, true);
        }

        string[] files = Directory.GetFiles(originDic);
        for (int i = 0; i < files.Length; i++)
        {
            File.Copy(files[i], desDic + "/" + Path.GetFileName(files[i]));
        }

    }


    /// <summary>
    /// 读取文本文件的内容
    /// </summary>
    /// <param name="filePath"></param>
    public static string GetFileString(string filePath)
    {
        if (File.Exists(filePath))
        {
            var b = File.ReadAllBytes(filePath);
            using (TextReader tr = new StreamReader(new MemoryStream(b)))
            {
                return tr.ReadToEnd();
            }
        }
        else
        {
            Debug.LogWarning("warning, GetFileString failed,no file with filePath:" + filePath);
            return "";
        }
    }



    public static void WriteString(string path, string content)
    {
        string d = path.Substring(0, path.LastIndexOf('/'));
        if (!Directory.Exists(d))
        {
            Directory.CreateDirectory(d);
        }

        if (!File.Exists(path))
        {
            File.Create(path).Dispose();
        }
        File.WriteAllText(path, content);
    }


    /// <summary>
    /// 获得文件所在的文件夹名
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetFileFolderName(string filePath)
    {
        var directory = Path.GetDirectoryName(filePath);
        return Path.GetFileNameWithoutExtension(directory);
    }


    public static string CreateFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }


    public static string[] getAllABFilesInUpdateDir()
    {
        return Directory.GetFiles(resPersistantPath, "*" + abSuffix, SearchOption.TopDirectoryOnly);
    }
    public static byte[] GetResFileDataFromUpdateDir(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("GetResFileData name is null!");
        }
        string uppath = resPersistantPath + "/" + name;

        if (File.Exists(uppath))
        {
            return File.ReadAllBytes(uppath);
        }
        return null;
    }


    public static bool existFileInUpdateDir(string fileName)
    {
        return File.Exists(resPersistantPath + "/" + fileName);
    }



    public static string GetABPath(string name)
    {
        name = name.ToLower();

        name = (HybridCLRConfig.isABResNameCrypto ? DES.EncryptStrToHex(name, HybridCLRConfig.abResNameCryptoKey) : name) + HybridCLRIOTools.abSuffix;

        return GetFilePath(name);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="realName">name with suffix</param>
    /// <returns></returns>
    public static string GetFilePath(string realName)
    {
        string path = "";
        if (HybridCLRIOTools.IsResInPersistantDir(realName))
        {
            path = HybridCLRIOTools.GetResPersistantPath(realName);
        }
#if UNITY_EDITOR
        else if (HybridCLRIOTools.IsResInEditorResExDir(realName))
        {
            path = HybridCLRIOTools.GetResEditorResExPath(realName);
        }
#endif
        else
        {
            path = HybridCLRIOTools.GetResStreamingPath(realName);
        }

        return path;
    }
}