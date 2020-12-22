using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if XLua
using XLua;
#endif
using ZGame.Window;

public class LuaBridgeWindow : Window
{

    Action<string, GameObject> onInitFunc;
    Action<string, object> onShowFunc;
    Action onHideFunc;
    Action onDestroyFunc;
    Action onUpdateFunc;
    Action onAddUIEventListenerFunc;
    Action onRemoveUIEventListenerFunc;

    Action<int, object> onHandMsgFunc;
#if XLua
    LuaTable table;
#endif

    public LuaBridgeWindow(GameObject obj, string windowName) : base(obj, windowName)
    {

    }

    public override void AddUIEventListener()
    {
        base.AddUIEventListener();
        this.onAddUIEventListenerFunc?.Invoke();
    }

    public override void Destroy()
    {
        this.onDestroyFunc?.Invoke();
        base.Destroy();
    }

    public override void DoShowAniamtion()
    {
        base.DoShowAniamtion();
    }

    public override void HandleMessage(int msgId, params object[] paras)
    {
        base.HandleMessage(msgId, paras);
        this.onHandMsgFunc?.Invoke(msgId, paras);
    }

    public override void Hide()
    {
        this.onHideFunc?.Invoke();
        base.Hide();

    }

    public override void Init(string windowName, GameObject obj)
    {
        base.Init(windowName, obj);

#if XLua
        table = ScriptManager.Instance.CallLuaNewWindowFunc(windowName);
        if (table == null)
        {
            Debug.LogError("lua   窗口返回错误：" + windowName);
            return;
        }


        onInitFunc = table.Get<Action<string, GameObject>>("Init");
        onShowFunc = table.Get<Action<string, object>>("Show");
        onHideFunc = table.Get<Action>("Hide");
        onDestroyFunc = table.Get<Action>("Destroy");
        onUpdateFunc = table.Get<Action>("Update");
        onHandMsgFunc = table.Get<Action<int, object>>("HandleMessage");
        onAddUIEventListenerFunc = table.Get<Action>("AddUIEventListener");
        onRemoveUIEventListenerFunc = table.Get<Action>("RemoveUIEventListener");

        this.onInitFunc?.Invoke(windowName, obj);
#endif
    }

    public override void RemoveUIEventListener()
    {
        base.RemoveUIEventListener();
        this.onRemoveUIEventListenerFunc?.Invoke();
    }

    public override void Show(string layerName, params object[] datas)
    {
        base.Show(layerName, datas);
        this.onShowFunc?.Invoke(layerName, datas);
    }

    public override void Update()
    {
        base.Update();
        this.onUpdateFunc?.Invoke();
    }
}
