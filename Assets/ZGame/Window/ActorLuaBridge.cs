using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if XLua
using XLua;

public class ActorLuaBridge : MonoBehaviour
{ 
    Action<LuaTable, float> onUpdateFunc;
    Action<LuaTable> onFixedUpdateFunc;
    

    LuaTable table;

    public void BridgeInit(LuaTable tb)
    {
        this.table = tb;
        onUpdateFunc = table.Get<Action<LuaTable, float>>("Update");
        onFixedUpdateFunc = table.Get<Action<LuaTable>>("FixedUpdate");
         
    }


    private void Update()
    {
        onUpdateFunc?.Invoke(table, Time.deltaTime);
    }


    private void FixedUpdate()
    {
        onFixedUpdateFunc?.Invoke(table);
    }

    private void OnDestroy()
    {
         

        if (table != null)
        {
            table.Dispose();
            table = null;
        }
    }
}
#endif