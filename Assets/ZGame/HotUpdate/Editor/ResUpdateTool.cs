using UnityEditor;
using UnityEngine;
using ZGame;

public class ResUpdateTool : EditorWindow
{
    static BuildTargetGroup buildTargetGroup;
    static string macros;
    [MenuItem("HotUpdate/游戏热更/一键整理热更代码和资源")]
    public static void GenUpdateResList()
    {
        ResUpdateTool resTool = EditorWindow.GetWindow(typeof(ResUpdateTool)) as ResUpdateTool;
        buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        macros = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("version:" + Config.appVersion);
        EditorGUILayout.LabelField("resVersion:" + Config.resVersion);
        EditorGUILayout.LabelField("channelId:" + Config.gameChannelId);
        GUILayout.Space(10);
        EditorGUILayout.LabelField("macros:" + macros);
        GUILayout.Space(10);

        if (GUILayout.Button("Confirm"))
        {
            new HotResCollector().Build();
            this.Close();
        }
        if (GUILayout.Button("Cancel"))
        {
            this.Close();
        }
    }
}
