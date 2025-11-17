using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    private static readonly string SaveFolder = Path.Combine(Application.persistentDataPath, "Saves");
    private const int MaxSlots = 3;
    private const int CurrentVersion = 1;

    public static string LastLoadedCheckpointID { get; private set; }

    public static async Task SaveAsync(int slotIndex, PlayerStatsSO runtimeStats, string currentDoorID = null, string currentCheckpointID = null)
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

            string path = GetSlotPath(slotIndex);
            float previousPlaytime = 0f;

            if (File.Exists(path))
            {
                try
                {
                    string oldJson = await File.ReadAllTextAsync(path);
                    GameSaveData oldData = JsonUtility.FromJson<GameSaveData>(oldJson);
                    if (oldData != null)
                        previousPlaytime = oldData.totalPlaytime;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[SaveSystem] Failed to read old save data: {ex.Message}");
                }
            }

            float updatedPlaytime = previousPlaytime + TimeManager.Instance.GetTotalPlaytime();

            GameSaveData saveData = new GameSaveData
            {
                playerData = new PlayerSaveData(),
                currentSceneID = SceneManager.GetActiveScene().name,
                lastDoorID = currentDoorID,
                currentCheckpointID = currentCheckpointID,
                totalPlaytime = updatedPlaytime,
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                version = CurrentVersion
            };

            saveData.playerData.FromRuntime(runtimeStats, slotIndex);

            foreach (var altar in UnityEngine.Object.FindObjectsByType<AltarController>(FindObjectsSortMode.None))
            {
                if (altar != null)
                    saveData.altarData.Add(altar.ToSaveData());
            }

            string json = JsonUtility.ToJson(saveData, true);

            using (StreamWriter writer = new StreamWriter(path, false))
                await writer.WriteAsync(json);

            TimeManager.Instance.ResetPlaytime();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SaveSystem] Error saving slot {slotIndex}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public static async Task LoadAsync(int slotIndex, PlayerStatsSO runtimeStats)
    {
        try
        {
            string path = GetSlotPath(slotIndex);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[SaveSystem] No save found for slot {slotIndex}");
                return;
            }

            string json = await File.ReadAllTextAsync(path);
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError($"[SaveSystem] Empty save file for slot {slotIndex}");
                return;
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
            LastLoadedCheckpointID = saveData.currentCheckpointID;

            foreach (var altar in UnityEngine.Object.FindObjectsByType<AltarController>(FindObjectsSortMode.None))
            {
                if (altar == null || altar.altarSO == null) continue;
                var data = saveData.altarData.Find(a => a.altarID == altar.altarSO.displayName);
                if (data != null)
                    altar.FromSaveData(data);
            }

            TimeManager.Instance.ResetPlaytime();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SaveSystem] Error loading slot {slotIndex}: {ex.Message}\n{ex.StackTrace}");
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

            if (!SceneExistsInBuild(data.currentSceneID))
            {
                Debug.LogWarning($"[SaveSystem] Scene '{data.currentSceneID}' not found in build settings!");
                return null;
            }

            return data.currentSceneID;
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

    public static string GetSavedCheckpoint(int slotIndex)
    {
        try
        {
            if (!SaveExists(slotIndex)) return null;
            string json = File.ReadAllText(GetSlotPath(slotIndex));
            GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
            return data.currentCheckpointID;
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

            return (true, data.currentSceneID, data.timestamp, data.totalPlaytime);
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
