using System.Text.RegularExpressions;
using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using ZGame;


public class PackTool
{

    [MenuItem("工具/打包/IOS/打全量XCode工程")]
    public static void BuildFullXCodeProj()
    {
        string macros = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
        if (EditorUtility.DisplayDialog("警告", "打全量XCode工程  当前宏为：" + macros + "------->是否继续??？", "OK", "Cancel"))
        {
            BuildLuaBundle.build();

            Debug.LogError("copyResFilesToStreamingAssets");
            copyAllResFilesToStreamingAssets();

            Debug.LogError("build XCode");
            // buildXCode("f");
            EditorPrefs.SetInt("isPack", 1);
            MacroCodeDetection("buildXCode", "f");

            Debug.Log("全量 XCode 工程输出完毕！");
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError("取消打包");
        }
    }
    [MenuItem("工具/打包/IOS/打 热更 XCode工程")]
    public static void BuildHotXCodeProj()
    {
        string macros = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
        if (EditorUtility.DisplayDialog("警告", "打 热更 XCode工程  当前宏为：" + macros + "------->是否继续??？", "OK", "Cancel"))
        {
            BuildLuaBundle.build();

            Debug.LogError("copyHotUpdateInitResFilesToStreamingAssets");
            bool cr = copyHotUpdateInitResFilesToStreamingAssets();
            if (cr == false)
            {
                return;
            }


            Debug.LogError("build XCode");
            // buildXCode("h");
            EditorPrefs.SetInt("isPack", 1);
            MacroCodeDetection("buildXCode", "h");
            Debug.Log("热更 XCode 工程输出完毕！");
            AssetDatabase.Refresh();

        }
        else
        {
            Debug.LogError("取消打包");
        }
    }






    [MenuItem("工具/打包/安卓/打全量apk")]
    public static void BuildFullAPK()
    {
        string macros = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);

