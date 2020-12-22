using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using XLua;

public delegate void EventAction(string evtId, params object[] paras);

public class EventDispatcher : Singleton<EventDispatcher>
{
    //c#事件
    private Dictionary<string, EventAction> mEventHandlers;
    private Dictionary<string, EventAction> mEventOnceHandlers;



    public EventDispatcher()
    {
        init();
    }

    void init()
    {
        mEventHandlers = new Dictionary<string, EventAction>();
        mEventOnceHandlers = new Dictionary<string, EventAction>();
    }

   

    public void DispatchEvent(string evtId, params object[] paras)
    {
        if (checkEventId(evtId) == false)
        {
            return;
        }



        EventAction handler = getHandler(mEventHandlers, evtId);
        if (handler != null)
        {
            int count = handler.GetInvocationList().Length;
            //Debug.LogWarning("evtID:" + evtId + "，有" + count + "个delegate");//加上这个log，防止编码者忘记remove delegate
            handler(evtId, paras);
        }
        handler = getHandler(mEventOnceHandlers, evtId);
        if (handler != null)
        {
            removeListener(mEventOnceHandlers, evtId, handler);
            handler(evtId, paras);
        }
    }



    private bool checkEventId(string evtId)
    {
        if (!mEventHandlers.ContainsKey(evtId) &&
            !mEventOnceHandlers.ContainsKey(evtId))
        {
            Debug.LogWarning("evtId:" + evtId + ",未注册");
            return false;
        }
        return true;
    }

    EventAction getHandler(Dictionary<string, EventAction> eventDic, string evtId)
    {
        EventAction handler = null;
        eventDic.TryGetValue(evtId, out handler);
        return handler;
    }


    public void AddListener(string evtId, EventAction handler)
    {
        addListener(mEventHandlers, evtId, handler);
    }
    public void AddListenerOnce(string evtId, EventAction handler)
    {
        addListener(mEventOnceHandlers, evtId, handler);
    }



    public void RemoveListener(string evtId, EventAction handler)
    {
        removeListener(mEventHandlers, evtId, handler);
        removeListener(mEventOnceHandlers, evtId, handler);
    }


    private void removeListener(Dictionary<string, EventAction> eventDic, string evtId, EventAction handler)
    {
        EventAction eventAction = getHandler(eventDic, evtId);
        if (eventAction != null)
        {
            if (eventAction.GetInvocationList().Contains(handler))
            {
                eventAction -= handler;
                eventDic[evtId] = eventAction;
            }

            if (eventAction == null || eventAction.GetInvocationList().Length == 0)
            {
                eventDic.Remove(evtId);
            }
        }
    }



    private void addListener(Dictionary<string, EventAction> eventDic, string evtId, EventAction handler)
    {
        EventAction eventAction = getHandler(eventDic, evtId);
        if (eventAction != null)
        {
            if (!eventAction.GetInvocationList().Contains(handler))
            {
                eventAction += handler;
            }
            else
            {
                Debug.LogWarning(evtId + "多次添加重复事件");
            }
        }
        else
        {
            eventAction = handler;
        }
        eventDic[evtId] = eventAction;
    }


    public void ClearAll()
    {
        mEventHandlers.Clear();
        mEventOnceHandlers.Clear();
    }
}
