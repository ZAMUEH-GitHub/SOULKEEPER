using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(Collider2D))]
public class CameraTriggerZone : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("The static room camera to activate.")]
    [SerializeField] private CinemachineCamera roomCamera;

    private void Start()
    {
        if (roomCamera != null)
        {
            roomCamera.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"[CameraTriggerZone] No room camera assigned on {gameObject.name}!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PlayerController>() != null)
        {
            if (roomCamera != null)
            {
                roomCamera.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PlayerController>() != null)
        {
            if (roomCamera != null)
            {
                roomCamera.gameObject.SetActive(false);
            }
        }
    }
}