        if (EditorUtility.DisplayDialog("警告", "打全量apk 当前宏为：" + macros + "------->是否继续??？", "OK", "Cancel"))
        {

            BuildLuaBundle.build();


            Debug.LogError("copyResFilesToStreamingAssets");
            copyAllResFilesToStreamingAssets();


            // buildAndroid("f");
            EditorPrefs.SetInt("isPack", 1);
            MacroCodeDetection("buildAndroid", "f");
            Debug.Log("安卓 一键打全量包完毕!");
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError("取消打包");
        }
    }


    [MenuItem("工具/打包/安卓/打热更apk")]
    public static void BuildHotupdateAPK()
    {
        string macros = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        if (EditorUtility.DisplayDialog("警告", "打热更apk 当前宏为：" + macros + "------->是否继续??？", "OK", "Cancel"))
        {
            //打lua ab
            //BuildLuaBundle.build();
            //不用打了，否则就是最新的lua脚本了

            //从热更目录中 拷贝初始资源到StreamingAssets目录
            bool cr = copyHotUpdateInitResFilesToStreamingAssets();
            if (cr == false)
            {
                return;
            }

            // buildAndroid("h");
            EditorPrefs.SetInt("isPack", 1);
            MacroCodeDetection("buildAndroid", "h");

            Debug.Log("安卓  一键打热更包完毕！");
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError("取消打包");
        }
    }
    public static void MacroCodeDetection(string action, string suffix)
    {
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string symbols = PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup));
        if (EditorPrefs.GetInt("isPack", -1) != 1)
            return;
        if (!EditorPrefs.GetBool("isProjectScrips"))
        {
            if (EditorPrefs.GetString("ScripsArray", "") == "")
            {
                EditorPrefs.SetString("ScripsArray", symbols);
            }
            if (symbols.Contains("OriginLuaFile"))
            {
                Regex pattern = new Regex("OriginLuaFile(.*?;)?");
                symbols = pattern.Replace(symbols, "");
            }
            if (suffix == "f")
            {
                if (!symbols.Contains("XLua"))
                {
                    symbols += "XLua;";
                }
                if (symbols.Contains("HOTUPDATE"))
                {
                    Regex pattern = new Regex("HOTUPDATE(.*?;)?");
                    symbols = pattern.Replace(symbols, "");
                }
            }
            else if (suffix == "h")
            {
                if (!symbols.Contains("XLua"))
                {
                    symbols += "XLua;";
                }
                if (!symbols.Contains("HOTUPDATE"))
                {
                    symbols += "HOTUPDATE;";
                }
                Debug.LogError("增加XLua和HOTUPDATE后的宏列表：" + symbols);
            }
            Debug.LogError("最终宏列表：" + symbols);
            EditorPrefs.SetString("suffix", suffix);
            EditorPrefs.SetString("PackAction", action);
            PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup), symbols);
        }
        try
        {
            Type type = typeof(PackTool);//获取类名
            MethodInfo mt = type.GetMethod(action, System.Reflection.BindingFlags.IgnoreCase
                    | System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Static);//获取方法信息
            object obj = System.Activator.CreateInstance(type);
            Debug.LogError($"action:{action} ---- suffix:{suffix}");
            mt.Invoke(obj, new string[] { suffix });
            ClearCache();
        }
        catch (NullReferenceException e)
        {
            Debug.LogError(e);
            ClearCache();
        }

    }


    static void buildXCode(string suffix)
    {
        var scenes = getBuildScenes();

        setVersion();
        setProductName();

        AssetDatabase.SaveAssets();

        //BuildPipeline.BuildPlayer(scenes, Application.dataPath + "/../SGame-XCodeProj-v" + PlayerSettings.bundleVersion + "-" + PlayerSettings.iOS.buildNumber + "-" + suffix, BuildTarget.iOS, BuildOptions.None);
        string locatePathName = $"{Application.dataPath}/../{Config.productName}-XCodeProj-v{Config.appVersion}-{Config.appBundleVersion}-{suffix}";

        BuildPipeline.BuildPlayer(scenes, locatePathName, BuildTarget.iOS, BuildOptions.None);
    }

    static void buildAndroid(string suffix)
    {
        var scenes = getBuildScenes();
        setKeystore();

        setVersion();
        setProductName();



        AssetDatabase.SaveAssets();

        //BuildPipeline.BuildPlayer(scenes, Application.dataPath + "/../CrazyPets-v" + PlayerSettings.bundleVersion + "-" + PlayerSettings.Android.bundleVersionCode + "-" + suffix + ".apk", BuildTarget.Android, BuildOptions.None);
        string locatePathName = $"{Application.dataPath}/../{Config.productName}-v{Config.appVersion}-{Config.appBundleVersion}-{TimeTool.GetyyyyMMddHHmm(DateTime.Now, "")}-{suffix}.apk";

        BuildPipeline.BuildPlayer(scenes, locatePathName, BuildTarget.Android, BuildOptions.None);
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
        PlayerSettings.iOS.buildNumber =Config.appBundleVersion;
        Debug.LogError("set buildNumber:" + PlayerSettings.iOS.buildNumber);
#endif
    }
    static void setKeystore()
    {
        string bundleId = "com.candy.slot.casino.coin.master.crazy.pet";
        PlayerSettings.Android.keystoreName = Application.dataPath + "/../keystore/" + bundleId + "/user.keystore";
        PlayerSettings.Android.keystorePass = bundleId;
        PlayerSettings.Android.keyaliasName = bundleId;
        PlayerSettings.Android.keyaliasPass = bundleId;
        PlayerSettings.Android.useCustomKeystore = true;

    }
    static void setProductName()
    {
        PlayerSettings.productName = Config.productName;
    }


    [MenuItem("工具/打包/拷贝ResEx资源到StreamingAssets")]
    static void copyAllResFilesToStreamingAssets()
    {
        string sourcePath = Application.dataPath + "/../ResEx/" + IOTools.PlatformFolderName;
        string targetPath = Application.dataPath + "/StreamingAssets/ResEx";
        IOTools.CreateDirectorySafe(targetPath);
        IOTools.MoveFiles(sourcePath, targetPath, true);
        Debug.Log("copy finished");
        AssetDatabase.Refresh();
    }

    static bool copyHotUpdateInitResFilesToStreamingAssets()
    {
        string sourcePath = Application.dataPath + "/../hotupdate/" + IOTools.PlatformFolderName + "/channel_" + Config.gameChannelId + "/" + Config.appVersion + "/" + Config.resVersion + "/cur/res";
        if (Directory.Exists(sourcePath) == false)
        {
            Debug.LogError("directory not exist:" + sourcePath);
            return false;
        }
        string targetPath = Application.dataPath + "/StreamingAssets/ResEx";
        IOTools.CreateDirectorySafe(targetPath);

        IOTools.MoveFiles(sourcePath, targetPath, true);

        return true;
    }
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void onScriptDefineSymbolsReset()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            Debug.Log("isCompiling or updating");
            EditorApplication.delayCall += TODO2;
            return;
        }
        Debug.Log("DidReloadScripts");
        EditorApplication.delayCall += TODO2;
    }
    static void TODO2()
    {
        var suffix = EditorPrefs.GetString("suffix");
        var action = EditorPrefs.GetString("PackAction");
        var isPsck = EditorPrefs.GetInt("isPack", -1);
        if (isPsck == 1)
        {
            Debug.LogError($"suffix={suffix},action={action}");
            MacroCodeDetection(action, suffix);
        }
    }
    static void ClearCache()
    {
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        var ScripsArray = EditorPrefs.GetString("ScripsArray");
        EditorPrefs.DeleteKey("ScripState");
        EditorPrefs.DeleteKey("suffix");
        EditorPrefs.DeleteKey("PackAction");
        EditorPrefs.DeleteKey("ScripsArray");
        EditorPrefs.DeleteKey("isPack");
        Debug.LogError($"还原的宏定义：{ScripsArray}");
        PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup), ScripsArray);
    }

}
