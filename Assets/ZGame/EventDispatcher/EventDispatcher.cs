using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using XLua;

public delegate void EventAction(int evtId, params object[] paras);

public class EventDispatcher : Singleton<EventDispatcher>
{
    //c#事件
    private Dictionary<int, EventAction> mEventHandlers;
    private Dictionary<int, EventAction> mEventOnceHandlers;



    public EventDispatcher()
    {
        init();
    }

    void init()
    {
        mEventHandlers = new Dictionary<int, EventAction>();
        mEventOnceHandlers = new Dictionary<int, EventAction>();
    }

    internal void AddListener(object onFuPiaoDropWater)
    {
        throw new NotImplementedException();
    }

    public void DispatchEvent(int evtId, params object[] paras)
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



    private bool checkEventId(int evtId)
    {
        if (!mEventHandlers.ContainsKey(evtId) &&
            !mEventOnceHandlers.ContainsKey(evtId))
        {
            Debug.LogWarning("evtId:" + evtId + ",未注册");
            return false;
        }
        return true;
    }

    EventAction getHandler(Dictionary<int, EventAction> eventDic, int evtId)
    {
        EventAction handler = null;
        eventDic.TryGetValue(evtId, out handler);
        return handler;
    }


    public void AddListener(int evtId, EventAction handler)
    {
        addListener(mEventHandlers, evtId, handler);
    }
    public void AddListenerOnce(int evtId, EventAction handler)
    {
        addListener(mEventOnceHandlers, evtId, handler);
    }



    public void RemoveListener(int evtId, EventAction handler)
    {
        removeListener(mEventHandlers, evtId, handler);
        removeListener(mEventOnceHandlers, evtId, handler);
    }


    private void removeListener(Dictionary<int, EventAction> eventDic, int evtId, EventAction handler)
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



    private void addListener(Dictionary<int, EventAction> eventDic, int evtId, EventAction handler)
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
                Debug.LogError(evtId + "多次添加重复事件");
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
