using UnityEngine;

public class SessionManager : Singleton<SessionManager>
{
    protected override bool IsPersistent => false;

    public PlayerStatsSO RuntimeStats { get; private set; }

    public string CurrentCheckpointID { get; set; }

    public static bool IsLoadingFromSave { get; set; }

    public void StartSession(PlayerStatsSO baseStats)
    {
        EndSession();

        if (baseStats == null)
        {
            Debug.LogError("[SessionManager] Base PlayerStatsSO is null! Cannot start session.");
            return;
        }

        RuntimeStats = baseStats.Clone();
    }

    public void EndSession()
    {
        if (RuntimeStats != null)
        {
            RuntimeStats = null;
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }

    public bool HasActiveSession => RuntimeStats != null;
}
