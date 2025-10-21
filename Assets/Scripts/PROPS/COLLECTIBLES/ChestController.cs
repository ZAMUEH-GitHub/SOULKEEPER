using UnityEngine;

public class ChestController : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Player Opened Chest!!");
    }
}
