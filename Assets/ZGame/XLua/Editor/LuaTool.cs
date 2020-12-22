using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System;
using System.IO;
using System.Collections.Generic;

public class LuaTool : MonoBehaviour
{
    #if XLua

    [MenuItem("ZGame/LuaTool/GenUnityAPIForEmmyLua")]
    public static void GenUnityAPIForLua()
    {
        //C:\Program Files\Unity2018.3.0f2\Editor\Data\Managed\UnityEngine\UnityEngine.PhysicsModule.dll
        //版本比较新的Unity客户端，不再统一用UnityEngine.dll，而是按照功能点分了很多个dll
        //var assembly = Assembly.LoadFrom("./Library/UnityAssemblies/UnityEngine.dll");

        string outputUnity3DFolder = Application.dataPath + "/../LuaAPI";
        IOTools.DeleteAllFiles(outputUnity3DFolder, true);


        List<string> allFiles = new List<string>(Directory.GetFiles(Application.dataPath + "/../LuaCSharpDll", "*.dll", SearchOption.TopDirectoryOnly));
        allFiles.Add(Application.dataPath + "/../Library/ScriptAssemblies/Assembly-CSharp.dll");

        for (int i = 0; i < allFiles.Count; i++)
        {
            var assembly = Assembly.LoadFile(allFiles[i]);
            outputCode(assembly, outputUnity3DFolder);
        }

        Debug.LogError("gen finished");
    }


    static void outputCode(Assembly assembly, string outputFolder)
    {
        foreach (Type type in assembly.GetTypes())
        {
            if (type.FullName.Contains("<") || type.FullName.Contains("/") || type.FullName.Contains("+")
                || type.FullName.Contains("`"))
            {
                Debug.LogWarning("<color=red>含非法字符的</color>，排除,  FullName:" + type.FullName);
                continue;
            }
            if (!LuaCallCsList.LuaCallCSharp.Contains(type))//排除不需要的
            {
                Debug.LogWarning("<color=green>工程用不到的</color>，排除,  FullName:" + type.FullName);
                continue;
            }


            //Debug.Log("fullName:" + type.FullName + ",  name:" + type.Name + ",  methods count:" + type.GetMethods().Length + ", fields count:" + type.GetFields().Length);
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            string filePath = outputFolder + "/Assist_" + type.FullName + ".lua";//加个Assist_前缀，防止辅助代码和真正要使用的lua代码有名字冲突，导致require出错
            string fileContent = getUnityFileContent(type);
            File.WriteAllText(filePath, fileContent);

        }
    }



    //[MenuItem("My/LuaTool//GenFairyAPIForLua")]
    public static void GenFairyAPIForLua()
    {
        var assembly = Assembly.LoadFrom(Application.dataPath + "/../zhouhui_test/FairyGUI.dll");

        writeFairyGlobalTable();

        foreach (Type type in assembly.GetTypes())
        {
            if (type.FullName.Contains("<") || type.FullName.Contains("/") || type.FullName.Contains("+"))
            {
                Debug.LogWarning("<color=red>含非法字符的</color>，continue,  FullName:" + type.FullName);
                continue;
            }
            if (type.FullName.Contains("SimpleJson"))//排除不需要的
            {
                Debug.LogWarning("<color=green>工程用不到的</color>，continue,  FullName:" + type.FullName);
                continue;
            }


            //Debug.Log("fullName:" + type.FullName + ",  name:" + type.Name + ",  methods count:" + type.GetMethods().Length + ", fields count:" + type.GetFields().Length);

            string filePath = Application.dataPath + "/../zhouhui_test/FairyGUI5.4.0LUA/" + type.FullName + ".lua";
            string fileContent = getFairyFileContent(type);
            File.WriteAllText(filePath, fileContent);

        }

        Debug.LogError("finished");
    }

    private static void writeFairyGlobalTable()
    {
        string filePath = Application.dataPath + "/../zhouhui_test/FairyGUI5.4.0LUA/FairyGUI.lua";
        string fileContent = "FairyGUI={}";
        File.WriteAllText(filePath, fileContent);

        Debug.LogError("生成Global FairyGUI.lua");
    }


