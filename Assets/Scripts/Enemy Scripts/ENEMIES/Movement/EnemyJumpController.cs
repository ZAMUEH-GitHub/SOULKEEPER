using UnityEngine;
using System.Collections;

public class EnemyJumpController : MonoBehaviour
{
    public EnemyStatsSO enemyStats;
    
    [Header("Jump Settings")]
    public bool isGrounded;
    private float jumpForce;
    private float jumpRate;
    private float nextJump;
    private float jumpChargeDuration = 1;
    public bool isChargingJump;

    [Header("Ground Check (OverlapCircle)")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;

    public LayerMask groundLayer;
    private Animator animator;
    private Rigidbody2D enemyRB;

    private EnemyWallController wallController;
    private EnemyDamageController damageController;
    private EnemyAttackController attackController;

    private void Awake()
    {
        wallController = GetComponent<EnemyWallController>();
        damageController = GetComponent<EnemyDamageController>();
        attackController = GetComponentInChildren<EnemyAttackController>();

        animator = GetComponent<Animator>();
        enemyRB = GetComponent<Rigidbody2D>();

        jumpForce = enemyStats.jumpForce;
        jumpRate = enemyStats.jumpRate;

    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        nextJump = Mathf.Max(0, nextJump - Time.deltaTime);
    }

    public void EnemyJump(bool isAlive, Vector2 currentTarget)
    {
        if (!isAlive || damageController.isKnockedBack || attackController.isChargingAttack || attackController.isAttacking) return;

        if (isGrounded && !isChargingJump && currentTarget.y > transform.position.y + 2 && nextJump <= 0)
        {
            StartCoroutine(JumpCharge());
            isChargingJump = true;
        }
    }

    private IEnumerator JumpCharge()
    {
        yield return new WaitForSeconds(jumpChargeDuration);
        enemyRB.linearVelocity = new Vector2(enemyRB.linearVelocityX, jumpForce);
        isChargingJump = false;
        nextJump = jumpRate;
        animator.SetTrigger("EnemyJump");
    }
    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }
}
