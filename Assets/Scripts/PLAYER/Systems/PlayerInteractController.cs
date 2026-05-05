using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractController : MonoBehaviour
{
    public bool isInteractable => interactablesInRange.Count > 0;

    private List<IInteractable> interactablesInRange = new List<IInteractable>();

    private void Update()
    {
        if (isInteractable && PlayerController.Instance.ConsumeInteractInput())
        {
            IInteractable target = interactablesInRange[interactablesInRange.Count - 1];

            if (target != null)
            {
                target.Interact();
            }
            else
            {
                interactablesInRange.RemoveAt(interactablesInRange.Count - 1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var candidate = collision.GetComponentInParent<IInteractable>();

        if (candidate != null && !interactablesInRange.Contains(candidate))
        {
            interactablesInRange.Add(candidate);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var candidate = collision.GetComponentInParent<IInteractable>();

        if (candidate != null)
        {
            interactablesInRange.Remove(candidate);
        }
    }
}