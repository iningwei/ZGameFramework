using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;

namespace ZGame.RessEditor
{
    public class AutoCreatLuaWinCodeAndRegitWinTool : EditorWindow
    {
        UnityEngine.Object prefab;
        [MenuItem("工具/强制编译")]
        static void ForcedToCompile()
        {
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }
        [MenuItem("工具/自动注册lua窗口")]
        static void PrefabChanger()
        {
            EditorWindow.GetWindow(typeof(AutoCreatLuaWinCodeAndRegitWinTool));
        }
        private void OnGUI()
        {
            prefab = EditorGUILayout.ObjectField("窗口预制体：", prefab, typeof(UnityEngine.Object), false);
            string prefabPath =  AssetDatabase.GetAssetPath(prefab);
            if (prefab == null)
                return;
            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(prefab))
            {
                GUILayout.Label("该物体不是预制件,请选择预制件;");
                return;
            }
            if (!prefab.name.EndsWith("Window"))
            {
                GUILayout.Label("名称应以Window结束");
                return;
            }
            Regex rgx = new Regex("^Assets/.*/Window/(.*)/.*Window\\..*");
            if (!rgx.IsMatch(AssetDatabase.GetAssetPath(prefab)))
            {
                GUILayout.Label("文件目录结构错误，应放在Window目录下新建文件夹存放");
                return;
            }
            if (GUILayout.Button("Lua窗口文件创建+自动注册"))
            {

                string fileDir = Environment.CurrentDirectory;
                fileDir = fileDir.Replace('\\', '/');
               string desFilePath = fileDir + "/LuaTemplate";
                Match match = rgx.Match(prefabPath);
                string TargetPath = fileDir + "/../project/LuaScript/Windows/" + match.Groups[1];

                if (!Directory.Exists(desFilePath))
                {
                    Debug.LogError($"{desFilePath}---“LuaWindowTemplate.lua”模板文件不存在");
                    return;
                }
                try
                {
                    if (!Directory.Exists(TargetPath))
                    {
                        Directory.CreateDirectory(TargetPath);
                    }

                }
                catch (IOException e)
                {
                    Debug.LogError("文件夹创建失败:" + e);
                    return;
                }
                try
                {
                    if (File.Exists(TargetPath + "/" + prefab.name + ".lua"))
                    {
                        throw new IOException("文件已存在！");
                    }
                    File.Copy(desFilePath + "/LuaWindowTemplate.lua", TargetPath + "/" + prefab.name + ".lua");
                    string filePath = TargetPath + "/" + prefab.name + ".lua";
                    StreamReader sr = new StreamReader(filePath);
                    string str = sr.ReadToEnd();
                    sr.Close();
                    str = str.Replace("$FolderName$", match.Groups[1].ToString());
                    str = str.Replace("$WindowPrefabName$", prefab.name);
                    str = str.Replace("-- ", "");
                    StreamWriter sw = new StreamWriter(filePath, false);
                    sw.WriteLine(str);
                    sw.Close();
                    Debug.LogError("Lua创建成功");
                }
                catch (IOException e)
                {
                    Debug.LogError("Lua创建失败:" + e);
                }
                try
                {
                    string winRegitfilePath = fileDir + "/../project/LuaScript/Windows/Base/LuaWinRegister.lua";
                    string winresconfigTxt = $"{prefab.name} = {{name = \"{prefab.name}\", package = \"{prefab.name}\", comName = \"{prefab.name}\"}},";
                    string winregconfigTxt = $"WindowManager:RegisterLuaWindowType(WinResConfig.{prefab.name}.name,WinResConfig.{prefab.name}.package)";
                    StreamReader regsr = new StreamReader(winRegitfilePath);
                    string regstr = regsr.ReadToEnd();
                    regsr.Close();
                    if (regstr.Contains(winresconfigTxt))
                    {
                        Debug.LogError("WinResConfig已添加");
                    }
                    else
                    {
                        regstr = regstr.Replace("WinResConfig = {", "WinResConfig = {\n\t" + winresconfigTxt);
                    }
                    if (regstr.Contains(winregconfigTxt))
                    {
                        Debug.LogError("RegisterAllLuaWindowType已添加");
                    }
                    else
                    {
                        regstr = regstr.Replace("function RegisterAllLuaWindowType()", "function RegisterAllLuaWindowType()\n\t" + winregconfigTxt);
                    }
                    StreamWriter regsw = new StreamWriter(winRegitfilePath, false);
                    regsw.WriteLine(regstr);
                    regsw.Close();
                    Debug.LogError("Lua窗体注册成功");
                }
                catch (IOException e)
                {
                    Debug.LogError("Lua注册失败：" + e);
                }
                EditorUtility.DisplayDialog("成功", "执行完成！\n执行情况输出在控制台", "确认");
            }

        }
    }
}