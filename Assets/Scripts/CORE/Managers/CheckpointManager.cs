using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : Singleton<CheckpointManager>
{
    protected override bool IsPersistent => true;

    [field: SerializeField] public string ActiveCheckpointID { get; private set; }
    [field: SerializeField] public string ActiveSceneName { get; private set; }

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
    }

    public void RegisterActivation(string checkpointID)
    {
        ActiveCheckpointID = checkpointID;
        ActiveSceneName = SceneManager.GetActiveScene().name;

        if (SessionManager.Instance != null)
            SessionManager.Instance.CurrentCheckpointID = checkpointID;

        Debug.Log($"[CheckpointManager] Active checkpoint set to '{checkpointID}' in scene '{ActiveSceneName}'");
    }

    public bool IsCheckpointActive(string checkpointID)
        => checkpointID == ActiveCheckpointID;

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

        Debug.LogWarning($"[CheckpointManager] Could not find active checkpoint '{ActiveCheckpointID}' in this scene.");
        return Vector2.zero;
    }
}
