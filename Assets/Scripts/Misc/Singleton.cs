using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance { get { return instance; } }

    protected virtual void Awake()
    {
        if (instance != null && gameObject != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = (T)this;
            // not perfect, but ok in most cases
            if (gameObject.transform.parent == null)
                DontDestroyOnLoad(gameObject);
            else
                DontDestroyOnLoad(gameObject.transform.parent.gameObject);
        }
    }
}
