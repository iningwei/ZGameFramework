using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if XLua
using XLua;

public class ActorMonoBehaviour : MonoBehaviour
{
    private LuaTable luaTable;
    private Action luaAwake;
    private Action luaStart;
    private Action luaUpdate;
    private Action luaOnEnable;
    private Action luaOnDestroy;

    private void Awake()
    {
        Debug.LogError("awake");

        //////this.luaTable = ScriptManager.Instance.InitMonoBehaviour(this);


        //////luaAwake = CallLuaFunction("Awake");
        //////luaStart = CallLuaFunction("Start");
        //////luaUpdate = CallLuaFunction("Update");
        ////////luaOnEnable = CallLuaFunction("OnEnable");
        ////////luaOnDestroy = CallLuaFunction("OnDestroy");


        //////luaAwake?.Invoke();


        //////LuaFunction luaFunc = ScriptManager.Instance.CallYYYYFunction("Awake");
        //////if (luaFunc == null)
        //////{
        //////    Debug.LogError("xxxxxxxxx");
        //////}
        //////else
        //////{
        //////    Debug.LogError("yyyyy");
        //////}
    }


    //////private Action CallLuaFunction(string funName)
    //////{
    //////    //return  this.luaTable.Get<Action>(funName);
    //////    //////return ScriptManager.Instance.CallXXXXFunction(funName);

    //////}

    private void OnEnable()
    {
        luaOnEnable?.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (luaStart == null)
        {
            Debug.LogError("luaStart is null");
        }
        luaStart?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        luaUpdate?.Invoke();
    }

    private void OnDestroy()
    {
        luaOnDestroy?.Invoke();
        luaOnDestroy = null;
        luaStart = null;
        luaUpdate = null;
        this.luaTable.Dispose();
    }
}
#endif