using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton : MonoBehaviour
{
    private static bool hasBeenMovedToPersistentScene = false;

    private void Awake()
    {
        var existing = FindFirstObjectByType<Singleton>();
        if (existing != null && existing != this)
        {
            Debug.Log("[Singleton] Duplicate SINGLETONS prefab detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Scene currentScene = gameObject.scene;
        if (currentScene.name == "DontDestroyOnLoad")
        {
            Debug.Log("[Singleton] Already in persistent scene. Skipping DontDestroyOnLoad().");
            hasBeenMovedToPersistentScene = true;
            return;
        }

        if (!hasBeenMovedToPersistentScene)
        {
            try
            {
                DontDestroyOnLoad(gameObject);
                hasBeenMovedToPersistentScene = true;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[Singleton] Failed to mark persistent: {ex.Message}");
            }
        }
        else
        {
            Debug.Log("[Singleton] Duplicate SINGLETONS prefab detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }
}
