using UnityEngine;

public class SessionManager : Singleton<SessionManager>
{
    protected override bool IsPersistent => false;
    [field: SerializeField]
    public PlayerStatsSO RuntimeStats { get; private set; }
    [field: SerializeField]
    public string CurrentCheckpointID { get; set; }
    public int CurrentStoryState { get; set; }
    public System.Collections.Generic.HashSet<string> UnlockedFlags { get; set; } = new();
    public static bool IsLoadingFromSave { get; set; }

    #region Session Functions
    public void StartSession(PlayerStatsSO baseStats)
    {
        EndSession();

        if (baseStats == null)
        {
            Debug.LogError("[SessionManager] Base PlayerStatsSO is null! Cannot start session.");
            return;
        }

        RuntimeStats = baseStats.Clone();

        CurrentStoryState = 0;
        UnlockedFlags = new System.Collections.Generic.HashSet<string>();
    }

    public void EndSession()
    {
        if (RuntimeStats != null)
        {
            RuntimeStats = null;

            CurrentStoryState = 0;
            UnlockedFlags?.Clear();

            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }
    #endregion

    #region Game Events Functions
    public void UnlockProgressFlag(string flagID)
    {
        if (UnlockedFlags == null) return;

        bool wasAdded = UnlockedFlags.Add(flagID);
        Debug.Log($"[SessionManager] Attempting to add {flagID}. Was it newly added? {wasAdded}");

        if (wasAdded)
        {
            GameEvents.TriggerFlagUnlocked(flagID);
        }
    }

    public void SetStoryState(int newState)
    {
        if (CurrentStoryState == newState) return;

        CurrentStoryState = newState;
        GameEvents.TriggerStoryStateChanged(newState);
    }
    #endregion

    public bool HasActiveSession => RuntimeStats != null;
}