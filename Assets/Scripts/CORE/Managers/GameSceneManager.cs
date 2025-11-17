using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading.Tasks;

public enum SceneLoadMode
{
    DoorTransition,
    CheckpointSpawn,
    DirectLoad
}

public class GameSceneManager : Singleton<GameSceneManager>
{
    protected override bool IsPersistent => false;

    [Header("Scene Management")]
    [SerializeField] private SceneLoadMode currentLoadMode;
    [SerializeField] private bool isLoadingScene;
    [SerializeField] private string targetDoorID;
    [SerializeField] private string targetCheckpointID;
    [SerializeField] private Vector2 directSpawnPosition;

    private CanvasManager canvasManager;
    private SaveSlotManager saveSlotManager;
    private SceneDoorManager sceneDoorManager;

    #region Unity Lifecycle
    protected override void Awake() => base.Awake();
    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    #endregion

    #region Load Scene API
    public void LoadSceneFromDoor(SceneField scene, string targetDoor)
    {
        if (isLoadingScene) return;
        StartCoroutine(LoadSceneRoutine(scene, SceneLoadMode.DoorTransition, targetDoor, null));
    }

    public void LoadSceneFromCheckpoint(string checkpointID)
    {
        if (isLoadingScene) return;
        StartCoroutine(LoadSceneRoutine(null, SceneLoadMode.CheckpointSpawn, null, checkpointID));
    }

    public void LoadSceneFromCheckpointSlot(int slotIndex)
    {
        if (isLoadingScene) return;
        SessionManager.IsLoadingFromSave = true;

        string sceneName = SaveSystem.GetSavedScene(slotIndex);
        string checkpointID = SaveSystem.LastLoadedCheckpointID ?? SaveSystem.GetSavedCheckpoint(slotIndex);

        if (string.IsNullOrEmpty(sceneName)) return;

        SceneField targetScene = sceneName;
        StartCoroutine(LoadSceneRoutine(targetScene, SceneLoadMode.CheckpointSpawn, null, checkpointID));
    }

    public void LoadSceneDirect(SceneField scene, Vector2 spawnPosition)
    {
        if (isLoadingScene) return;

        directSpawnPosition = spawnPosition;
        StartCoroutine(LoadSceneRoutine(scene, SceneLoadMode.DirectLoad, null, null));
    }
    #endregion

    #region Scene Loading Logic
    private IEnumerator LoadSceneRoutine(SceneField scene, SceneLoadMode mode, string doorID, string checkpointID)
    {
        isLoadingScene = true;
        currentLoadMode = mode;
        targetDoorID = doorID;
        targetCheckpointID = checkpointID;

        canvasManager ??= CanvasManager.Instance;
        if (canvasManager != null)
            canvasManager.FadeIn(PanelType.BlackScreen);

        yield return new WaitForSeconds(canvasManager?.GetFadeDuration(PanelType.BlackScreen) ?? 0.5f);

        SceneManager.LoadScene(scene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _ = PostSceneLoadRoutineAsync();
    }

    private async Task PostSceneLoadRoutineAsync()
    {
        await Task.Yield();

        FindSceneDoorManager();
        canvasManager = CanvasManager.Instance;
        saveSlotManager = SaveSlotManager.Instance;

        string loadedCheckpointID = null;

        if (saveSlotManager != null)
        {
            int slot = saveSlotManager.ActiveSlotIndex;
            if (SaveSystem.SaveExists(slot))
            {
                var runtimeStats = SessionManager.Instance.RuntimeStats;
                if (runtimeStats != null)
                    await SaveSystem.LoadAsync(slot, runtimeStats);
            }
        }

        if (SessionManager.Instance != null && !string.IsNullOrEmpty(SessionManager.Instance.CurrentCheckpointID))
            loadedCheckpointID = SessionManager.Instance.CurrentCheckpointID;

        if (!string.IsNullOrEmpty(loadedCheckpointID))
            targetCheckpointID = loadedCheckpointID;

        await PositionPlayerRootAsync();

        if (canvasManager != null)
        {
            canvasManager.FadeOut(PanelType.BlackScreen);
            await Task.Delay((int)(canvasManager.GetFadeDuration(PanelType.BlackScreen) * 1000));
        }

        var player = PlayerRoot.Instance?.GetComponentInChildren<PlayerController>();
        if (player != null)
            player.UnfreezeAllInputs();

        if (!string.IsNullOrEmpty(loadedCheckpointID))
        {
            Debug.Log($"[GameSceneManager] Broadcasting checkpoint activation for '{loadedCheckpointID}' after load (final)");
            Checkpoint.BroadcastActivation(loadedCheckpointID);
        }

        isLoadingScene = false;
        SessionManager.IsLoadingFromSave = false;
    }
    #endregion

    #region Player Position & Utilities
    private async Task PositionPlayerRootAsync()
    {
        float timeout = 3f, timer = 0f;
        while (PlayerRoot.Instance == null && timer < timeout)
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.MainMenu)
                return;
            timer += Time.unscaledDeltaTime;
            await Task.Yield();
        }

        var playerRoot = PlayerRoot.Instance;
        if (playerRoot == null) return;

        var player = playerRoot.GetComponentInChildren<PlayerController>();
        if (player == null) return;

        switch (currentLoadMode)
        {
            case SceneLoadMode.DoorTransition:
                if (sceneDoorManager != null && !string.IsNullOrEmpty(targetDoorID))
                {
                    try { sceneDoorManager.ChooseDoor(targetDoorID); }
                    catch (MissingReferenceException)
                    {
                        Debug.LogWarning("[GameSceneManager] Door reference missing during transition.");
                    }
                }
                break;

            case SceneLoadMode.CheckpointSpawn:
                if (!string.IsNullOrEmpty(targetCheckpointID))
                {
                    var checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
                    bool found = false;

                    foreach (var cp in checkpoints)
                    {
                        var checkpoint = cp.GetComponent<Checkpoint>();
                        if (checkpoint != null && checkpoint.checkpointID == targetCheckpointID)
                        {
                            player.transform.position = checkpoint.transform.position;
                            found = true;
                            Debug.Log($"[GameSceneManager] Loaded from checkpoint '{targetCheckpointID}' at {checkpoint.transform.position}");
                            break;
                        }
                    }

                    if (!found)
                    {
                        Debug.LogWarning($"[GameSceneManager] Checkpoint '{targetCheckpointID}' not found in scene '{SceneManager.GetActiveScene().name}'. Spawning at (0,0).");
                        player.transform.position = Vector2.zero;
                    }
                }
                else
                {
                    Debug.LogWarning("[GameSceneManager] No checkpoint ID found in save data. Spawning at (0,0).");
                    player.transform.position = Vector2.zero;
                }
                break;

            case SceneLoadMode.DirectLoad:
                player.transform.position = directSpawnPosition;
                Debug.Log($"[GameSceneManager] DirectLoad: Player spawned at {directSpawnPosition}");
                break;
        }

        player.FreezeAllInputs();
    }

    private void FindSceneDoorManager()
    {
        var sceneDoorObj = GameObject.FindGameObjectWithTag("Scene Door Manager");
        sceneDoorManager = sceneDoorObj ? sceneDoorObj.GetComponent<SceneDoorManager>() : null;
    }
    #endregion
}
