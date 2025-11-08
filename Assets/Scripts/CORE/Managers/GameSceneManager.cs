using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum SceneLoadMode
{
    DoorTransition,
    CheckpointSpawn
}

public class GameSceneManager : MonoBehaviour
{
    private SceneDoorManager sceneDoorManager;

    [SerializeField] private string targetDoorID;
    [SerializeField] private SceneLoadMode currentLoadMode;
    [SerializeField] private bool isLoadingScene;

    [Header("References")]
    [SerializeField] private CanvasManager canvasManager;
    [SerializeField] private SaveSlotManager saveSlotManager;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadSceneFromDoor(SceneField scene, string targetDoor)
    {
        if (isLoadingScene) return;
        StartCoroutine(LoadSceneRoutine(scene, SceneLoadMode.DoorTransition, targetDoor));
    }

    public void LoadSceneFromCheckpoint(SceneField scene)
    {
        if (isLoadingScene) return;
        StartCoroutine(LoadSceneRoutine(scene, SceneLoadMode.CheckpointSpawn, null));
    }

    private IEnumerator LoadSceneRoutine(SceneField scene, SceneLoadMode mode, string doorID)
    {
        isLoadingScene = true;
        currentLoadMode = mode;
        targetDoorID = doorID;

        if (canvasManager == null)
            canvasManager = FindFirstObjectByType<CanvasManager>();

        if (canvasManager != null)
            canvasManager.FadeIn(PanelType.BlackScreen);

        yield return new WaitForSeconds(canvasManager.GetFadeDuration(PanelType.BlackScreen));

        SceneManager.LoadScene(scene);

        isLoadingScene = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(PostSceneLoadRoutine());
    }

    private IEnumerator PostSceneLoadRoutine()
    {
        yield return new WaitForEndOfFrame();

        FindSceneDoorManager();

        if (saveSlotManager != null)
        {
            int slot = saveSlotManager.ActiveSlotIndex;
            if (SaveSystem.SaveExists(slot))
            {
                var runtimeStats = FindFirstObjectByType<PlayerStatsSO>();
                if (runtimeStats != null)
                {
                    SaveSystem.Load(slot, runtimeStats);
                    Debug.Log($"[GameSceneManager] Player stats restored for slot {slot}");
                }
            }
        }

        if (currentLoadMode == SceneLoadMode.DoorTransition)
        {
            if (sceneDoorManager != null && !string.IsNullOrEmpty(targetDoorID))
            {
                sceneDoorManager.ChooseDoor(targetDoorID);
            }
        }
        else if (currentLoadMode == SceneLoadMode.CheckpointSpawn)
        {
            var spawn = GameObject.FindGameObjectWithTag("Checkpoint");
            if (spawn != null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    player.transform.position = spawn.transform.position;

                Debug.Log("[GameSceneManager] Spawned player at checkpoint");
            }
            else
            {
                Debug.LogWarning("[GameSceneManager] No checkpoint found, using default spawn position");
            }
        }

        if (canvasManager != null)
        {
            canvasManager.FadeOut(PanelType.BlackScreen);
            yield return new WaitForSeconds(canvasManager.GetFadeDuration(PanelType.BlackScreen));
        }
    }

    private void FindSceneDoorManager()
    {
        GameObject sceneDoorObj = GameObject.FindGameObjectWithTag("Scene Door Manager");
        if (sceneDoorObj != null)
            sceneDoorManager = sceneDoorObj.GetComponent<SceneDoorManager>();
        else
            sceneDoorManager = null;
    }
}
