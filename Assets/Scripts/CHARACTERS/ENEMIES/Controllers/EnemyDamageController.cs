using System.Collections;
using UnityEngine;

public class EnemyDamageController : MonoBehaviour, IKnockbackable, IDamageable
{
    public EnemyStatsSO enemyStats;

    [Header("Damage Stats")]
    public int enemyHealth;
    public int enemyScore;
    public bool isTakingDamage;
    public bool isAlive = true;

    [Header("VFX Settings")]
    public GameObject soulObject;
    public ParticleSystem damageParticles;
    public ParticleSystem deathParticles;
    [SerializeField] private SpriteRenderer[] enemySprites;

    private float damageRate = 0.25f;
    private float nextDamage;

    private Coroutine takeDamageCoroutine;
    private EnemyBaseController enemyBaseController;
    private Rigidbody2D enemyRB;

    private void Awake()
    {
        enemyBaseController = GetComponent<EnemyBaseController>();
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
        if (enemyHealth <= 0) return;

        enemyBaseController.stateMachine.ChangeState(new KnockbackState(enemyBaseController, knockbackVector, knockbackForce, knockbackDuration));
    }

    public void TakeDamage(int damage, Vector2 damageVector)
    {
        if (enemyHealth <= 0) return;

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

        if (enemyHealth <= 0)
        {
            enemyBaseController.Die();
        }

        foreach (SpriteRenderer sr in enemySprites)
        {
            sr.color = Color.red;
        }

        yield return new WaitForSeconds(0.25f);

        foreach (SpriteRenderer sr in enemySprites)
        {
            sr.color = Color.white;
        }

        isTakingDamage = false;
    }
}