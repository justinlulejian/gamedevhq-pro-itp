using UnityEngine;

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
                    $"Singleton instance of type {typeof(T).ToString()} was not created " +
                    $"before usage.");
            }
            return _instance;
        }
        private set => _instance = value;
    }

    protected virtual void Awake()
    {
        _instance = this as T;
    }

    public virtual void Init() {}
}