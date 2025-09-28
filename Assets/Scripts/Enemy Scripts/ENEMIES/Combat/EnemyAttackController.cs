using UnityEngine;
using System.Collections;

public class EnemyAttackController : MonoBehaviour
{
    public EnemyStatsSO enemyStats;
    
    [Header("Attack Settings")]
    public int enemyDamage;
    public bool isAttacking;
    private float attackRate;
    private float attackTimer;
    private float attackRange;
    private float nextAttack;
    private bool canComboAttack;
    private bool hasChargedAttack;
    public bool isChargingAttack;
    public float attackChargeDuration;
    public float attackDuration;
    public float attackForce;
    public enum AttackState { Attack1, Attack2, Attack3 }
    public AttackState currentAttackState = AttackState.Attack1;

    [Header("Knockback Settings")]
    public Vector2 knockbackVector;
    public float knockbackForce;
    public float knockbackDuration;

    private Animator animator;
    private Rigidbody2D enemyRB;
    public Transform enemy;

    private EnemyJumpController jumpController;
    private EnemyDamageController damageController;

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
        enemyRB = GetComponentInParent<Rigidbody2D>();
        jumpController = GetComponentInParent<EnemyJumpController>();
        damageController = GetComponentInParent<EnemyDamageController>();

        enemyDamage = enemyStats.damage;
        attackRate = enemyStats.attackRate;
        attackRange = enemyStats.attackRange;
        canComboAttack = enemyStats.canComboAttack;
        hasChargedAttack = enemyStats.hasChargedAttack;
        knockbackForce = enemyStats.knockback;
    }

    private void Update()
    {
        nextAttack = Mathf.Max(0, nextAttack - Time.deltaTime);
        attackTimer = Mathf.Max(0, attackTimer - Time.deltaTime);

        if (attackTimer == 0)
        {
            currentAttackState = AttackState.Attack1;
        }

        animator.SetBool("isChargingAttack", isChargingAttack);
        animator.SetBool("isAttacking", isAttacking);
    }

    public void EnemyAttack(Vector2 playerTarget)
    {
        if (canComboAttack)
        {
            ComboAttack(playerTarget);
        }
        
        if (hasChargedAttack)
        {
            ChargedAttack(playerTarget);
        }

        if (!canComboAttack && !hasChargedAttack)
        {
            SimpleAttack(playerTarget);
        }
    }

    private void SimpleAttack(Vector2 playerTarget)
    {
        if (Vector2.Distance(transform.position, playerTarget) <= attackRange && nextAttack <= 0)
        {
            if (!isAttacking)
            {
                animator.SetTrigger("EnemyAttack");
                nextAttack = attackRate;
            }
        }
    }
    private void ChargedAttack(Vector2 playerTarget)
    {
        if (!jumpController.isGrounded || !jumpController.isChargingJump) return;

        if (Vector2.Distance(transform.position, playerTarget) <= 5 && nextAttack <= 0)
        {
            if (!isAttacking && !isChargingAttack)
            {
                float attackDirX = Mathf.Sign(playerTarget.x - transform.position.x);

                isChargingAttack = true;
                StartCoroutine(EnemyChargeAttack(new Vector2(attackDirX, 0)));
            }
        }
    }

    private IEnumerator EnemyChargeAttack(Vector2 attackVector)
    {
        yield return new WaitForSeconds(attackChargeDuration);

        isChargingAttack = false;
        isAttacking = true;

        enemyRB.linearVelocity = new Vector2(attackVector.x * attackForce, enemyRB.linearVelocity.y);
        nextAttack = attackRate;

        yield return new WaitForSeconds(attackDuration);

        enemyRB.linearVelocity = new Vector2(0, enemyRB.linearVelocity.y);
        isAttacking = false;
    }

    private void ComboAttack(Vector2 playerTarget)
    {
        if (Vector2.Distance(transform.position, playerTarget) <= attackRange && nextAttack <= 0 && !damageController.isKnockedBack && !isAttacking)
        {
            switch (currentAttackState)
            {
                case AttackState.Attack1:
                    Attack1();
                    break;

                case AttackState.Attack2:
                    Attack2();
                    break;

                case AttackState.Attack3:
                    Attack3();
                    break;
            }
        }
    }

    private void Attack1()
    {
        animator.SetTrigger("EnemyAttack1");
        attackTimer = attackRate * 2f;
        currentAttackState = AttackState.Attack2;
        nextAttack = attackRate;
    }

    private void Attack2()
    {
        animator.SetTrigger("EnemyAttack2");
        attackTimer = attackRate * 2f;
        currentAttackState = AttackState.Attack3;
        nextAttack = attackRate;
    }

    private void Attack3()
    {
        animator.SetTrigger("EnemyAttack3");
        attackTimer = attackRate * 2f;
        currentAttackState = AttackState.Attack1;
        nextAttack = attackRate;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            IKnockbackable knockbackable = collision.GetComponent<IKnockbackable>();

            Vector2 knockbackVector = (collision.transform.position - enemy.position).normalized;

            if (damageable != null)
            {
                damageable.TakeDamage(enemyDamage, knockbackVector);
            }

            if (knockbackable  != null)
            {
                knockbackable.Knockback(knockbackVector, knockbackForce, knockbackDuration);
            }
        }
    }
}