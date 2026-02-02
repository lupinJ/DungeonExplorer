using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEvent { }

public interface ISubscribble<T> : IEvent
{
    void Subscribe(Action<T> listener);
    void UnSubscribe(Action<T> listener);
}

public abstract class GameAction<T> : ISubscribble<T>
{
    private event Action<T> action;
    public void Subscribe(Action<T> listener) => action += listener;
    public void UnSubscribe(Action<T> listener) => action -= listener;
    public void Invoke(T arg) => action?.Invoke(arg);
  
}

public class EventManager : Singleton<EventManager>
{
    // Action
    Dictionary<Type, IEvent> eventDic;

    protected override void Init()
    {
        eventDic = new Dictionary<Type, IEvent>();
    }
    private void OnDestroy()
    {
        eventDic?.Clear();
    }

    /// <summary>
    /// 새로운 Event를 등록
    /// </summary>
    public void AddEvent<TEvent>(TEvent action) where TEvent : IEvent
    {
        Type type = typeof(TEvent);

        if (!eventDic.ContainsKey(type))
        {
            eventDic.Add(type, action);
        }
    }

    /// <summary>
    /// 등록한 Event를 제거
    /// </summary>
    public void RemoveEvent<TEvent>() where TEvent : IEvent
    {
        Type type = typeof(TEvent);

        if (eventDic.ContainsKey(type))
        {
            eventDic.Remove(type);
        }

    }

    /// <summary>
    /// 등록된 event에 함수를 구독
    /// </summary>
    public void Subscribe<TEvent, TData>(Action<TData> listener) 
        where TEvent : ISubscribble<TData>
    {
        Type type = typeof(TEvent);

        if (eventDic.TryGetValue(type, out IEvent eventBase))
        {
            if (eventBase is ISubscribble<TData> action)
            {
                action.Subscribe(listener);
            }
        }
       
    }

    /// <summary>
    /// 구독했던 함수를 구독해제
    /// </summary>
    public void Unsubscribe<TEvent, TData>(Action<TData> listener)
        where TEvent : ISubscribble<TData>
    {
        Type type = typeof(TEvent);

        if (listener == null)
            return;

        if (eventDic.TryGetValue(type, out IEvent eventBase))
        {
            if (eventBase is ISubscribble<TData> action)
            {
                action.UnSubscribe(listener);
            }
        }
        
    }

}
