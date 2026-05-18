using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SafeGroundTrigger : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.Instance.lastSafePosition = transform.position;
        }
    }
}