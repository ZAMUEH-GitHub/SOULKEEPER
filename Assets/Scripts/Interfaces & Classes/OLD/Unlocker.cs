using UnityEngine;

public class Unlocker : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] unlockableTargets;

    private IUnlockable[] unlockables;

    private void Awake()
    {
        unlockables = new IUnlockable[unlockableTargets.Length];
        for (int i = 0; i < unlockableTargets.Length; i++)
        {
            unlockables[i] = unlockableTargets[i] as IUnlockable;
        }
    }

    public void TriggerUnlock()
    {
        foreach (var u in unlockables)
        {
            u?.Unlock(true);
        }
    }
}
