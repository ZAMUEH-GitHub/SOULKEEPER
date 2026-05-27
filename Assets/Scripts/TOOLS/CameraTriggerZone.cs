using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(Collider2D))]
public class CameraTriggerZone : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("The room camera to activate.")]
    [SerializeField] private CinemachineCamera roomCamera;

    [Tooltip("Camera priority when active. Higher priority overrides lower ones (useful for nested zones).")]
    [SerializeField] private int activePriority = 10;

    [Tooltip("If true, this camera zone will only activate once and never again.")]
    [SerializeField] private bool isOneTime = false;

    private bool hasTriggered = false;

    private void Start()
    {
        if (roomCamera != null)
        {
            roomCamera.Priority = 0;
            roomCamera.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"[CameraTriggerZone] No room camera assigned on {gameObject.name}!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOneTime && hasTriggered) return;

        PlayerController player = collision.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            if (roomCamera != null)
            {
                roomCamera.Target.TrackingTarget = player.transform;
                roomCamera.Priority = activePriority;
                roomCamera.gameObject.SetActive(true);
            }

            if (isOneTime)
            {
                hasTriggered = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            if (roomCamera != null)
            {
                roomCamera.Priority = 0;
                roomCamera.gameObject.SetActive(false);
                roomCamera.Target.TrackingTarget = null;
            }
        }
    }
}