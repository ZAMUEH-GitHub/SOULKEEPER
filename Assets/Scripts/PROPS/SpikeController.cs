using UnityEngine;

public class SpikeController : MonoBehaviour
{
    [Header("Spike Settings")]
    [SerializeField] private int damage;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackDuration;
    [SerializeField] private float attackMultiplier = 3f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player Attack Collider"))
        {
            PlayerController player = collision.GetComponentInParent<PlayerController>();
            if (player != null)
                Debug.Log("NIGGAAA");
            {
                Vector2 direction = (player.transform.position - transform.position).normalized;
                player.jumpController.ExecuteBounce(direction, knockbackForce * attackMultiplier);
            }
            return;
        }
        else if (collision.CompareTag("Enemy Attack Collider"))
        {
            IKnockbackable attackKnockbackable = collision.GetComponentInParent<IKnockbackable>();
            if (attackKnockbackable != null)
            {
                Vector2 direction = (collision.transform.position - transform.position).normalized;
                attackKnockbackable.Knockback(direction, knockbackForce, knockbackDuration);
            }
            return;
        }

        IKnockbackable knockbackable = collision.GetComponent<IKnockbackable>();
        IDamageable damageable = collision.GetComponent<IDamageable>();

        Vector2 collisionVector = collision.transform.position - transform.position;

        if (knockbackable != null)
            knockbackable.Knockback(collisionVector, knockbackForce, knockbackDuration);

        if (damageable != null)
            damageable.TakeDamage(damage, collisionVector);
    }
}