using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyDamageController : MonoBehaviour, IKnockbackable, IDamageable
{
    public EnemyStatsSO enemyStats;

    public int enemyHealth;
    public int enemyScore;
    public bool isTakingDamage;
    public bool isAlive;

    public GameObject soulObject;
    public ParticleSystem damageParticles;
    public ParticleSystem deathParticles;

    private float damageRate = 0.25f;
    private float nextDamage;
    public bool isKnockedBack;

    private Coroutine takeDamageCoroutine;

    private SpriteRenderer enemySprite;
    private Rigidbody2D enemyRB;
    private Animator enemyAnimator;

    private void Awake()
    {
        enemySprite = GetComponent<SpriteRenderer>();
        enemyRB = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();

        enemyHealth = enemyStats.health;
        enemyScore = enemyStats.score;
    }

    private void Update()
    {
        nextDamage = Mathf.Max(0, nextDamage - Time.deltaTime);
    }

    public void Knockback(Vector2 knockbackVector, float knockbackForce, float knockbackDuration)
    {
        enemyRB.AddForce(knockbackVector * knockbackForce, ForceMode2D.Impulse);
        enemyRB.linearVelocity = (knockbackVector * knockbackForce);
        isKnockedBack = true;
        StartCoroutine(CancelKnockback(knockbackDuration));
    }
    public IEnumerator CancelKnockback(float knockbackDuration)
    {
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
    }

    public void TakeDamage(int damage, Vector2 damageVector)
    {
        Quaternion particleRotation = Quaternion.FromToRotation(Vector2.up, damageVector);

        if (!isTakingDamage && nextDamage <= 0)
        {
            if (takeDamageCoroutine != null)
            {
                StopCoroutine(takeDamageCoroutine);
            }
            takeDamageCoroutine = StartCoroutine(ExecuteTakeDamage(damage));
            Instantiate(damageParticles, transform.position, particleRotation);
        }
    }

    private IEnumerator ExecuteTakeDamage(int damage)
    {
        isTakingDamage = true;
        enemyHealth -= damage;
        nextDamage = damageRate;
        enemySprite.color = Color.red;

        yield return new WaitForSeconds(0.25f);

        enemySprite.color = Color.white;
        isTakingDamage = false;

        if (enemyHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isAlive = false;

        transform.Translate(Vector2.zero);
        enemyRB.constraints = RigidbodyConstraints2D.FreezeAll;

        enemyAnimator.SetTrigger("EnemyDeath");
        //unlocker?.TriggerUnlock();
    }
    private void Destroy()
    {
        Instantiate(deathParticles, new Vector2(transform.position.x, transform.position.y - 1.5f), Quaternion.identity);

        for (int i = enemyScore; i > 0; i--)
        {
            GameObject soul = Instantiate(soulObject, transform.position, Quaternion.identity);
            soul.transform.position = new Vector2(soul.transform.position.x + Random.Range(-2f, 2f), soul.transform.position.y + Random.Range(-2f, -0.5f));
        }

        Destroy(gameObject);
    }
}
