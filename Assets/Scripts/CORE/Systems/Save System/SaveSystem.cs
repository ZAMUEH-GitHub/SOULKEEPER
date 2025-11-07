using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private const int MaxSlots = 3;
    private static readonly string SaveFolder = Path.Combine(Application.persistentDataPath, "Saves");

    public static void Save(int slotIndex, PlayerStatsSO runtimeStats)
    {
        if (slotIndex < 1 || slotIndex > MaxSlots)
        {
            Debug.LogError($"SaveSystem: Invalid slot index {slotIndex}. Must be between 1 and {MaxSlots}.");
            return;
        }

        if (!Directory.Exists(SaveFolder))
            Directory.CreateDirectory(SaveFolder);

        PlayerSaveData data = new PlayerSaveData();
        data.FromRuntime(runtimeStats, slotIndex);

        string json = JsonUtility.ToJson(data, true);
        string path = GetSlotPath(slotIndex);

        try
        {
            File.WriteAllText(path, json);
            Debug.Log($"[SaveSystem] Game saved to slot {slotIndex} at {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to save to {path}: {e.Message}");
        }
    }

    public static void Load(int slotIndex, PlayerStatsSO runtimeStats)
    {
        if (slotIndex < 1 || slotIndex > MaxSlots)
        {
            Debug.LogError($"SaveSystem: Invalid slot index {slotIndex}. Must be between 1 and {MaxSlots}.");
            return;
        }

        string path = GetSlotPath(slotIndex);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveSystem] No save file found at slot {slotIndex} ({path})");
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
            data.ApplyToRuntime(runtimeStats);

            Debug.Log($"[SaveSystem] Game loaded from slot {slotIndex}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to load slot {slotIndex}: {e.Message}");
        }
    }

    public static bool SaveExists(int slotIndex)
    {
        return File.Exists(GetSlotPath(slotIndex));
    }

    private static string GetSlotPath(int slotIndex)
    {
        return Path.Combine(SaveFolder, $"SaveSlot_{slotIndex}.json");
    }
}
