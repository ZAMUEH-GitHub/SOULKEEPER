/*using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneServiceManagerOLD : MonoBehaviour
{
    [Header("Assign all gameplay singleton GameObjects here")]
    public GameObject[] gameplaySingletons;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main Menu")
        {
            DisableGameplaySingletons();
        }
        else
        {
            EnableGameplaySingletons();
        }
    }

    private void DisableGameplaySingletons()
    {
        foreach (var go in gameplaySingletons)
        {
            if (go != null)
                go.SetActive(false);
        }
    }

    private void EnableGameplaySingletons()
    {
        foreach (var go in gameplaySingletons)
        {
            if (go != null)
                go.SetActive(true);
        }
    }
}*/
