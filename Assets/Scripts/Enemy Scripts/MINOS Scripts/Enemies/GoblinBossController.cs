using UnityEngine;
using System.Collections;

public class GoblinBossController : MonoBehaviour, IEnemy
{
    [Header("Enemy Stats")]
    public int enemyHealth;
    public float enemySpeed;
    public int enemyScore;
    public GameObject soulObject;
    public bool isAlive = true;
    private bool isTakingDamage = false;

    [Header("Target Settings")]
    public bool targetPlayer;
    public bool isIdling;
    public float idleDuration;
    private Vector2 currentTarget;
    private Vector2 playerTarget;

    [Header("Jump Settings")]
    public bool canJump;
    public float enemyJumpForce;
    public float jumpRate;
    private float nextJump;
    public float jumpChargeDuration;
    private bool isChargingJump;

    [Header("Attack Settings")]
    public bool isChargingAttack;
    public bool isAttacking;
    public float attackChargeDuration;
    public float attackForce;
    public float attackDuration;
    public float attackRate;
    private float nextAttack;

    [Header("Knockback Settings")]
    public bool isKnockedBack;
    public float knockbackDuration;
    public Vector2 knockbackVector;

    [Header("GameObjects")]
    public EnemyJumpController enemyJumpController;
    public EnemyWallController enemyWallController;
    public ParticleSystem damageParticles;
    public ParticleSystem deathParticles;

    private PlayerController playerController;
    private Transform playerTransform;
    private GameManager gameManager;

    private CapsuleCollider2D enemyCollider;
    private Rigidbody2D enemyRigidBody;
    private Animator enemyAnimator;
    private SpriteRenderer enemySprite;


    void Start()
    {
        enemyCollider = GetComponent<CapsuleCollider2D>();
        enemyRigidBody = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();
        enemySprite = GetComponent<SpriteRenderer>();

        gameManager = GameObject.Find("GAME MANAGER").GetComponent<GameManager>();

        if (PlayerController.Instance != null)
        {
            playerController = PlayerController.Instance;
            playerTransform = PlayerController.Instance.transform;
        }
        else
        {
            Debug.LogWarning("PlayerController instance not found!");
        }

        isAlive = true;
        targetPlayer = true;
    }

    void Update()
    {
        if (!isAlive) return;
        
        playerTarget = playerTransform.position;

        if (!isIdling)
        {
            if (targetPlayer)
            {
                MoveToTarget(playerTarget);
            }
        }
        
        EnemyJump();
        EnemyAttack();
        EnemyFlip();
        EnemyAnimation();

        nextJump = Mathf.Max(0, nextJump - Time.deltaTime);
        nextAttack = Mathf.Max(0, nextAttack - Time.deltaTime);
    }

    private void MoveToTarget(Vector2 target)
    {
        if (!isAlive || isChargingJump || isKnockedBack || isChargingAttack || isAttacking || isIdling) return;

        Vector2 direction = (target - (Vector2)transform.position).normalized;
        enemyRigidBody.linearVelocity = new Vector2(direction.x * enemySpeed, enemyRigidBody.linearVelocityY);

        currentTarget = target;
    }

    public void SetTargetPlayer(bool targetPlayer)
    {
        this.targetPlayer = targetPlayer;
    }

    private void EnemyJump()
    {
        if (!isAlive || !canJump || isAttacking || isChargingAttack || isKnockedBack) return;

        if (enemyJumpController.isGrounded && enemyWallController.isWalled && !isChargingJump && currentTarget.y > transform.position.y + 1 && nextJump <= 0)
        {
            StartCoroutine(JumpCharge());
            isChargingJump = true;
        }
    }

    private IEnumerator JumpCharge()
    {
        yield return new WaitForSeconds(jumpChargeDuration);
        enemyRigidBody.linearVelocity = new Vector2(enemyRigidBody.linearVelocityX, enemyJumpForce);
        nextJump = jumpRate;
        isChargingJump = false;
        enemyAnimator.SetTrigger("EnemyJump");
    }

    private void EnemyFlip()
    {
        if (!isAlive || isChargingAttack || isAttacking) return;

        Vector2 targetPosition = targetPlayer ? playerTarget : currentTarget;

        if (targetPosition.x > transform.position.x)
        {
            transform.localScale = new Vector2(1, 1);
        }
        else if (targetPosition.x < transform.position.x)
        {
            transform.localScale = new Vector2(-1, 1);
        }
    }

    private void EnemyAttack()
    {
        if (!isAlive) return;

        if (Vector2.Distance(transform.position, playerTarget) <= 5 && nextAttack <= 0)
        {
            if (!isAttacking)
            {
                StartCoroutine(EnemyChargeAttack(new Vector2(transform.localScale.x, 0)));
                isChargingAttack = true;
            }
        }
    }

    private IEnumerator EnemyChargeAttack(Vector2 attackVector)
    {
        yield return new WaitForSeconds(attackChargeDuration);
        isChargingAttack = false;
        isAttacking = true;
        enemyRigidBody.linearVelocity = new Vector2(attackVector.x * attackForce, enemyRigidBody.linearVelocityY);
        nextAttack = attackRate;
        StartCoroutine(CancelEnemyAttack());
    }

    private IEnumerator CancelEnemyAttack()
    {
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }

    private void EnemyAnimation()
    {
        if (!isAlive) return;

        if (enemyJumpController.isGrounded && Mathf.Abs(enemyRigidBody.linearVelocityX) > 0.1f)
        {
            enemyAnimator.SetBool("isMoving", true);
        }
        else { enemyAnimator.SetBool("isMoving", false); }

        if (!enemyJumpController.isGrounded && Mathf.Abs(enemyRigidBody.linearVelocityY) > 0.1f)
        {
            enemyAnimator.SetBool("isFalling", true);
        }
        else { enemyAnimator.SetBool("isFalling", false); }

        enemyAnimator.SetBool("isChargingJump", isChargingJump);
        enemyAnimator.SetBool("isChargingAttack", isChargingAttack);
        enemyAnimator.SetBool("isAttacking", isAttacking);
    }

    public void EnemyKnockback(Vector2 knockbackVector, float knockbackForce)
    {
        if (!isAlive) return;

        enemyRigidBody.linearVelocity = knockbackVector * knockbackForce;
        isKnockedBack = true;
        StartCoroutine(CancelEnemyKnockback());
    }

    private IEnumerator CancelEnemyKnockback()
    {
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
    }

    public IEnumerator TakeDamage(int damage, Vector2 damageVector)
    {
        Quaternion particleRotation = Quaternion.FromToRotation(Vector2.up, damageVector);

        if (!isTakingDamage)
        {
            enemyHealth -= damage;
            isTakingDamage = true;
            Instantiate(damageParticles, transform.position, particleRotation);
            enemySprite.color = Color.red;
            yield return new WaitForSeconds(0.25f);
            isTakingDamage = false;
            enemySprite.color = Color.white;
            if (enemyHealth <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        isAlive = false;
        transform.Translate(Vector2.zero);
        enemyRigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        enemyAnimator.SetTrigger("EnemyDeath");
    }
    
    private void Destroy()
    {
        Instantiate(deathParticles, new Vector2(transform.position.x, transform.position.y - 1.25f), Quaternion.identity);

        for (int i = enemyScore; i > 0; i--)
        {
            GameObject soul = Instantiate(soulObject, transform.position, Quaternion.identity);
            soul.transform.position = new Vector2(soul.transform.position.x + Random.Range(-2f, 2f), soul.transform.position.y + Random.Range(-2f, -0.5f));
        }

        Destroy(gameObject);
    }
}
