using UnityEngine;

public class PlayerCollisionController : MonoBehaviour
{
    private PlayerStatsSO playerStats;

    #region Script and Variable References
    public bool isTrapped;

    private Rigidbody2D playerRigidBody;
    private CapsuleCollider2D playerCollider;
    private Animator playerAnimator;
    private PlayerDamageController damageController;
    private PlayerAttachmentController platformAttachment;
    #endregion

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
