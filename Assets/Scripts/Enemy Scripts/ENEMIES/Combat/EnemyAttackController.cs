using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    public EnemyStatsSO enemyStats;
    
    [Header("Attack Settings")]
    public int enemyDamage;
    public bool isAttacking;
    public bool isChargingAttack;

    [Header("Knockback Settings")]
    public float knockbackForce;
    public float knockbackDuration;

    [SerializeField] private Transform enemy;

    private void Awake()
    {
        if (enemy == null)
            enemy = GetComponentInParent<Transform>();

        enemyDamage = enemyStats.damage;
        knockbackForce = enemyStats.knockback;
        knockbackDuration = enemyStats.knockbackDuration;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAttacking) return;

        if (collision.CompareTag("Player"))
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            IKnockbackable knockbackable = collision.GetComponent<IKnockbackable>();

            Vector2 knockbackVector = (collision.transform.position - enemy.position).normalized;

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
