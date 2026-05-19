using UnityEngine;
using UnityEngine.InputSystem;

public class HUDDebugTester : MonoBehaviour
{
    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Ensure we are grabbing the exact same clone the HUD is looking at
        if (PlayerController.Instance == null || PlayerController.Instance.playerRuntimeStats == null)
            return;

        var stats = PlayerController.Instance.playerRuntimeStats;

        // --- HEALTH TESTING ---
        // Press Up Arrow to Heal, Down Arrow to take Damage
        if (keyboard.upArrowKey.wasPressedThisFrame)
        {
            stats.health = Mathf.Min(stats.health + 1, stats.maxHealth);
            Debug.Log($"[HUDDebugTester] Healed! Current Health: {stats.health}");
        }

        if (keyboard.downArrowKey.wasPressedThisFrame)
        {
            stats.health = Mathf.Max(stats.health - 1, 0);
            Debug.Log($"[HUDDebugTester] Damaged! Current Health: {stats.health}");
        }

        // --- POWER-UP TESTING ---
        // Press 1, 2, 3, or 4 to unlock abilities
        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            stats.attackUnlocked = true;
            Debug.Log("[HUDDebugTester] Attack Unlocked");
        }
        if (keyboard.digit2Key.wasPressedThisFrame)
        {
            stats.jumpUnlocked = true;
            Debug.Log("[HUDDebugTester] Double Jump Unlocked");
        }
        if (keyboard.digit3Key.wasPressedThisFrame)
        {
            stats.dashUnlocked = true;
            Debug.Log("[HUDDebugTester] Dash Unlocked");
        }
        if (keyboard.digit4Key.wasPressedThisFrame)
        {
            stats.wallJumpUnlocked = true;
            Debug.Log("[HUDDebugTester] Wall Jump Unlocked");
        }
    }
}