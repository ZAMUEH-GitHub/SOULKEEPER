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

        if (saveSlotManager != null)
        {
            int slot = saveSlotManager.ActiveSlotIndex;
            if (SaveSystem.SaveExists(slot))
            {
                var runtimeStats = SessionManager.Instance.RuntimeStats;
                if (runtimeStats != null)
                    await SaveSystem.LoadAsync(slot, runtimeStats);

                var global = CheckpointManager.Instance;
                if (string.IsNullOrEmpty(global.ActiveCheckpointID) && !string.IsNullOrEmpty(SaveSystem.LastLoadedCheckpointID))
                {
                    global.RegisterActivation(SaveSystem.LastLoadedCheckpointID);
                    Debug.Log($"[GameSceneManager] Synced CheckpointManager with '{SaveSystem.LastLoadedCheckpointID}' after load.");
                }
            }
        }

        await PositionPlayerRootAsync();

        var sceneCheckpointManager = Object.FindFirstObjectByType<SceneCheckpointManager>();
        sceneCheckpointManager?.RefreshActiveCheckpoint();

        if (canvasManager != null)
        {
            canvasManager.FadeOut(PanelType.BlackScreen);
            await Task.Delay((int)(canvasManager.GetFadeDuration(PanelType.BlackScreen) * 1000));
        }

        var playerRoot = PlayerRoot.Instance;
        var player = playerRoot?.GetComponentInChildren<PlayerController>();
        if (player != null)
        {
            var deathController = player.GetComponent<PlayerDeathController>();
            if (deathController != null)
                deathController.ResetAfterRespawn();

            player.UnfreezeAllInputs();
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
                    sceneDoorManager.ChooseDoor(targetDoorID);
                break;

            case SceneLoadMode.CheckpointSpawn:
                var global = CheckpointManager.Instance;
                Vector2 spawnPos = global.GetSpawnPosition();
                player.transform.position = spawnPos;
                Debug.Log($"[GameSceneManager] Spawned player from checkpoint '{global.ActiveCheckpointID}' at {spawnPos}");
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
        var obj = GameObject.FindGameObjectWithTag("Scene Door Manager");
        sceneDoorManager = obj ? obj.GetComponent<SceneDoorManager>() : null;
    }
    #endregion
}
