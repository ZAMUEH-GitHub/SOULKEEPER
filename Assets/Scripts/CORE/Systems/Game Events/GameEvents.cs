using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<int> OnStoryStateChanged;
    public static event Action<string> OnFlagUnlocked;
    public static event Action<PowerUpDefinition> OnPowerUpUnlocked;

    // --- You can add other global events here later ---
    // public static event Action OnPlayerDied;
    // public static event Action<int> OnPlayerHealthChanged;

    public static void TriggerStoryStateChanged(int newState)
    {
        OnStoryStateChanged?.Invoke(newState);
        Debug.Log($"[GameEvents] Story State changed to: {newState}");
    }

    public static void TriggerFlagUnlocked(string flagID)
    {
        OnFlagUnlocked?.Invoke(flagID);
        Debug.Log($"[GameEvents] Flag unlocked: {flagID}");
    }

    public static void TriggerPowerUpUnlocked(PowerUpDefinition def)
    {
        OnPowerUpUnlocked?.Invoke(def);
        Debug.Log($"[GameEvents] Power-up unlocked: {def.displayName}");
    }
}