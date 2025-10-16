using UnityEngine;

public class EnemyJumpController : MonoBehaviour
{
    [Header("Stats Reference")]
    public EnemyStatsSO enemyStats;

    [Header("Ground Check (OverlapCircle)")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public bool isGrounded;
    [HideInInspector] public float nextJump;

    private Rigidbody2D enemyRB;
    private Animator animator;

    private float jumpForce;
    private float jumpRate;

    private void Awake()
    {
        enemyRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (enemyStats != null)
        {
            jumpForce = enemyStats.jumpForce;
            jumpRate = enemyStats.jumpRate;
        }
    }

    private void Update()
    {
        if (groundCheckPoint != null)
            isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        nextJump = Mathf.Max(0, nextJump - Time.deltaTime);
    }


    public void DoJump()
    {
        enemyRB.linearVelocity = new Vector2(enemyRB.linearVelocity.x, jumpForce);
        nextJump = jumpRate;

        if (animator != null)
            animator.SetTrigger("EnemyJump");
    }

    public bool CanJump => isGrounded && nextJump <= 0;

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }
}
