using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;




public class IOTools
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
            return "other"
#endif
        }
    }

    static string resStreamingPath;

    static string resPersistantPath;

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

    static IOTools()
    {
        init();
    }

    static void init()
    {
        resPersistantPath = Application.persistentDataPath + "/ResEx";
        if (!Directory.Exists(resPersistantPath))
        {
            Directory.CreateDirectory(resPersistantPath);
        }

        resStreamingPath = Application.streamingAssetsPath + "/ResEx";

        if (Application.platform == RuntimePlatform.Android)
        {
            resStreamingPath = Application.dataPath + "!assets/ResEx";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IOS
            //设置iOS沙盒不备份该路径
            //iOS上可能会因为这个原因审核不通过
            UnityEngine.iOS.Device.SetNoBackupFlag(Application.persistentDataPath);
#endif
        }
        else
        {
            resStreamingPath = Application.dataPath + "/" + "../ResEx/" + PlatformFolderName;

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







    public static bool IsResInPDir(string resName)
    {
        string uppath = resPersistantPath + "/" + resName;
        if (File.Exists(uppath))
        {
            return true;
        }
        return false;
    }





    public static void WriteFileToUpdateDir(string name, byte[] data)
    {
        if (data != null && data.Length > 0)
            File.WriteAllBytes(resPersistantPath + name, data);
    }

    public static void WriteStringToUpdateDir(string name, string str)
    {
        File.WriteAllText(resPersistantPath + name, str);
    }

    public static void DeleteFileFromUpdateDir(string name)
    {
        string s = resPersistantPath + name;
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
            Debug.LogWarning("warning, DeleteAllFiles dir不存在：" + dir);
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
    /// 检测目标文件夹中是否有同名文件,不支持子文件夹查找
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="fileName">文件名，带后缀</param>
    /// <returns></returns>
    public static bool ExistSameNameFile(string dir, string fileName)
    {
        if (!Directory.Exists(dir))
        {
            Debug.LogError("Error, dir不存在：" + dir);
            return false;
        }

        string[] files = Directory.GetFiles(dir);
        for (int i = 0; i < files.Length; i++)
        {
            string name = Path.GetFileName(files[i]);
            if (name == fileName)
            {
                return true;
            }
        }
        return false;
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
            Debug.LogError("Error, dir不存在：" + originDic);
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
}