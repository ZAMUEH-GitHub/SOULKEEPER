using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    private EnemyBaseController enemy;
    private Collider2D attackCollider;

    [Header("Attack Settings")]
    public int enemyDamage;

    // Changing these to Properties automates the Physics Collider!
    [SerializeField] private bool _isAttacking;
    public bool isAttacking
    {
        get { return _isAttacking; }
        set
        {
            _isAttacking = value;
            UpdateCollider();
        }
    }

    [SerializeField] private bool _isChargingAttack;
    public bool isChargingAttack
    {
        get { return _isChargingAttack; }
        set
        {
            _isChargingAttack = value;
            UpdateCollider();
        }
    }

    [Header("Obstacle Detection")]
    public LayerMask obstacleLayers;
    public bool hasHitObstacle;

    [Header("Knockback Settings")]
    public float knockbackForce;
    public float knockbackDuration;

    private void Awake()
    {
        enemy = GetComponentInParent<EnemyBaseController>();
        attackCollider = GetComponent<Collider2D>();

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

        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    private void UpdateCollider()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = _isAttacking || _isChargingAttack;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isAttacking && !_isChargingAttack) return;

        if (((1 << collision.gameObject.layer) & obstacleLayers) != 0)
        {
            hasHitObstacle = true;
            return;
        }

        if (_isAttacking && collision.CompareTag("Player"))
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