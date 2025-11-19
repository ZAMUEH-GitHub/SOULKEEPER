using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
    private static bool _isShuttingDown;
    private static readonly object _lock = new object();

    protected virtual bool IsPersistent => true;

    public static T Instance
    {
        get
        {
            if (_isShuttingDown)
            {
                Debug.LogWarning($"[Singleton<{typeof(T).Name}>] Instance already destroyed — returning null.");
                return null;
            }

            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                    return null;
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        lock (_lock)
        {
            if (_instance == null)
            {
                _instance = this as T;

                if (IsPersistent)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    protected virtual void OnApplicationQuit()
    {
        _isShuttingDown = true;
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
            _isShuttingDown = true;
    }
}
