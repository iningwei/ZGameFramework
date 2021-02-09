using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if XLua
using XLua;
using ZGame.Net.Tcp;
using ZGame.Ress.AB;
#endif

public class LuaResLoader : Singleton<LuaResLoader>
{
#if XLua
    public LuaResLoader()//构造函数，通过单例调用
    {
    }

    Action onLogicFileLoaded = null;
    public void Init(Action onLogicLoaded)
    {
        onLogicFileLoaded = onLogicLoaded;
#if UNITY_EDITOR && !HOTUPDATE
        //得到脚本路径下所有.lua脚本
        Debug.Log("lua script：load by ordinary .lua file");
        getAllLuaFiles(Application.dataPath + "/../LuaScript");
        onLogicFileLoaded?.Invoke();
#else
        Debug.Log("lua logic：load by ab");
        LoadScriptBundle("logic");
        onLogicFileLoaded?.Invoke();
#endif
    }

    /// <summary>
    /// 非编辑器环境，通过bundle加载的lua文件资源映射
    /// </summary>
    Dictionary<string, string> codeFileMap = new Dictionary<string, string>();
#if UNITY_EDITOR
    //编辑器下脚本相对路径
    Dictionary<string, string> editorScriptPath = new Dictionary<string, string>();
    /// <summary>
    /// 编辑器模式下得到指定目录下所有lua脚本
    /// </summary> 
    void getAllLuaFiles(string strDirectory)
    {
        rootPath = Application.dataPath.Replace("Assets", "") + "LuaScript/";
        DirectoryInfo directory = new DirectoryInfo(strDirectory);
        DirectoryInfo[] directoryArray = directory.GetDirectories();
        FileInfo[] fileInfoArray = directory.GetFiles();
        if (fileInfoArray.Length > 0)
        {
            foreach (var f in fileInfoArray)
            {
                if (f.Name.EndsWith(".lua"))
                {
                    //UnityEngine.Debug.Log("add lua file path:" + f.Name);
                    editorScriptPath.Add(f.Name.ToLower(), f.FullName.Replace("\\", "/"));
                }
            }
        }
        foreach (DirectoryInfo _directoryInfo in directoryArray)
        {
            getAllLuaFiles(_directoryInfo.FullName);//递归遍历
        }
    }
    string rootPath;
    public string GetLuaFilePath(string fileName)
    {
        var strs = fileName.ToLower().Split('/');
        fileName = strs[strs.Length - 1];

        if (!fileName.Contains(".lua"))
            fileName += ".lua";



        return editorScriptPath[fileName].Replace(rootPath, "");
    }
#endif

    /// <summary>
    /// 从bundle中加载脚本资源
    /// </summary> 
    public void LoadScriptBundle(string name)
    {

        var tas = ABManager.Instance.LoadLogic(name);
        for (int i = 0; i < tas.Length; i++)
        {
            string key = tas[i].name.ToLower();
            if (codeFileMap.ContainsKey(key))
            {
                codeFileMap[key] = tas[i].text;
            }
            else
            {
                codeFileMap.Add(key, tas[i].text);
            }
        }



    }



    /// <summary>
    /// 重写 读取lua文件函数
    /// 编辑器环境下，若开启HOTUPDATE宏，则从bundle读取；否则从外部指定目录直接读取lua
    /// 在非编辑器环境下，统一从lua  bundle文件读取，bundle文件可用来更新
    /// </summary> 
    public string ReadFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return null;

        var strs = fileName.ToLower().Split('/');
        fileName = strs[strs.Length - 1];

        if (!fileName.Contains(".lua"))
            fileName += ".lua";

        ///读取文件

#if UNITY_EDITOR && !HOTUPDATE
        string str = null;

        if (editorScriptPath.ContainsKey(fileName))
        {
            str = File.ReadAllText(editorScriptPath[fileName]);
        }
        else
        {
            Debug.LogError("can not find lua file：" + fileName);
        }
        return str;
#else

        string str = null;
        codeFileMap.TryGetValue(fileName, out str);
        if (str != null)
        {
            return str;
        }
        else
        {
            Debug.LogError("can not file file:" + fileName);
            return null;
        }

#endif

    }
    public void Dispose()
    {
        codeFileMap.Clear();
    }
#endif
}




public class ScriptManager : Singleton<ScriptManager>
{
#if XLua
    LuaEnv myLuaEnv = null;
#endif

