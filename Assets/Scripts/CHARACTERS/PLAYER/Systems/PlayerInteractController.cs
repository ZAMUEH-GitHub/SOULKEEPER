using UnityEngine;

public class PlayerInteractController : MonoBehaviour
{
    private bool interactInput;
    public bool isInteractable;

    private IInteractable interactable;

    private void Update()
    {
        if (isInteractable && interactInput)
        {
            interactable.Interact();
        }
    }

    public void SetInteractInput(bool interactInput)
    {
        this.interactInput = interactInput;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        interactable = collision.GetComponent<IInteractable>();

        if (interactable != null)
        {
            isInteractable = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        interactable = collision.GetComponent<IInteractable>();

        if (interactable != null)
        {
            isInteractable = false;
        }
    }
}