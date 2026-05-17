using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    private EnemyBaseController enemy;

    [Header("Attack Settings")]
    public int enemyDamage;
    public bool isAttacking;
    public bool isChargingAttack;

    [Header("Obstacle Detection")]
    public LayerMask obstacleLayers;
    public bool hasHitObstacle;

    [Header("Knockback Settings")]
    public float knockbackForce;
    public float knockbackDuration;

    private void Awake()
    {
        enemy = GetComponentInParent<EnemyBaseController>();

        if (enemy != null && enemy.enemyStats != null)
        {
            enemyDamage = enemy.enemyStats.damage;
            knockbackForce = enemy.enemyStats.knockback;
            knockbackDuration = enemy.enemyStats.knockbackDuration;
        }
        else
        {
            Debug.LogError("[EnemyAttackController] EnemyBaseController or EnemyStatsSO is missing!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAttacking && !isChargingAttack) return;

        if (((1 << collision.gameObject.layer) & obstacleLayers) != 0)
        {
            hasHitObstacle = true;
            return;
        }

        if (isAttacking && collision.CompareTag("Player"))
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            IKnockbackable knockbackable = collision.GetComponent<IKnockbackable>();

            Vector2 knockbackVector = (collision.transform.position - enemy.transform.position).normalized;

            if (damageable != null)
            {
                damageable.TakeDamage(enemyDamage, knockbackVector);
            }

            if (knockbackable != null)
            {
                knockbackable.Knockback(knockbackVector, knockbackForce, knockbackDuration);
            }
        }
    }
}