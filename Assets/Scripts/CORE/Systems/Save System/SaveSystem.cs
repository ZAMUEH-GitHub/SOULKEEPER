using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    private static readonly string SaveFolder = Path.Combine(Application.persistentDataPath, "Saves");
    private const int MaxSlots = 3;
    private const int CurrentVersion = 1;

    public static async void Save(int slotIndex, PlayerStatsSO runtimeStats, string currentDoorID = "Cathedral_StartDoor")
    {
        if (slotIndex < 1 || slotIndex > MaxSlots)
        {
            Debug.LogError($"[SaveSystem] Invalid slot {slotIndex}");
            return;
        }

        try
        {
            if (!Directory.Exists(SaveFolder))
                Directory.CreateDirectory(SaveFolder);

            float previousPlaytime = 0f;
            string path = GetSlotPath(slotIndex);

            if (File.Exists(path))
            {
                string oldJson = File.ReadAllText(path);
                GameSaveData oldData = JsonUtility.FromJson<GameSaveData>(oldJson);
                if (oldData != null)
                    previousPlaytime = oldData.totalPlaytime;
            }

            float updatedPlaytime = previousPlaytime + Time.realtimeSinceStartup;

            GameSaveData saveData = new GameSaveData
            {
                playerData = new PlayerSaveData(),
                currentScene = SceneManager.GetActiveScene().name,
                lastDoorID = currentDoorID,
                totalPlaytime = updatedPlaytime,
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                version = CurrentVersion
            };

            saveData.playerData.FromRuntime(runtimeStats, slotIndex);

            foreach (var altar in UnityEngine.Object.FindObjectsByType<AltarController>(FindObjectsSortMode.None))
                saveData.altarData.Add(altar.ToSaveData());

            string json = JsonUtility.ToJson(saveData, true);

            using (StreamWriter writer = new StreamWriter(path, false))
                await writer.WriteAsync(json);

            Debug.Log($"[SaveSystem] Slot {slotIndex} saved successfully — Scene: {saveData.currentScene}, Door: {currentDoorID}, Total Playtime: {updatedPlaytime:F2}s");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SaveSystem] Error saving slot {slotIndex}: {ex.Message}");
        }
    }

    public static async void Load(int slotIndex, PlayerStatsSO runtimeStats)
    {
        try
        {
            string path = GetSlotPath(slotIndex);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[SaveSystem] No save found for slot {slotIndex}");
                return;
            }

            string json;
            using (StreamReader reader = new StreamReader(path))
            {
                json = await reader.ReadToEndAsync();
            }

            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
            if (saveData == null)
            {
                Debug.LogError($"[SaveSystem] Failed to parse save data for slot {slotIndex}");
                return;
            }

            if (saveData.version < CurrentVersion)
            {
                Debug.Log($"[SaveSystem] Upgrading slot {slotIndex} from version {saveData.version} to {CurrentVersion}");
                UpgradeSaveData(ref saveData);
            }

            saveData.playerData.ApplyToRuntime(runtimeStats);

            foreach (var altar in UnityEngine.Object.FindObjectsByType<AltarController>(FindObjectsSortMode.None))
            {
                var data = saveData.altarData.Find(a => a.altarID == altar.altarSO.displayName);
                if (data != null)
                    altar.FromSaveData(data);
            }

            Debug.Log($"[SaveSystem] Loaded slot {slotIndex} — Scene: {saveData.currentScene}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SaveSystem] Error loading slot {slotIndex}: {ex.Message}");
        }
    }

    public static bool SaveExists(int slotIndex) => File.Exists(GetSlotPath(slotIndex));

    public static string GetSavedScene(int slotIndex)
    {
        try
        {
            if (!SaveExists(slotIndex)) return null;
            string json = File.ReadAllText(GetSlotPath(slotIndex));
            GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

            if (!SceneExistsInBuild(data.currentScene))
            {
                Debug.LogWarning($"[SaveSystem] Scene '{data.currentScene}' not found in build settings!");
                return null;
            }

            return data.currentScene;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SaveSystem] Error reading scene from slot {slotIndex}: {ex.Message}");
            return null;
        }
    }

    public static string GetSavedDoor(int slotIndex)
    {
        try
        {
            if (!SaveExists(slotIndex)) return null;
            string json = File.ReadAllText(GetSlotPath(slotIndex));
            GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
            return data.lastDoorID;
        }
        catch
        {
            return null;
        }
    }

    private static string GetSlotPath(int slotIndex) =>
        Path.Combine(SaveFolder, $"SaveSlot_{slotIndex}.json");

    private static bool SceneExistsInBuild(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = Path.GetFileNameWithoutExtension(path);
            if (name == sceneName)
                return true;
        }
        return false;
    }

    public static (bool exists, string scene, string timestamp, float playtime) GetSlotMetadata(int slotIndex)
    {
        string path = GetSlotPath(slotIndex);
        if (!File.Exists(path))
            return (false, "", "", 0f);

        try
        {
            string json = File.ReadAllText(path);
            GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
            if (data == null) return (false, "", "", 0f);

            return (true, data.currentScene, data.timestamp, data.totalPlaytime);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SaveSystem] Metadata read failed for slot {slotIndex}: {ex.Message}");
            return (false, "", "", 0f);
        }
    }

    private static void UpgradeSaveData(ref GameSaveData saveData)
    {
        if (saveData.version < 2)
        {
            saveData.totalPlaytime = 0f;
            saveData.version = 2;
        }
    }
}
