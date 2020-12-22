using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;

public class CopyFile : MonoBehaviour
{
    //////[MenuItem("ZGame/拷贝客户端文件到debug工程")]
    //////static void CopyClientToClientDebugPorj()
    //////{
    //////    string path = Application.dataPath + "/../BatCmd/CopyClientToClientDebug.bat";
    //////    UnityEngine.Debug.LogError("目标路径：" + path);
    //////    Process.Start(path);
    //////}



    //////[MenuItem("ZGame/拷贝ab文件到StreamingAssets目录")]
    //////public static bool CopyAB2StreamingAssets()
    //////{
    //////    IOTools.DeleteAllFiles(Application.streamingAssetsPath + "/ResEx/", true);


    //////    string sourceFolder = Application.dataPath + "/../ResEx/" + IOTools.PlatformFolderName;
    //////    string desFolder = Application.streamingAssetsPath + "/ResEx/";
    //////    if (!Directory.Exists(desFolder))
    //////    {
    //////        Directory.CreateDirectory(desFolder);
    //////    }

    //////    string[] files = Directory.GetFiles(sourceFolder);
    //////    for (int i = 0; i < files.Length; i++)
    //////    {
    //////        FileInfo fi = new FileInfo(files[i]);
    //////        string fileName = fi.Name;
    //////        //由于fgui资源默认是放在Resources目录下的，是会被打出包内的。因此fgui的ab资源就不需要拷贝到StreamingAssets目录下了。
    //////        if (!fileName.Contains("fgui_res_"))
    //////        {
    //////            File.Copy(files[i], desFolder + fileName, true);
    //////        }

    //////    }
    //////    UnityEngine.Debug.Log("拷贝ab文件完毕，文件数：" + files.Length);



    //////    AssetDatabase.Refresh();
    //////    return true;
    //////}


    [MenuItem("ZGame/拷贝美术工程需要内容到客户端工程")]
    public static void CopyArtProj2ClientProj()
    {
        string path = Application.dataPath + "/../BatCmd/CopyArtProj2ClientProj.bat";
        Process.Start(path);
    }



}
