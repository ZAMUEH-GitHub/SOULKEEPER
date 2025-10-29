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

    private EnemyBaseController enemyBaseController;

    private SpriteRenderer enemySprite;
    private Rigidbody2D enemyRB;

    private void Awake()
    {
        enemyBaseController = GetComponent<EnemyBaseController>();
        enemySprite = GetComponent<SpriteRenderer>();
        enemyRB = GetComponent<Rigidbody2D>();

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
        enemyRB.linearVelocity = (new Vector2(knockbackVector.x, knockbackVector.y + 1) * knockbackForce);
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
            enemyBaseController.Die();
        }
    }
}
