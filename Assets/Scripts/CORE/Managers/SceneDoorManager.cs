using System.Collections;
using System.Linq;
using UnityEngine;

public class SceneDoorManager : MonoBehaviour
{
    [Header("Scene Doors")]
    [SerializeField] private GameObject[] sceneDoors;
    private static string lastUsedDoorID;

    private GameObject player;
    private SaveSlotManager saveSlotManager;

    #region Unity Lifecycle
    private void Start()
    {
        FindAllDoors();

        player = GameObject.FindGameObjectWithTag("Player");

        Debug.Log($"[SceneDoorManager] Initialized with {sceneDoors.Length} doors in scene '{gameObject.scene.name}'.");
    }
    #endregion

    #region Door Logic
    private void FindAllDoors()
    {
        SceneDoor[] doorComponents = FindObjectsByType<SceneDoor>(FindObjectsSortMode.None);
        sceneDoors = doorComponents.Select(d => d.gameObject).ToArray();

        if (sceneDoors == null || sceneDoors.Length == 0)
            Debug.LogWarning($"[SceneDoorManager] No doors found in scene '{gameObject.scene.name}'.");
    }

    public void RegisterDoorUse(string doorID)
    {
        lastUsedDoorID = doorID;

        var runtimeStats = SessionManager.Instance.RuntimeStats;
        if (runtimeStats == null)
        {
            Debug.LogWarning("[SceneDoorManager] RuntimePlayerStats not found — skipping autosave.");
            return;
        }

        saveSlotManager ??= SaveSlotManager.Instance;
        if (saveSlotManager == null)
        {
            Debug.LogWarning("[SceneDoorManager] SaveSlotManager.Instance not found — cannot autosave.");
            return;
        }

        int slot = saveSlotManager.ActiveSlotIndex;
        SaveSystem.Save(slot, runtimeStats, doorID);
    }

    public void ChooseDoor(string targetDoorID)
    {
        if (sceneDoors == null || sceneDoors.Length == 0)
        {
            StartCoroutine(WaitAndChooseDoor(targetDoorID));
            return;
        }

        TeleportToDoor(targetDoorID);
    }

    private IEnumerator WaitAndChooseDoor(string doorID)
    {
        for (int i = 0; i < 5; i++)
        {
            yield return null;
            FindAllDoors();

            if (sceneDoors.Any())
            {
                TeleportToDoor(doorID);
                yield break;
            }
        }

        Debug.LogWarning($"[SceneDoorManager] Failed to find door '{doorID}' after waiting.");
    }

    private void TeleportToDoor(string doorID)
    {
        var targetDoor = sceneDoors
            .Select(d => d.GetComponent<SceneDoor>())
            .FirstOrDefault(sd => sd != null && sd.doorID == doorID);

        if (targetDoor == null)
        {
            Debug.LogWarning($"[SceneDoorManager] Door '{doorID}' not found in this scene.");
            return;
        }

        StartCoroutine(DelayedTeleport(targetDoor.gameObject));
    }

    private IEnumerator DelayedTeleport(GameObject targetDoor)
    {
        player ??= GameObject.FindGameObjectWithTag("Player");
        int safety = 0;
        while (player == null && safety++ < 5)
        {
            yield return null;
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player != null)
        {
            player.transform.position = targetDoor.transform.position;
            Debug.Log($"[SceneDoorManager] Teleported player to '{targetDoor.name}' in scene '{gameObject.scene.name}'.");
        }
        else
        {
            Debug.LogWarning("[SceneDoorManager] Could not find player to teleport.");
        }
    }
    #endregion
}
