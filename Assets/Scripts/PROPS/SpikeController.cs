using UnityEngine;

public class SpikeController : MonoBehaviour
{
    [Header("Spike Settings")]
    [SerializeField] private int damage;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackDuration;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IKnockbackable knockbackable = collision.GetComponent<IKnockbackable>();
        IDamageable damageable = collision.GetComponent<IDamageable>();

        Vector2 collisionVector = collision.transform.position - transform.position;

        if (knockbackable != null)
            knockbackable.Knockback(collisionVector, knockbackForce, knockbackDuration);

        if (damageable != null)
            damageable.TakeDamage(damage, collisionVector);
    }
}
