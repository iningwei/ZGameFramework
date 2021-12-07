using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using ZGame.HotUpdate;

public class PackTool
{


    [MenuItem("ZGame/打包/安卓/打全量apk")]
    public static void BuildFullAPK()
    {
        string macros = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        if (EditorUtility.DisplayDialog("警告", "当前宏为：" + macros + "------->是否继续??？", "OK", "Cancel"))
        {

            BuildLuaBundle.build();

            //System.Func<Task> func = async () =>
            //{
            //    await Task.Delay(System.TimeSpan.FromSeconds(2));
            Debug.LogError("copyResFilesToStreamingAssets");
            copyAllResFilesToStreamingAssets();


            Debug.LogError("buildAndroid");
            buildAndroid();

            Debug.Log("安卓 一键打全量包完毕！");
            AssetDatabase.Refresh();
            //};
            //func(); 
        }
        else
        {
            Debug.LogError("取消打包");
        }
    }


    static void buildAndroid()
    {
        var scenes = getBuildScenes();
        setKeystore();
        setVersion();
        BuildPipeline.BuildPlayer(scenes, Application.dataPath + "/../sgame-v" + PlayerSettings.bundleVersion + "-" + PlayerSettings.Android.bundleVersionCode + ".apk", BuildTarget.Android, BuildOptions.None);
    }
    static string[] getBuildScenes()
    {
        List<string> names = new List<string>();

        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null) { continue; }
            if (e.enabled) { names.Add(e.path); }
        }
        return names.ToArray();
    }
    static void setVersion()
    {
        PlayerSettings.bundleVersion = Config.appVersion;
        Debug.LogError("set app version:" + Config.appVersion);
#if UNITY_ANDROID
        PlayerSettings.Android.bundleVersionCode = int.Parse(Config.appBundleVersion);
        Debug.LogError("set bundleVersionCode:" + PlayerSettings.Android.bundleVersionCode);
#elif UNITY_IOS
        PlayerSettings.iOS.buildNumber =int.Parse(Config.appBundleVersion);
        Debug.LogError("set buildNumber:" + PlayerSettings.iOS.buildNumber);
#endif
    }
    static void setKeystore()
    {
        PlayerSettings.Android.keystoreName = Application.dataPath + "/../keystore/user.keystore";
        PlayerSettings.Android.keystorePass = "com.trillionjoy.petcoincrazymaster";
        PlayerSettings.Android.keyaliasName = "com.trillionjoy.petcoincrazymaster";
        PlayerSettings.Android.keyaliasPass = "com.trillionjoy.petcoincrazymaster";
        PlayerSettings.Android.useCustomKeystore = true;




    }



    static void copyAllResFilesToStreamingAssets()
    {
        string sourcePath = Application.dataPath + "/../ResEx/" + IOTools.PlatformFolderName;
        string targetPath = IOTools.CreateFolder(Application.dataPath + "/StreamingAssets/ResEx");

        IOTools.MoveFiles(sourcePath, targetPath, true);

        AssetDatabase.Refresh();
    }
}
