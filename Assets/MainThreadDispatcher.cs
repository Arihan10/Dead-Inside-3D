using System;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _mainThreadActions = new();
    
    public static MainThreadDispatcher inst { get; private set; }

    public void Awake()
    {
        if (inst)
        {
            Destroy(gameObject);
        }
        else
        {
            inst = this;
        }
    }

    public static void RunOnMainThread(Action action)
    {
        lock (_mainThreadActions)
        {
            _mainThreadActions.Enqueue(action);
        }
    }

    void Update()
    {
        lock (_mainThreadActions)
        {
            while (_mainThreadActions.Count > 0)
            {
                _mainThreadActions.Dequeue()?.Invoke();
            }
        }
    }
}