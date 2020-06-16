using System;
using System.Collections.Generic;

public class EventCenter
{
    //分发事件
    //public event Action<CEvent> update = delegate { };


    //监听列表
    private Dictionary<CEventType, Action<CEvent>> listeners = new Dictionary<CEventType, Action<CEvent>>();

    //private Dictionary<CEventType, Action<CEvent>> listeners0 = new Dictionary<CEventType, Action<CEvent>>();
    /// <summary>
    /// 分发事件
    /// </summary>
    /// <param name="evt"></param>
    public void DispatchEvent(CEvent evt)
    {
        Util.Log("evt.Type=" + evt.Type);
        if (!listeners.ContainsKey(evt.Type))
            return;
        var onEvent = listeners[evt.Type];
        if(onEvent != null)
        {
            onEvent(evt);
        }
        //listeners[evt.Type](evt);
        //update(evt);
    }

    public void addListener(CEventType eventType, Action<CEvent> listener)
    {
        listeners[eventType] += listener;
    }

    public void removeListener(CEventType eventType, Action<CEvent> listener)
    {
        listeners[eventType] -= listener;
    }

    /// <summary>
    /// 实现单例
    /// </summary>
    protected EventCenter() { }

    private static EventCenter mInstance;
    public static EventCenter Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new EventCenter();
            }
            return mInstance;
        }
    }
}