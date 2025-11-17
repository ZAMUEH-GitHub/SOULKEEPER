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

    private CanvasManager canvasManager;
    private SaveSlotManager saveSlotManager;
    private SceneDoorManager sceneDoorManager;

    protected override void Awake() => base.Awake();
    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

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

        string sceneName = SaveSystem.GetSavedScene(slotIndex);
        string checkpointID = SaveSystem.LastLoadedCheckpointID ?? SaveSystem.GetSavedCheckpoint(slotIndex);

        if (string.IsNullOrEmpty(sceneName)) return;

        SceneField targetScene = sceneName;
        StartCoroutine(LoadSceneRoutine(targetScene, SceneLoadMode.CheckpointSpawn, null, checkpointID));
    }

    public void LoadSceneDirect(SceneField scene)
    {
        if (isLoadingScene) return;
        StartCoroutine(LoadSceneRoutine(scene, SceneLoadMode.DirectLoad, null, null));
    }

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
            }
        }

        if (SessionManager.Instance != null && !string.IsNullOrEmpty(SessionManager.Instance.CurrentCheckpointID))
            Checkpoint.BroadcastActivation(SessionManager.Instance.CurrentCheckpointID);

        await PositionPlayerRootAsync();

        if (canvasManager != null)
        {
            canvasManager.FadeOut(PanelType.BlackScreen);
            await Task.Delay((int)(canvasManager.GetFadeDuration(PanelType.BlackScreen) * 1000));
        }

        var player = PlayerRoot.Instance?.GetComponentInChildren<PlayerController>();
        if (player != null)
            player.UnfreezeAllInputs();

        isLoadingScene = false;
    }

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
                    { Debug.LogWarning("[GameSceneManager] Door reference missing during transition."); }
                }
                break;

            case SceneLoadMode.CheckpointSpawn:
                if (!string.IsNullOrEmpty(targetCheckpointID))
                {
                    var checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
                    foreach (var cp in checkpoints)
                    {
                        var checkpoint = cp.GetComponent<Checkpoint>();
                        if (checkpoint != null && checkpoint.checkpointID == targetCheckpointID)
                        {
                            player.transform.position = checkpoint.transform.position;
                            break;
                        }
                    }
                }
                break;

            case SceneLoadMode.DirectLoad:
                break;
        }

        player.FreezeAllInputs();
    }

    private void FindSceneDoorManager()
    {
        var sceneDoorObj = GameObject.FindGameObjectWithTag("Scene Door Manager");
        sceneDoorManager = sceneDoorObj ? sceneDoorObj.GetComponent<SceneDoorManager>() : null;
    }
}
