using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CheckpointManager : Singleton<CheckpointManager>
{
    protected override bool IsPersistent => false;

    [field: SerializeField] public string ActiveCheckpointID { get; private set; }
    [field: SerializeField] public string ActiveSceneName { get; private set; }

    [Header("Fallback Settings")]
    [SerializeField] private string defaultFallbackScene = "TheCathedralOfTheLost";
    [SerializeField] private string defaultFallbackCheckpoint = "Cathedral_Center";
    [SerializeField] private Vector2 defaultFallbackPosition = new Vector2(0f, 0f);

    private static bool isFallbackActive = false;

    private new void Awake()
    {
        base.Awake();

        if (SessionManager.IsLoadingFromSave && string.IsNullOrEmpty(ActiveCheckpointID))
        {
            ActiveCheckpointID = SaveSystem.LastLoadedCheckpointID ?? string.Empty;
            ActiveSceneName = SceneManager.GetActiveScene().name;

            if (!string.IsNullOrEmpty(ActiveCheckpointID))
                Debug.Log($"[CheckpointManager] Restored checkpoint '{ActiveCheckpointID}' on scene load '{ActiveSceneName}'");
        }

        if (string.IsNullOrEmpty(ActiveCheckpointID))
        {
            ActiveCheckpointID = "__NONE__";
            Debug.LogWarning("[CheckpointManager] No active checkpoint found. System initialized in idle mode and ready for activation.");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += TryRestoreCheckpointOnSceneLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= TryRestoreCheckpointOnSceneLoad;
    }

    private void Start()
    {
        StartCoroutine(DelayedCheckpointRestore());
    }

    private IEnumerator DelayedCheckpointRestore()
    {
        yield return new WaitForSeconds(0.5f);

        if ((string.IsNullOrEmpty(ActiveCheckpointID) || ActiveCheckpointID == "__NONE__") && !string.IsNullOrEmpty(SaveSystem.LastLoadedCheckpointID))
        {
            ActiveCheckpointID = SaveSystem.LastLoadedCheckpointID;
            ActiveSceneName = SceneManager.GetActiveScene().name;
            Debug.Log($"[CheckpointManager] Delayed restoration of checkpoint '{ActiveCheckpointID}' in scene '{ActiveSceneName}'.");

            var scm = Object.FindFirstObjectByType<SceneCheckpointManager>();
            scm?.RefreshActiveCheckpoint();
        }
    }

    private void TryRestoreCheckpointOnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (string.IsNullOrEmpty(ActiveCheckpointID) || ActiveCheckpointID == "__NONE__")
        {
            if (!string.IsNullOrEmpty(SaveSystem.LastLoadedCheckpointID))
            {
                ActiveCheckpointID = SaveSystem.LastLoadedCheckpointID;
                ActiveSceneName = scene.name;
                Debug.Log($"[CheckpointManager] Late restoration of checkpoint '{ActiveCheckpointID}' in scene '{scene.name}'.");
            }
        }
    }

    public void RegisterActivation(string checkpointID)
    {
        if (string.IsNullOrEmpty(checkpointID) || checkpointID == "__NONE__")
        {
            Debug.LogWarning("[CheckpointManager] Attempted to register empty or placeholder checkpoint ID. Ignoring.");
            return;
        }

        ActiveCheckpointID = checkpointID;
        ActiveSceneName = SceneManager.GetActiveScene().name;

        if (SessionManager.Instance != null)
            SessionManager.Instance.CurrentCheckpointID = checkpointID;

        Debug.Log($"[CheckpointManager] Activated checkpoint '{checkpointID}' in scene '{ActiveSceneName}'.");
    }

    public bool IsCheckpointActive(string checkpointID)
        => !string.IsNullOrEmpty(checkpointID) && checkpointID == ActiveCheckpointID;

    public bool IsSceneCurrent(string sceneName)
        => sceneName == ActiveSceneName;

    public Vector2 GetSpawnPosition()
    {
        if (IsSceneCurrent(SceneManager.GetActiveScene().name))
        {
            var scm = Object.FindFirstObjectByType<SceneCheckpointManager>();
            var cp = scm?.GetCheckpoint(ActiveCheckpointID);
            if (cp != null)
                return cp.transform.position;
        }

        if (SaveSystem.LastLoadedPlayerPosition != Vector2.zero || SaveSystem.HasValidPlayerPosition)
        {
            Debug.LogWarning($"[CheckpointManager] Fallback to last saved player position {SaveSystem.LastLoadedPlayerPosition}.");
            return SaveSystem.LastLoadedPlayerPosition;
        }

        if (!isFallbackActive)
        {
            isFallbackActive = true;
            Debug.LogWarning($"[CheckpointManager] Could not find active checkpoint '{ActiveCheckpointID}' in this scene. Performing direct safe zone load.");

            var sceneManager = Object.FindFirstObjectByType<GameSceneManager>();
            if (sceneManager != null)
            {
                Debug.Log($"[CheckpointManager] Executing DirectLoad to '{defaultFallbackScene}' at '{defaultFallbackCheckpoint}'.");
                sceneManager.LoadSceneDirect(defaultFallbackScene, defaultFallbackPosition);
                isFallbackActive = false;
                return defaultFallbackPosition;
            }

            isFallbackActive = false;
        }

        var fallbackSCM = Object.FindFirstObjectByType<SceneCheckpointManager>();
        var fallbackCP = fallbackSCM?.GetCheckpoint(defaultFallbackCheckpoint);
        if (fallbackCP != null)
        {
            Debug.Log($"[CheckpointManager] Fallback checkpoint '{defaultFallbackCheckpoint}' found in safe zone.");
            return fallbackCP.transform.position;
        }

        Debug.LogWarning("[CheckpointManager] Default fallback checkpoint not found. Spawning at default manual position.");
        return defaultFallbackPosition;
    }
}