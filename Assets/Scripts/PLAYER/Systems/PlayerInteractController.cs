using UnityEngine;

public class PlayerInteractController : MonoBehaviour
{
    public bool isInteractable => activeTarget != null;
    private IInteractable activeTarget;

    [Header("Scanner Settings")]
    [SerializeField] private float scanRadius = 1.5f;
    [Tooltip("Leave empty to scan all layers, or set to an 'Interactable' layer for better performance.")]
    [SerializeField] private LayerMask interactableLayer;

    private Transform scanOrigin;
    private Collider2D[] scanResults = new Collider2D[10];

    private void Awake()
    {
        scanOrigin = transform;
    }

    private void FixedUpdate()
    {
        ScanForInteractables();
    }

    private void Update()
    {
        if (isInteractable && PlayerController.Instance.ConsumeInteractInput())
        {
            activeTarget.Interact();
        }
    }

    private void ScanForInteractables()
    {
        int hitCount = interactableLayer == 0
            ? Physics2D.OverlapCircleNonAlloc(scanOrigin.position, scanRadius, scanResults)
            : Physics2D.OverlapCircleNonAlloc(scanOrigin.position, scanRadius, scanResults, interactableLayer);

        IInteractable closestTarget = null;
        float closestDistance = float.MaxValue;
        bool arrivalDoorStillInRange = false;

        for (int i = 0; i < hitCount; i++)
        {
            var candidate = scanResults[i].GetComponentInParent<IInteractable>();
            if (candidate != null)
            {
                if (candidate is SceneDoor door && door.doorID == SceneDoorManager.ArrivalDoorID)
                {
                    arrivalDoorStillInRange = true;
                }

                float dist = Vector2.Distance(scanOrigin.position, scanResults[i].transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestTarget = candidate;
                }
            }
        }

        activeTarget = closestTarget;

        if (!arrivalDoorStillInRange && !string.IsNullOrEmpty(SceneDoorManager.ArrivalDoorID))
        {
            SceneDoorManager.ClearArrivalDoor();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, scanRadius);
    }
}