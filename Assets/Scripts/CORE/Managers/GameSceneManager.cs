using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

    private CanvasManager canvasManager;
    private SaveSlotManager saveSlotManager;
    private SceneDoorManager sceneDoorManager;

    #region Unity Lifecycle
    protected override void Awake() => base.Awake();

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    #endregion

    #region Scene Loading
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

    public void LoadSceneDirect(SceneField scene)
    {
        if (isLoadingScene) return;
        StartCoroutine(LoadSceneRoutine(scene, SceneLoadMode.DirectLoad, null));
    }

    private IEnumerator LoadSceneRoutine(SceneField scene, SceneLoadMode mode, string doorID)
    {
        isLoadingScene = true;
        currentLoadMode = mode;
        targetDoorID = doorID;

        canvasManager ??= CanvasManager.Instance;
        if (canvasManager != null)
            canvasManager.FadeIn(PanelType.BlackScreen);

        yield return new WaitForSeconds(canvasManager?.GetFadeDuration(PanelType.BlackScreen) ?? 0.5f);

        SceneManager.LoadScene(scene);
    }
    #endregion

    #region Scene Post-Load
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(PostSceneLoadRoutine());
    }

    private IEnumerator PostSceneLoadRoutine()
    {
        yield return null;

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
                    SaveSystem.Load(slot, runtimeStats);
            }
        }

        yield return PositionPlayerRoot();

        if (canvasManager != null)
        {
            canvasManager.FadeOut(PanelType.BlackScreen);
            yield return new WaitForSeconds(canvasManager.GetFadeDuration(PanelType.BlackScreen));
        }

        var player = PlayerRoot.Instance?.GetComponentInChildren<PlayerController>();
        if (player != null)
            player.UnfreezeAllInputs();

        isLoadingScene = false;
    }

    private IEnumerator PositionPlayerRoot()
    {
        float timeout = 3f, timer = 0f;
        while (PlayerRoot.Instance == null && timer < timeout)
        {
            if (GameManager.Instance != null &&
                GameManager.Instance.CurrentState == GameState.MainMenu)
                yield break;

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        var playerRoot = PlayerRoot.Instance;
        if (playerRoot == null) yield break;

        var player = playerRoot.GetComponentInChildren<PlayerController>();
        if (player == null) yield break;

        switch (currentLoadMode)
        {
            case SceneLoadMode.DoorTransition:
                if (sceneDoorManager != null && !string.IsNullOrEmpty(targetDoorID))
                {
                    try
                    {
                        sceneDoorManager.ChooseDoor(targetDoorID);
                    }
                    catch (MissingReferenceException)
                    {
                        Debug.LogWarning("[GameSceneManager] Door reference missing during transition.");
                    }
                }
                break;

            case SceneLoadMode.CheckpointSpawn:
                var spawn = GameObject.FindGameObjectWithTag("Checkpoint");
                if (spawn != null)
                    player.transform.position = spawn.transform.position;
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
    #endregion
}
