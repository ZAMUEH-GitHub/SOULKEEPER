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
    private EnemyAnimationController animController;

    private float jumpForce;
    private float jumpRate;

    #region Unity Lifecycle
    private void Awake()
    {
        enemyRB = GetComponent<Rigidbody2D>();
        animController = GetComponent<EnemyAnimationController>();

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

        if (animController != null)
            animController.SetGrounded(isGrounded);

        nextJump = Mathf.Max(0, nextJump - Time.deltaTime);
    }
    #endregion

    #region Jump Logic
    public void DoJump()
    {
        enemyRB.linearVelocity = new Vector2(enemyRB.linearVelocity.x, jumpForce);
        nextJump = jumpRate;

        if (animController != null)
            animController.TriggerEnemyJump();
    }

    public bool CanJump => isGrounded && nextJump <= 0;

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }
    #endregion
}