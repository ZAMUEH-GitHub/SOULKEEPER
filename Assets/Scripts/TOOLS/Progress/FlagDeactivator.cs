using UnityEngine;

public class FlagDeactivator : MonoBehaviour
{
    [Header("Flag Requirements")]
    [Tooltip("The ID of the flag that will disable this GameObject.")]
    [SerializeField] private string targetFlagID;

    private void Start()
    {
        // Check if the flag was already unlocked before this object loaded
        if (SessionManager.Instance != null && SessionManager.Instance.UnlockedFlags.Contains(targetFlagID))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        // Start listening to the event
        GameEvents.OnFlagUnlocked += HandleFlagUnlocked;
    }

    private void OnDisable()
    {
        // Stop listening to prevent memory leaks
        GameEvents.OnFlagUnlocked -= HandleFlagUnlocked;
    }

    private void HandleFlagUnlocked(string flagID)
    {
        // If the unlocked flag matches our target, disable this GameObject
        if (flagID == targetFlagID)
        {
            gameObject.SetActive(false);
        }
    }
}