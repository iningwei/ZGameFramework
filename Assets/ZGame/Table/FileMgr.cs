using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;
using ZGame;
using ZGame.Obfuscation;
using UnityEngine.Networking;
using Unity.VisualScripting;

public class FileMgr
{
    static bool isEncrypt = true;//数据表文件是否加密
    static string encryptKey = "goodluck";//数据表文件加密密钥 
    public static void ReadFile(string fileName, Action<string> onLoadFinish)
    {
        string fullPath = _getFileFullName(fileName);
        //Debug.Log("readFile:" + fullPath + ",originName:" + fileName);
        //fullPath在android下有可能位于StreamingAssets下，对应的是一个类zip文件，是不能使用system.io下的接口的
        string androidSymbol = ".apk!/assets/";
        if (fullPath.Contains(androidSymbol))
        {
            // xxx.apk!/assets/abc.bin 
            int index = fullPath.IndexOf(androidSymbol);
            string loaderUsedPath = fullPath.Substring(index + androidSymbol.Length);

            if (!IOAndroidLoader.Instance.IsFileExist(loaderUsedPath))
            {
                Debug.LogError("file not exist:" + fullPath);
                return;
            }
            var data = IOAndroidLoader.Instance.GetBytes(loaderUsedPath);
            handleBinFileData(data, onLoadFinish);
        }
        else
        {
            if (!File.Exists(fullPath))
            {
                Debug.LogError("file not exist:" + fullPath);
                return;
            }

            using (Stream stream = File.OpenRead(fullPath))
            {
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);

                handleBinFileData(data, onLoadFinish);
            }
        }
    }

    private static void handleBinFileData(byte[] data, Action<string> onLoadFinish)
    {
        string text = Encoding.UTF8.GetString(data, 0, data.Length);
        if (isEncrypt)
        {
            text = _textDecrypt(text.ToCharArray(), encryptKey);
        }

        if (onLoadFinish != null)
        {
            onLoadFinish(text);
        }
    }




    static string _getFileFullName(string fileName)
    {
        //////string localPath = _getLocalPath();
        //////string subPath = string.Format("/table_output/{0}.bin", fileName);
        //////localPath = string.Format(localPath + subPath, fileName);
        //////return localPath;

        if (Config.isABResNameCrypto)
        {
            fileName = DES.EncryptStrToHex(fileName, Config.abResNameCryptoKey);
        }
        return IOTools.GetFilePath(fileName + ".bin");
    }


    static string _getLocalPath()
    {
        string localPath = string.Empty;
#if UNITY_EDITOR && !EDITOR_AS_PC
        localPath = Application.streamingAssetsPath;
#elif UNITY_ANDROID
        localPath=Application.persistentDataPath;
#elif UNITY_IOS
        localPath=Application.persistentDataPath;
#else
        localPath=Application.streamingAssetsPath;
#endif

        return localPath;
    }


    static string _textDecrypt(char[] data, string secretKey)
    {
        char[] key = secretKey.ToCharArray();
        for (int i = 0; i < data.Length; i++)
        {
            data[i] ^= key[i % key.Length];
        }
        return new string(data);
    }
}
