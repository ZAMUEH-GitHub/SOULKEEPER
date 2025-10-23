using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDoorManager : MonoBehaviour
{
    [Header("Scene Doors")]
    public GameObject[] sceneDoors;

    private GameObject player;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

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

    public void ChooseDoor(string targetDoorID)
    {
        foreach (GameObject targetDoor in sceneDoors)
        {
            var doorComp = targetDoor.GetComponent<SceneDoor>();
            if (doorComp != null && doorComp.doorID == targetDoorID)
            {
                TeleportPlayer(targetDoor);
                return;
            }
        }
    }

    private void TeleportPlayer(GameObject targetDoor)
    {
        player.transform.position = targetDoor.transform.position;
    }
}
