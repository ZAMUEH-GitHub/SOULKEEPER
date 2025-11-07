using UnityEngine;

public class PlayerInteractController : MonoBehaviour
{
    public bool isInteractable;
    private IInteractable interactable;

    private bool interactInput;
    private bool interactReleased = true;

    private void Update()
    {
        if (isInteractable && interactInput && interactReleased && interactable != null)
        {
            interactable.Interact();
            interactReleased = false;
        }

        if (!interactInput)
        {
            interactReleased = true;
        }
    }

    public void SetInteractInput(bool interactInput)
    {
        this.interactInput = interactInput;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var candidate = collision.GetComponentInParent<IInteractable>();
        if (candidate != null)
        {
            interactable = candidate;
            isInteractable = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var candidate = collision.GetComponentInParent<IInteractable>();
        if (candidate != null && candidate == interactable)
        {
            interactable = null;
            isInteractable = false;
        }
    }
}
