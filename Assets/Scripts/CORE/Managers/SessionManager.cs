using UnityEngine;

public class SessionManager : Singleton<SessionManager>
{
    public PlayerStatsSO RuntimeStats { get; private set; }

    public void StartSession(PlayerStatsSO baseStats)
    {
        EndSession();
        if (baseStats == null)
        {
            Debug.LogError("[SessionManager] Base PlayerStatsSO is null! Cannot start session.");
            return;
        }

        RuntimeStats = baseStats.Clone();
        Debug.Log("[SessionManager] New runtime PlayerStats clone created.");
    }

    public void EndSession()
    {
        if (RuntimeStats != null)
        {
            RuntimeStats = null;
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            Debug.Log("[SessionManager] Runtime PlayerStats cleared and memory released.");
        }
    }

    public bool HasActiveSession => RuntimeStats != null;
}
