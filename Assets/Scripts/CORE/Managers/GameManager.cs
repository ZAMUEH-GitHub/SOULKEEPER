using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Session Info")]
    public int CurrentSlot = -1;
    public bool HasActiveSession => CurrentSlot > 0;

    [Header("Autosave Settings")]
    public bool enableTimedAutosave = true;
    public float autosaveInterval = 180f;

    private Coroutine autosaveRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (HasActiveSession)
        {
            SaveGame();
            Debug.Log($"[GameSessionManager] Autosave on scene load: {scene.name}");
        }

        if (enableTimedAutosave && autosaveRoutine == null)
            autosaveRoutine = StartCoroutine(AutosaveTimer());
    }

    private IEnumerator AutosaveTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(autosaveInterval);
            if (HasActiveSession)
            {
                SaveGame();
                Debug.Log($"[GameSessionManager] Timed autosave at {Time.time:F1}s");
            }
        }
    }

    // -----------------------------------------------------------------
    // PUBLIC INTERFACE
    // -----------------------------------------------------------------

    public void StartNewGame(int slot)
    {
        CurrentSlot = slot;
        Debug.Log($"[GameSessionManager] Starting new game in slot {slot}");

        // Reset runtime stats to defaults
        var playerStats = PlayerController.Instance.playerBaseStats.Clone();
        PlayerController.Instance.playerRuntimeStats = playerStats;

        SaveSystem.Save(CurrentSlot, playerStats);
    }

    public void LoadGame(int slot)
    {
        CurrentSlot = slot;
        Debug.Log($"[GameSessionManager] Loading slot {slot}");

        SaveSystem.Load(CurrentSlot, PlayerController.Instance.playerRuntimeStats);
    }

    public void SaveGame()
    {
        if (!HasActiveSession)
        {
            Debug.LogWarning("[GameSessionManager] No active slot to save to.");
            return;
        }

        SaveSystem.Save(CurrentSlot, PlayerController.Instance.playerRuntimeStats);
    }

    public void SaveAndQuit()
    {
        SaveGame();
        Debug.Log("[GameSessionManager] Game saved before quit.");
        Application.Quit();
    }
}
