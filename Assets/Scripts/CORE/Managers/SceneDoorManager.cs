using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDoorManager : MonoBehaviour
{
    [Header("Scene Doors")]
    public GameObject[] sceneDoors;

    [SerializeField] SaveSlotManager saveSlotManager;
    private GameObject player;
    private static string lastUsedDoorID;

    private void Awake()
    {
        var existing = FindObjectsByType<SceneDoorManager>(FindObjectsSortMode.None);
        if (existing.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        FindAllDoors();
    }

    private void FindAllDoors()
    {
        SceneDoor[] doorComponents = FindObjectsByType<SceneDoor>(FindObjectsSortMode.None);
        sceneDoors = doorComponents.Select(d => d.gameObject).ToArray();
    }

    public void RegisterDoorUse(string doorID)
    {
        lastUsedDoorID = doorID;
        Debug.Log($"[SceneDoorManager] Player used door: {doorID}");

        var playerStats = FindFirstObjectByType<PlayerStatsSO>();
        if (playerStats != null)
        {
            int slot = saveSlotManager != null ? saveSlotManager.ActiveSlotIndex : 1;
            SaveSystem.Save(slot, playerStats, doorID);
            Debug.Log($"[SceneDoorManager] Auto-saved door '{doorID}' to slot {slot}");
        }
    }

    public void ChooseDoor(string targetDoorID)
    {
        var targetDoor = sceneDoors
            .Select(d => d.GetComponent<SceneDoor>())
            .FirstOrDefault(sd => sd != null && sd.doorID == targetDoorID);

        if (targetDoor == null)
        {
            Debug.LogWarning($"[SceneDoorManager] Door '{targetDoorID}' not found in this scene.");
            return;
        }

        StartCoroutine(DelayedTeleport(targetDoor.gameObject));
    }

    private IEnumerator DelayedTeleport(GameObject targetDoor)
    {
        yield return new WaitForEndOfFrame();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            player.transform.position = targetDoor.transform.position;
            Debug.Log($"[SceneDoorManager] Teleported player to {targetDoor.name}");
        }
        else
        {
            Debug.LogWarning("[SceneDoorManager] Player not found for teleport!");
        }
    }
}
