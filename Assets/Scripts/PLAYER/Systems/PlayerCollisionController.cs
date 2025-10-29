using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCollisionController : MonoBehaviour
{
    public PlayerStatsSO playerStats;

    #region Script and Variable References

    public bool isTrapped;
    
    private Rigidbody2D playerRigidBody;
    private CapsuleCollider2D playerCollider;
    private Animator playerAnimator;

    private PlayerDamageController damageController;
    #endregion

    private void Start()
    {
        #region Script and Variable Subscriptions

        playerRigidBody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        playerAnimator = GetComponent<Animator>();

        damageController = GetComponent<PlayerDamageController>();
        #endregion
    }

    #region Collision Management

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other != null && other.CompareTag("MovingObstacle") || other != null && other.CompareTag("Parenting Collider"))
        {
            transform.SetParent(other.transform, true);
            playerCollider.isTrigger = false;
        }

        if (other != null && other.CompareTag("Minos Grab Collider"))
        {
            transform.SetParent(other.transform, true);
            transform.position = other.transform.position;
            playerRigidBody.gravityScale = 0f;
            isTrapped = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other != null && other.CompareTag("MovingObstacle") || other != null && other.CompareTag("Parenting Collider"))
        {
            transform.SetParent(null);
            playerCollider.isTrigger = false;

            DontDestroyOnLoad(gameObject);
        }

        if (other != null && other.CompareTag("Minos Grab Collider"))
        {
            transform.SetParent(null);
            playerCollider.isTrigger = false;
            playerRigidBody.gravityScale = 5f;
            isTrapped = false;

            DontDestroyOnLoad(gameObject);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 collisionVector = (transform.position - collision.transform.position).normalized;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            damageController.TakeDamage(1, collisionVector);
            damageController.Knockback(collisionVector, playerStats.damageForce, playerStats.damageLenght);
        }

        if (collision.gameObject.CompareTag("Bulb"))
        {
            playerAnimator.SetTrigger("PlayerJump");
        }
    }

    #endregion
}
