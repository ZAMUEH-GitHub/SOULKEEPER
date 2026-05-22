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
    [SerializeField] private Vector3 defaultFallbackPosition;

    private static bool isFallbackActive = false;

    #region Unity Lifecycle
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
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += TryRestoreCheckpointOnSceneLoad;
    private void OnDisable() => SceneManager.sceneLoaded -= TryRestoreCheckpointOnSceneLoad;

    private void Start()
    {
        StartCoroutine(DelayedCheckpointRestore());
    }
    #endregion

    #region Checkpoint API
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

        if ((Vector3)SaveSystem.LastLoadedPlayerPosition != Vector3.zero || SaveSystem.HasValidPlayerPosition)
        {
            return SaveSystem.LastLoadedPlayerPosition;
        }

        if (!isFallbackActive)
        {
            isFallbackActive = true;
            var sceneManager = Object.FindFirstObjectByType<GameSceneManager>();
            if (sceneManager != null)
            {
                sceneManager.LoadSceneDirect(defaultFallbackScene, defaultFallbackPosition);
                isFallbackActive = false;
                return defaultFallbackPosition;
            }
            isFallbackActive = false;
        }

        return defaultFallbackPosition;
    }
    #endregion
}