    static string GetLuaType(Type csType)
    {
        string luaType = "";
        if (csType == typeof(int) ||
            csType == typeof(short) ||
            csType == typeof(long) ||
            csType == typeof(double) ||
            csType == typeof(float)
             )
        {
            luaType = "number";
        }
        else if (csType == typeof(bool))
        {
            luaType = "boolean";
        }
        else if (csType == typeof(string))
        {
            luaType = "string";
        }
        else
        {
            var typeStr = csType.ToString();
            luaType = typeStr.Substring(typeStr.LastIndexOf(".") + 1);
        }

        return luaType;
    }
    private static string getUnityFileContent(Type item)
    {
        string itemName = item.Name;
        if (itemName == "GameObjectExt")
        {
            itemName = "GameObject";
        }
        if (itemName == "TransformExt")
        {
            itemName = "Transform";
        }

        string content = "---@class " + itemName + "\r\n";

        content += itemName + "={}\r\n";

        content += "\r\n---****************************Fields****************************\r\n";
        foreach (FieldInfo field in item.GetFields())//字段 (枚举类型也归于字段)
        {

            //content += "---@field public " + field.Name + " " + GetLuaType(field.FieldType) + " @" + "[字段] 类型:" + field.FieldType + "\r\n";
            content += "---@type " + GetLuaType(field.FieldType) + " @" + "[字段] 类型:" + field.FieldType + "\r\n";
            content += itemName + "." + field.Name + "=nil\r\n";
        }

        content += "\r\n---****************************Properties****************************\r\n";
        foreach (PropertyInfo prop in item.GetProperties())//属性
        {
            //Debug.LogError("property:" + prop.Name + ", propertyType:" + prop.PropertyType); 
            //content += "---@field public " + prop.Name + " " + GetLuaType(prop.PropertyType) + " @" + "[属性] 类型:" + prop.PropertyType + "\r\n";
            content += "---@type " + GetLuaType(prop.PropertyType) + " @" + "[字段] 类型:" + prop.PropertyType + "\r\n";
            content += itemName + "." + prop.Name + "=nil\r\n";
        }

        content += "\r\n---****************************Methods****************************\r\n";
        foreach (MethodInfo method in item.GetMethods())
        {
            if (method.Name.Contains("get_") || method.Name.Contains("set_"))//去掉属性对应的get、set方法
            {
                continue;
            }


            //Debug.LogError("method:" + method.Name + ", 返回类型：" + method.ReturnParameter.ParameterType);

            content += "---@return " + GetLuaType(method.ReturnParameter.ParameterType) + " @" + "对应C#方法:" + method.ReturnParameter.ParameterType + " " + method.Name + "(" + getParamsCS(method) + ")\r\n";
            //参数
            for (int i = 0; i < method.GetParameters().Length; i++)
            {
                var para = method.GetParameters()[i];

                Type type = para.ParameterType;
                content += "---@param " + para.Name + " " + GetLuaType(type) + "\r\n";
            }

            if (method.IsStatic)
            {
                //https://www.bbsmax.com/A/A7zgq9o54n/
                if (method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false))//是扩展方法
                {
                    content += "function " + itemName + ":" + method.Name + "(" + getParamsLUA(method) + ")\r\n" + "end\r\n\r\n";
                }
                else
                {
                    content += "function " + itemName + "." + method.Name + "(" + getParamsLUA(method) + ")\r\n" + "end\r\n\r\n";
                }
            }
            else
            {
                content += "function " + itemName + ":" + method.Name + "(" + getParamsLUA(method) + ")\r\n" + "end\r\n\r\n";
            }
        }

        return content;
    }


    private static string getFairyFileContent(Type item)
    {
        string content = "FairyGUI." + item.Name + "={}\r\n";

        content += "\r\n--- /****************************Fields****************************\r\n";
        foreach (FieldInfo field in item.GetFields())//字段 (枚举类型也归于字段)
        {
            //Debug.LogError("field:" + field.Name);
            content += "--- <summary>\r\n";
            content += "--- [字段] 类型:" + field.FieldType + "\r\n";
            content += "--- </summary>\r\n";
            if (field.Name == "inst")
            {
                Debug.Log(item.Name + "含有inst字段,特殊处理");
                content += "FairyGUI." + item.Name + "." + field.Name + "=" + "FairyGUI." + item.Name + "\r\n";
            }
            else
            {
                content += "FairyGUI." + item.Name + "." + field.Name + "=nil\r\n";
            }
        }


        content += "\r\n--- ****************************Properties****************************\r\n";
        foreach (PropertyInfo prop in item.GetProperties())//属性
        {
            //Debug.LogError("property:" + prop.Name + ", propertyType:" + prop.PropertyType);
            content += "--- <summary>\r\n";
            content += "--- [属性] 类型:" + prop.PropertyType + "\r\n";
            content += "--- </summary>\r\n";

            if (prop.Name == "inst")
            {
                Debug.Log(item.Name + "含有inst属性,特殊处理");
                content += "FairyGUI." + item.Name + "." + prop.Name + "=" + "FairyGUI." + item.Name + "\r\n\r\n";
            }
            else
            {
                content += "FairyGUI." + item.Name + "." + prop.Name + "=nil\r\n\r\n";
            }
        }

        content += "\r\n--- ****************************Methods****************************\r\n";
        foreach (MethodInfo method in item.GetMethods())
        {
            if (method.Name.Contains("get_") || method.Name.Contains("set_"))//去掉属性对应的get、set方法
            {
                continue;
            }
            //Debug.LogError("method:" + method.Name + ", 返回类型：" + method.ReturnParameter.ParameterType);
            content += "--- <summary>\r\n";
            content += "--- 对应C#方法:" + method.ReturnParameter.ParameterType + " " + method.Name + "(" + getParamsCS(method) + ")\r\n";
            content += "--- </summary>\r\n";
            content += "function " + "FairyGUI." + item.Name + ":" + method.Name + "(" + getParamsLUA(method) + ")\r\n" + "end\r\n\r\n";
        }

        return content;
    }


    private static string getParamsCS(MethodInfo method)
    {
        string paras = "";
        if (method.GetParameters().Length > 0)
        {
            foreach (var item in method.GetParameters())
            {
                paras += item.ParameterType + " " + item.Name + ",";
            }
        }

        if (paras.EndsWith(","))
        {
            paras = paras.Remove(paras.Length - 1);
        }
        return paras;
    }

    private static string getParamsLUA(MethodInfo method)
    {
        string paras = "";

        var paramInfos = method.GetParameters();
        for (int i = 0; i < paramInfos.Length; i++)
        {
            paras += paramInfos[i].Name + ",";
        }

        if (paras.EndsWith(","))
        {
            paras = paras.TrimEnd(',');
        }
        return paras;
    }
#endif
}