    public void Init()
    {
#if XLua
        LuaResLoader.Instance.Init(() =>
        {

            Debug.Log("ScriptManager Init");
            myLuaEnv = new LuaEnv();
            myLuaEnv.AddBuildin("rapidjson", XLua.LuaDLL.Lua.LoadRapidJson);
            myLuaEnv.AddBuildin("lpeg", XLua.LuaDLL.Lua.LoadLpeg);
            myLuaEnv.AddBuildin("pb", XLua.LuaDLL.Lua.LoadLuaProfobuf);
            myLuaEnv.AddBuildin("ffi", XLua.LuaDLL.Lua.LoadFFI);
            //myLuaEnv.AddBuildin("BigNumber", XLua.LuaDLL.Lua.LoadBigNumber);
            //myLuaEnv.AddBuildin("sproto.core", XLua.LuaDLL.Lua.LoadSproto);

            myLuaEnv.AddLoader((ref string filename) =>
            {
                if (filename == "InMemory")
                {
                    return System.Text.Encoding.UTF8.GetBytes("return {ccc = 9999}");
                }
                return System.Text.Encoding.UTF8.GetBytes(LuaResLoader.Instance.ReadFile(filename));
            });

#if LUA_DEBUG && UNITY_EDITOR
        DoFile("RemoteDebug");
#endif

            DoFile("Main");
        });
#endif
    }
#if XLua
    public void SetSocketMsg(string serverAddress, string identityToken)
    {
        Action<string> luaSetSocketMsg = ScriptManager.Instance.GetLuaActionWithP1<string>("SetSocketMsg");
        if (luaSetSocketMsg != null)
        {
            string msg = serverAddress + "|" + identityToken;
            luaSetSocketMsg(msg);
        }
        else
        {
            Debug.LogError("luaSetSocketMsg is null");
        }
    }


    public void OnDestroy()
    {
        myLuaEnv.Dispose();
        myLuaEnv = null;
    }

    /// <summary>
    /// 执行语句
    /// </summary>
    public void DoString(string chunk, string chunkName = "LuaState.cs")
    {
        myLuaEnv.DoString(chunk, chunkName);
    }

    /// <summary>
    /// 执行脚本
    /// </summary>
    public object[] DoFile(string fileName)
    {

        if (!string.IsNullOrEmpty(fileName))
        {
            var src = LuaResLoader.Instance.ReadFile(fileName);
            if (!string.IsNullOrEmpty(src))
                return myLuaEnv.DoString(src, fileName + ".lua");
        }
        return null;
    }


    public LuaFunction GetFunction(string funcName)
    {
        if (myLuaEnv == null)
            return null;
        return myLuaEnv.Global.Get<LuaFunction>(funcName);
    }
    public LuaTable GetTable(string name)
    {
        if (myLuaEnv == null)
            return null;
        return myLuaEnv.Global.Get<LuaTable>(name);
    }

    public Action GetLuaAction(string funcName)
    {
        if (myLuaEnv == null)
            return null;
        return myLuaEnv.Global.Get<Action>(funcName);
    }
    public Action<P1, P2> GetLuaActionWithP2<P1, P2>(string funcName)
    {
        if (myLuaEnv == null)
            return null;
        return myLuaEnv.Global.Get<Action<P1, P2>>(funcName);
    }
    public Action<P1> GetLuaActionWithP1<P1>(string funcName)
    {
        if (myLuaEnv == null)
            return null;
        return myLuaEnv.Global.Get<Action<P1>>(funcName);
    }

    public void CallFunctionNoParaAndReturn(string funcName)
    {
        var func = GetFunction(funcName);
        if (func != null)
        {
            func.Call();
            func.Dispose();
        }
    }

    public object[] CallFunction(string funcName, params object[] paras)
    {
        var func = GetFunction(funcName);
        if (func != null)
        {
            var rets = func.Call(paras);
            func.Dispose();
            return rets;
        }
        return null;
    }

    public LuaTable CallLuaNewWindowFunc(string scriptName)
    {
        var func = GetFunction("NewLuaWindow");
        if (func != null)
        {
            return func.Call(scriptName)[0] as LuaTable;
        }
        return null;
    }

    public LuaTable CallLuaNewActionFunc(string scriptName)
    {
        var func = GetFunction("NewLuaAction");
        if (func != null)
        {
            return func.Call(scriptName)[0] as LuaTable;
            //func.BeginPCall();
            //func.Push(scriptName);
            //func.PCall();
            //var table = func.CheckLuaTable();
            //func.EndPCall(); 
        }
        return null;
    }

    public LuaTable CallLuaNewGameStateFunc(string scriptName)
    {
        var func = GetFunction("NewGameStateAction");
        if (func != null)
        {
            return func.Call(scriptName)[0] as LuaTable;
        }
        return null;
    }

    public void Update()
    {
        if (myLuaEnv != null)
        {
            myLuaEnv.Tick();
        }



    }
#endif
}
