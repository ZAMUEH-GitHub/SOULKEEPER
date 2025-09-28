using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

#region Require Component
[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerJumpController))]
[RequireComponent(typeof(PlayerWallController))]
[RequireComponent(typeof(PlayerDamageController))]
[RequireComponent(typeof(PlayerDashController))]
#endregion

public class PlayerController : Singleton<PlayerController>
{
    public PlayerStatsSO playerStats;

    [Header("Player Input")]
    public Vector2 moveVector;
    public bool moveInput;
    public bool jumpInput;
    public bool dashInput;
    public bool attackInput;
    [Space(5)]
    public int playerHealth;
    public bool isAlive = true;
    public int playerScore;
    public bool isTrapped;

    #region Player Script & Component References

    private Rigidbody2D playerRigidBody;
    private CapsuleCollider2D playerCollider;
    private Animator playerAnimator;
    private SpriteRenderer playerSprite;

    private PlayerMovementController movementController;
    private PlayerJumpController jumpController;
    private PlayerWallController wallController;
    private PlayerDashController dashController;
    private PlayerAttackController attackController;
    private PlayerDamageController damageController;

    #endregion

    protected override void Awake()
    {
        base.Awake();
        playerHealth = playerStats.health;
        playerScore = playerStats.score;
    }

    void Start()
    {
        #region Player Script & Component Subscriptions

        playerRigidBody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        playerAnimator = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();

        movementController = GetComponent<PlayerMovementController>();
        jumpController = GetComponent<PlayerJumpController>();
        wallController = GetComponent<PlayerWallController>();
        dashController = GetComponent<PlayerDashController>();
        attackController = GetComponentInChildren<PlayerAttackController>();
        damageController = GetComponent<PlayerDamageController>();
        #endregion

        isAlive = true;
    }

    void Update()
    {
        if (!isAlive) { return; }
        
        movementController.SetMoveInput(moveVector, moveInput);
        wallController.SetWallInput(moveVector, moveInput);
        dashController.SetDashInput(moveVector, dashInput);
        attackController.SetAttackInput(attackInput, moveVector);

        if (jumpInput)
        {
            if (wallController.IsWallSliding)
            {
                wallController.SetWallJumpInput(jumpInput);
            }
            else
            {
                jumpController.SetJumpInput(jumpInput);
            }

            jumpInput = false;
        }

        if (dashInput)
        {
            dashInput = false;
        }

        if (attackInput)
        {
            attackInput = false;
        }

        PlayerAnimation();
    }

    public void PlayerInputMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveVector = context.ReadValue<Vector2>();
            moveInput = true;
        }
        else if (context.canceled)
        {
            moveVector = Vector2.zero;
            moveInput = false;
        }
    }

    public void PlayerInputJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpInput = true;
        }
    }

    public void PlayerInputDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            dashInput = true;
        }
    }
    
    public void PlayerInputAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            attackInput = true;
        }
    }

    private void PlayerAnimation()
    {
        if (moveVector.x != 0 && jumpController.isGrounded)
        {
            playerAnimator.SetBool("isMoving", true);
            playerAnimator.SetBool("isWallSliding", false);
        }
        else { playerAnimator.SetBool("isMoving", false); }

        if (!jumpController.isGrounded && playerRigidBody.linearVelocityY < -1 && !wallController.IsWallSliding)
        {
            playerAnimator.SetBool("isFalling", true);
            playerAnimator.SetBool("isWallSliding", false);
        }
        else { playerAnimator.SetBool("isFalling", false); }

        if (dashController.isDashing)
        {
            playerAnimator.SetBool("isDashing", true);
        }
        else { playerAnimator.SetBool("isDashing", false); }

        if (wallController.IsWallSliding)
        {
            playerAnimator.SetBool("isWallSliding", true);
            playerAnimator.SetBool("PlayerWallJump", false);
        }
        else if (wallController.IsWallJumping)
        {
            playerAnimator.SetBool("PlayerWallJump", true);
            playerAnimator.SetBool("isWallSliding", false);
        }
    }

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
            isTrapped = false;
            playerRigidBody.gravityScale = 5f;
            playerCollider.isTrigger = false;

            DontDestroyOnLoad(gameObject);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 knockbackVector = (collision.transform.position - transform.position).normalized;

        if (collision.gameObject.CompareTag("Bulb"))
        {
            playerAnimator.SetTrigger("PlayerJump");
        }
    }
}