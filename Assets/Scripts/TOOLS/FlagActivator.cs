using UnityEngine;

public class FlagActivator : MonoBehaviour
{
    [Header("Activation Settings")]
    [Tooltip("The ID of the flag that will enable the target GameObject.")]
    [SerializeField] private string targetFlagID;

    [Tooltip("The GameObject to activate. (Keep THIS script on an active GameObject!)")]
    [SerializeField] private GameObject objectToActivate;

    private void Start()
    {
        if (objectToActivate == null) return;

        // Check if the flag was already unlocked before this object loaded
        if (SessionManager.Instance != null && SessionManager.Instance.UnlockedFlags.Contains(targetFlagID))
        {
            objectToActivate.SetActive(true);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnFlagUnlocked += HandleFlagUnlocked;
    }

    private void OnDisable()
    {
        GameEvents.OnFlagUnlocked -= HandleFlagUnlocked;
    }

    private void HandleFlagUnlocked(string flagID)
    {
        // If the unlocked flag matches our target, enable the assigned GameObject
        if (flagID == targetFlagID && objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
    }
}