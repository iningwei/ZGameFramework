using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using ZGame.Window;

public class WindowAutoRegister
{



    [DidReloadScripts]
    public static void Auto()
    {

        gatherWindowName();

        registerWindow();

        AssetDatabase.Refresh();
    }

    static void gatherWindowName()
    {
        Debug.Log("GatherWindowName--->");
        string str = "public class WindowNames\n{\n";

        string path = "./Library/ScriptAssemblies/Assembly-CSharp.dll";
        byte[] buffer = File.ReadAllBytes(path);
        var assembly = Assembly.Load(buffer);
        foreach (var t in assembly.GetTypes())
        {
            if (t.IsClass && t.BaseType != null && t.BaseType.Name == typeof(Window).Name && t.Name.EndsWith("Window") && t.Name != "LuaBridgeWindow")
            {
                str += "\tpublic static string " + t.Name + " = \"" + t.Name + "\";\n";
                Debug.Log(t.Name);

            }
        }
        str += "}";

        string writePath = Application.dataPath + "/Scripts/Window/WindowNames.cs";
        string d = writePath.Substring(0, writePath.LastIndexOf('/'));
        if (!Directory.Exists(d))
        {
            Directory.CreateDirectory(d);
        }

        if (!File.Exists(writePath))
        {
            File.Create(writePath).Dispose();
        }
        File.WriteAllText(writePath, str);


    }
    static void registerWindow()
    {
        Debug.Log("RegisterWindow--->");
        string str = "using ZGame.Window;\npublic class WindowRegister\n{\n";
        str += "\tpublic static void Register()\n\t{\n";
        str += "\t\tvar winManager=WindowManager.Instance;\n";

        //TODO:
        //winManager.RegisterWindowType(WindowNames.Window_Battle, typeof(LoginWindow), "Window_Login");



        string path = "./Library/ScriptAssemblies/Assembly-CSharp.dll";
        byte[] buffer = File.ReadAllBytes(path);
        var assembly = Assembly.Load(buffer);
        foreach (var t in assembly.GetTypes())
        {
            if (t.IsClass && t.BaseType != null && t.BaseType.Name == typeof(Window).Name && t.Name.EndsWith("Window") && t.Name != "LuaBridgeWindow")
            {
                str += $"\t\twinManager.RegisterWindowType(WindowNames.{t.Name},typeof({t.Name}).ToString(),\"{t.Name}\");\n";
                Debug.Log(t.Name);
            }
        }
        str += "\t}\n";
        str += "}";

        string writePath = Application.dataPath + "/Scripts/Window/WindowRegister.cs";
        string d = writePath.Substring(0, writePath.LastIndexOf('/'));
        if (!Directory.Exists(d))
        {
            Directory.CreateDirectory(d);
        }


        if (!File.Exists(writePath))
        {
            File.Create(writePath).Dispose();
        }
        File.WriteAllText(writePath, str);
    }
}
