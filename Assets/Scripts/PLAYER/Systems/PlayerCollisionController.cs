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
    private CapsuleCollider2D playerCollider;
    private Animator playerAnimator;
    private PlayerDamageController damageController;
    private PlayerAttachmentController platformAttachment;

    private void Awake()
    {
        var controller = GetComponent<PlayerController>();
        if (controller != null)
            playerStats = controller.playerRuntimeStats;

        playerRigidBody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        playerAnimator = GetComponent<Animator>();
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
        if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
            playerCollider.isTrigger = false;

        if (other.CompareTag("MovingObstacle") || other.CompareTag("Parenting Collider"))
        {
            var platformRb = other.attachedRigidbody;
            if (platformAttachment != null)
            {
                if (platformAttachment != null)
                    platformAttachment.AttachToPlatform(other);
            }

            playerCollider.isTrigger = false;
        }

        if (other.CompareTag("Minos Grab Collider"))
        {
            transform.position = other.transform.position;
            playerRigidBody.gravityScale = 0f;
            isTrapped = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("MovingObstacle") || other.CompareTag("Parenting Collider"))
        {
            platformAttachment?.DetachFromPlatform();
            playerCollider.isTrigger = false;
        }

        if (other.CompareTag("Minos Grab Collider"))
        {
            playerCollider.isTrigger = false;
            playerRigidBody.gravityScale = 5f;
            isTrapped = false;
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