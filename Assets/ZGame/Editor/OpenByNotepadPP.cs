using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class OpenByNotepadPP : MonoBehaviour
{
    //http://docs.unity3d.com/ScriptReference/MenuItem.html
    [MenuItem("Assets/open with Notepad++", false, 1)]
    static void openByNotepadPP()
    {
        if (Selection.objects.Length != 1)//由于文本文件不属于GameObject类型，因此这里使用Selection.gameObjects是无法获取选择的文本文件的
        {
            UnityEngine.Debug.LogError("you can only open one file per time");
            return;
        }
        UnityEngine.Object obj = Selection.objects[0];
        string path = Application.dataPath + AssetDatabase.GetAssetPath(obj).Substring(6);
        if (!(path.EndsWith(".cs") || path.EndsWith(".txt")
            || path.EndsWith(".bin") || path.EndsWith(".xml")
            || path.EndsWith(".shader")
            || path.EndsWith(".bin")
            || path.EndsWith(".bytes")
             || path.EndsWith(".json")))
        {
            UnityEngine.Debug.LogError("you can only open .cs .txt .bin .xml .shader .bin .bytes file");
            return;
        }
        UnityEngine.Debug.Log(path + ",opened");
        //Process.Start(path);//使用默认打开方式打开文本文件
        Process.Start("notepad++.exe", path);//使用notepad++打开，需要机器安装了该软件
    }
}
