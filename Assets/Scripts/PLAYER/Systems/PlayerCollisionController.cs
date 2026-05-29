using UnityEngine;

public class PlayerCollisionController : MonoBehaviour
{
    private PlayerStatsSO playerStats;

    [Header("Environment Checks")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public bool isGrounded;

    [Space(5)]
    public Transform wallCheckPoint;
    public float wallCheckRadius = 0.2f;
    public LayerMask wallLayer;
    public bool isWalled;

    [Header("State Flags")]
    public bool isTrapped;

    private Rigidbody2D playerRigidBody;
    private Animator playerAnimator;
    private PlayerDamageController damageController;
    private PlayerAttachmentController platformAttachment;

    private void Awake()
    {
        var controller = GetComponent<PlayerController>();
        if (controller != null)
            playerStats = controller.playerRuntimeStats;

        playerRigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponentInChildren<Animator>();
        damageController = GetComponent<PlayerDamageController>();
        platformAttachment = GetComponent<PlayerAttachmentController>();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        isWalled = Physics2D.OverlapCircle(wallCheckPoint.position, wallCheckRadius, wallLayer);
    }

    #region Collision Management
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("MovingObstacle") || other.CompareTag("Parenting Collider"))
        {
            var platformRb = other.attachedRigidbody;
            if (platformAttachment != null)
            {
                platformAttachment.AttachToPlatform(other);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("MovingObstacle") || other.CompareTag("Parenting Collider"))
        {
            platformAttachment?.DetachFromPlatform();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 collisionVector = (transform.position - collision.transform.position).normalized;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            damageController.TakeDamage(1, collisionVector);
            damageController.Knockback(collisionVector, playerStats.damageForce, playerStats.damageLenght);
        }

        if (collision.gameObject.CompareTag("Bulb"))
            playerAnimator.SetTrigger("PlayerJump");
    }
    #endregion
}