using UnityEngine;

public class PlayerInteractController : MonoBehaviour
{
    public bool isInteractable;
    private IInteractable interactable;

    private void Update()
    {
        if (isInteractable && PlayerController.Instance.ConsumeInteractInput() && interactable != null)
        {
            interactable.Interact();
        }
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