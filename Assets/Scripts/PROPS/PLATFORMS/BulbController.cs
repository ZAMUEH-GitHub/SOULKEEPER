using UnityEngine;

public class BulbController : MonoBehaviour
{
    [Header("Bulb Settings")]
    [SerializeField] private float bulbKnockbackForce = 20f;
    [SerializeField] private float bulbKnockbackDuration = 0.25f;
    [SerializeField] private float bulbAttackMultiplier = 1.5f;

    [Header("Effects")]
    public ParticleSystem bulbHitParticles;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player Attack Collider"))
        {
            if (bulbHitParticles != null)
                Instantiate(bulbHitParticles, transform.position, Quaternion.identity);

            PlayerController player = collision.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                Vector2 direction = (player.transform.position - transform.position).normalized;

                player.jumpController.ExecuteBounce(direction, bulbKnockbackForce * bulbAttackMultiplier);
            }
        }
        else if (collision.CompareTag("Enemy Attack Collider"))
        {
            if (bulbHitParticles != null)
                Instantiate(bulbHitParticles, transform.position, Quaternion.identity);

            IKnockbackable knockbackable = collision.GetComponentInParent<IKnockbackable>();
            if (knockbackable != null)
            {
                Vector2 direction = (collision.transform.position - transform.position).normalized;
                knockbackable.Knockback(direction, bulbKnockbackForce, bulbKnockbackDuration);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (bulbHitParticles != null)
            Instantiate(bulbHitParticles, transform.position, Quaternion.identity);

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            player.jumpController.ExecuteBounce(direction, bulbKnockbackForce);
        }
        else
        {
            IKnockbackable knockbackable = collision.gameObject.GetComponent<IKnockbackable>();
            if (knockbackable != null)
            {
                Vector2 direction = (collision.transform.position - transform.position).normalized;
                knockbackable.Knockback(direction, bulbKnockbackForce, bulbKnockbackDuration);
            }
        }
    }
}