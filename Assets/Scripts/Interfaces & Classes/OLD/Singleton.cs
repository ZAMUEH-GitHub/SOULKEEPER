using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindFirstObjectByType<T>();
                        if (_instance == null)
                        {
                            Debug.LogError($"[Singleton] An instance of {typeof(T)} is needed in the scene, but none was found.");
                        }
                    }
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isGameplayScene = IsGameplayScene(scene.name);
        OnSceneChanged(isGameplayScene);
    }

    protected virtual void OnSceneChanged(bool isGameplayScene)
    {
        Debug.Log($"[Singleton<{typeof(T)}>] Scene changed. Is gameplay: {isGameplayScene}");
    }

    protected bool IsGameplayScene(string sceneName)
    {
        return sceneName == "Level 1" || sceneName == "Level 2" || sceneName == "Level 3" || sceneName == "BossLevel";
    }
}
