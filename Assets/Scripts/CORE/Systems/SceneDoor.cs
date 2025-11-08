using UnityEngine;

public class SceneDoor : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public string doorID;
    public SceneField targetScene;
    public string targetDoorID;

    public void Interact()
    {
        var manager = FindFirstObjectByType<SceneDoorManager>();
        if (manager != null)
            manager.RegisterDoorUse(doorID);

        var sceneManager = FindFirstObjectByType<GameSceneManager>();
        if (sceneManager != null)
            sceneManager.LoadSceneFromDoor(targetScene, targetDoorID);
    }
}
