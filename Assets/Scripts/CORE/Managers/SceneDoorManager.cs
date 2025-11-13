using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDoorManager : Singleton<SceneDoorManager>
{
    protected override bool IsPersistent => false;

    [Header("Scene Doors")]
    [SerializeField] private GameObject[] sceneDoors;
    private static string lastUsedDoorID;

    private GameObject player;
    private SaveSlotManager saveSlotManager;

    #region Unity Lifecycle
    protected override void Awake()
    {
        if (Instance == null)
        {
            var singletonField = typeof(Singleton<SceneDoorManager>).GetField("_instance",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            singletonField?.SetValue(null, this);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        GameSceneManager gsm = GameSceneManager.Instance;

        FindAllDoors();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ReinitializeAfterSceneLoad());
    }

    private IEnumerator ReinitializeAfterSceneLoad()
    {
        yield return null;

        FindAllDoors();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
    }
    #endregion

    #region Door Logic
    private void FindAllDoors()
    {
        sceneDoors = null;

        SceneDoor[] doorComponents = FindObjectsByType<SceneDoor>(FindObjectsSortMode.None);
        sceneDoors = doorComponents.Select(d => d.gameObject).ToArray();
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
        for (int i = 0; i < 5 && player == null; i++)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }

        if (player != null)
        {
            player.transform.position = targetDoor.transform.position;
        }
        else
        {
            Debug.LogWarning("[SceneDoorManager] Could not find player to teleport.");
        }
    }
    #endregion
}
