using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEvent
{
    void Register(Delegate pending);
    Delegate GetListener();
}

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

    public void Register(Delegate func)
    {
        if (func is Action<T> combinedAction)
        {
            action -= combinedAction;
            action += combinedAction;
        }
    }

    public Delegate GetListener() => action;

    public void Invoke(T arg) => action?.Invoke(arg);

}

public class EventManager : Singleton<EventManager>
{
    // Action
    Dictionary<Type, IEvent> eventDic = new();
    Dictionary<Type, Delegate> waitDic = new();

    protected override void Init()
    {
        
    }

    private void OnApplicationQuit()
    {
        eventDic?.Clear();
        waitDic?.Clear();
    }

    /// <summary>
    /// 새로운 Event를 등록
    /// </summary>
    public void AddEvent<TEvent>(TEvent action) where TEvent : IEvent
    {
        Type type = typeof(TEvent);

        if (!eventDic.ContainsKey(type))
        {
            // 이벤트 추가
            eventDic.Add(type, action);

            // 대기 중인 구독자가 있다면 연결
            if (waitDic.Remove(type, out var func))
            {
                action.Register(func);
            }
        }
    }

    /// <summary>
    /// 등록한 Event를 제거
    /// </summary>
    public void RemoveEvent<TEvent>() where TEvent : IEvent
    {
        Type type = typeof(TEvent);

        if (eventDic.TryGetValue(type, out IEvent eventBase))
        {
            // 구독자들을 대기열에 넘김
            Delegate existingListeners = eventBase.GetListener();

            if (existingListeners != null)
            {
                waitDic[type] = Delegate.Combine(waitDic.GetValueOrDefault(type), existingListeners);
            }
           
            // 이벤트 제거
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

        // 있으면 구독
        if (eventDic.TryGetValue(type, out IEvent eventBase))
        {
            if (eventBase is ISubscribble<TData> action)
            {
                action.UnSubscribe(listener);
                action.Subscribe(listener);
            }
        }
        else
        {
            // 없으면 대기열에 추가
            var existingWait = waitDic.GetValueOrDefault(type);
            var cleanedWait = Delegate.Remove(existingWait, listener);
            waitDic[type] = Delegate.Combine(cleanedWait, listener);
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

        // event에서 제거
        if (eventDic.TryGetValue(type, out IEvent eventBase))
        {
            if (eventBase is ISubscribble<TData> action)
            {
                action.UnSubscribe(listener);
            }
        }

        // 대기열에서 제거
        if (waitDic.TryGetValue(type, out var func))
        {
            var result = Delegate.Remove(func, listener);
            if (result == null) waitDic.Remove(type);
            else waitDic[type] = result;
        }

    }

}
