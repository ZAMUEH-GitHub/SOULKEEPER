using UnityEngine;

public class BulbController : MonoBehaviour
{
    [Header("Bulb Settings")]
    [SerializeField] private float bulbKnockbackForce = 20f;
    [SerializeField] private float bulbKnockbackDuration = 0.25f;
    [SerializeField] private float bulbAttackMultiplier = 3f;

    [Header("Bulb Components")]
    private Animator bulbAnimator;

    [Header("Effects")]
    [SerializeField] private ParticleSystem bulbHitParticles;
    [SerializeField] private Transform customParticlesTransform;

    private void Start()
    {
        bulbAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        

        if (collision.CompareTag("Player Attack Collider"))
        {
            bulbAnimator.SetTrigger("onContact");
            if (bulbHitParticles != null)
            {
                if (customParticlesTransform != null)
                {
                    ParticleSystem spawnedParticles = Instantiate(bulbHitParticles, customParticlesTransform.position, Quaternion.identity);
                    spawnedParticles.transform.localScale = customParticlesTransform.localScale;
                }
                else
                {
                    Instantiate(bulbHitParticles, transform.position, Quaternion.identity);
                }
            }

            PlayerController player = collision.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                Vector2 direction = (player.transform.position - transform.position).normalized;

                player.jumpController.ExecuteBounce(direction, bulbKnockbackForce * bulbAttackMultiplier);
            }
        }
        else if (collision.CompareTag("Enemy Attack Collider") )
        {
            bulbAnimator.SetTrigger("onContact");
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
        if (collision.gameObject.tag == "Player")
        {
            bulbAnimator.SetTrigger("onContact");
        }
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