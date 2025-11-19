using UnityEngine;

public class BulbController : MonoBehaviour
{
    [Header("Bulb Settings")]
    [SerializeField] private float bulbKnockbackForce = 20f;
    [SerializeField] private float bulbKnockbackDuration = 0.25f;

    [Header("Effects")]
    public ParticleSystem bulbHitParticles;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player Attack Collider") || collision.CompareTag("Enemy Attack Collider"))
        {
            Instantiate(bulbHitParticles, transform.position, Quaternion.identity);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Instantiate(bulbHitParticles, transform.position, Quaternion.identity);
        
        //Vector2 forceVector = collision.transform.position - transform.position;
        //Rigidbody2D rB = collision.gameObject.GetComponent<Rigidbody2D>();
        //rB.AddForce(forceVector.normalized * bulbKnockbackForce, ForceMode2D.Impulse);

        IKnockbackable knockbackable = collision.gameObject.GetComponent<IKnockbackable>();
        if (knockbackable != null)
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            knockbackable.Knockback(direction, bulbKnockbackForce, bulbKnockbackDuration);
        }
    }
}
