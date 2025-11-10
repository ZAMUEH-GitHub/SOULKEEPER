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
    private SceneDoorManager sceneDoorManager;

    [SerializeField] private string targetDoorID;
    [SerializeField] private SceneLoadMode currentLoadMode;
    [SerializeField] private bool isLoadingScene;

    [Header("References")]
    [SerializeField] private CanvasManager canvasManager;
    [SerializeField] private SaveSlotManager saveSlotManager;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

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

        isLoadingScene = false;
    }
    #endregion

    #region Scene Post-Load
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(PostSceneLoadRoutine());
    }

    private IEnumerator PostSceneLoadRoutine()
    {
        yield return new WaitForEndOfFrame();

        FindSceneDoorManager();

        saveSlotManager ??= SaveSlotManager.Instance;

        if (saveSlotManager != null)
        {
            int slot = saveSlotManager.ActiveSlotIndex;
            if (SaveSystem.SaveExists(slot))
            {
                var runtimeStats = SessionManager.Instance.RuntimeStats;
                if (runtimeStats != null)
                {
                    SaveSystem.Load(slot, runtimeStats);
                    Debug.Log($"[GameSceneManager] Player stats restored for slot {slot}");
                }
            }
        }

        switch (currentLoadMode)
        {
            case SceneLoadMode.DoorTransition:
                if (sceneDoorManager != null && !string.IsNullOrEmpty(targetDoorID))
                    sceneDoorManager.ChooseDoor(targetDoorID);
                break;

            case SceneLoadMode.CheckpointSpawn:
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
                break;

            case SceneLoadMode.DirectLoad:
                Debug.Log("[GameSceneManager] Direct scene load (no spawn positioning).");
                break;
        }

        if (canvasManager != null)
        {
            canvasManager.FadeOut(PanelType.BlackScreen);
            yield return new WaitForSeconds(canvasManager.GetFadeDuration(PanelType.BlackScreen));
        }
    }
    #endregion

    private void FindSceneDoorManager()
    {
        GameObject sceneDoorObj = GameObject.FindGameObjectWithTag("Scene Door Manager");
        if (sceneDoorObj != null)
            sceneDoorManager = sceneDoorObj.GetComponent<SceneDoorManager>();
        else
            sceneDoorManager = null;
    }
}
