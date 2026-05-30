using UnityEngine;

public class FireController : MonoBehaviour
{
    [Header("Spike Settings")]
    [SerializeField] private int damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player Attack Collider"))
        {
            PlayerController player = collision.GetComponentInParent<PlayerController>();
            if (player != null)
                Debug.Log("NIGGAAA");

            return;
        }
        else if (collision.CompareTag("Enemy Attack Collider"))
        {
            return;
        }

        IDamageable damageable = collision.GetComponent<IDamageable>();

        Vector2 collisionVector = collision.transform.position - transform.position;

        if (damageable != null)
            damageable.TakeDamage(damage, collisionVector);
    }
}