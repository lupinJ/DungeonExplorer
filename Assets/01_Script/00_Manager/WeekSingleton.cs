using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeekSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static bool is_quit = false;
    public static T Instance
    {
        get
        {
            if (is_quit)
                return null;

            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            Init();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
  
    }

    protected virtual void Init() { }

    private void OnApplicationQuit()
    {
        is_quit = true;
    }

}
