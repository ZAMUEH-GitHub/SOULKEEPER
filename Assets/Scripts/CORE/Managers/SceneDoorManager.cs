using UnityEngine;

public class SceneDoorManager : MonoBehaviour
{
    public GameObject[] sceneDoors;
    private GameObject player;
    private GameObject chosenDoor;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void ChooseDoor(int doorName)
    {
        chosenDoor = sceneDoors[doorName];
        TeleportPlayer(chosenDoor);
    }

    private void TeleportPlayer(GameObject chosenDoor)
    {
        player.transform.position = chosenDoor.transform.position;
    }
}
