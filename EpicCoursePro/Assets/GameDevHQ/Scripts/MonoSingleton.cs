﻿using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError(
                    $"Singleton instance of type {typeof(T).ToString()} was not created before usage.");
                // TODO(?): Try lazy instantiation to deal with this?
                // Debug.Log(typeof(T).ToString());
                // Debug.Log($"Creating new single of type: {typeof(T).ToString()}.");
                // Instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
            }
            return _instance;
        }
        private set => _instance = value;
    }

    private void Awake()
    {
        _instance = this as T;
    }

    public virtual void Init() {}
}