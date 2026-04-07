using UnityEngine;
using UnityEngine.Splines;

public class PlayerAnimationController : MonoBehaviour
{
    #region Player Script & Component References

    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody2D playerRigidBody;

    private PlayerMovementController movementController;
    private PlayerJumpController jumpController;
    private PlayerWallController wallController;
    private PlayerDashController dashController;

    private PlayerAttackController attackController;
    #endregion

    private void Start()
    {
        #region Player Script & Component Subscriptions

        playerRigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponentInChildren<Animator>();

        movementController = PlayerController.Instance.movementController;
        jumpController = PlayerController.Instance.jumpController;
        wallController = PlayerController.Instance.wallController;
        dashController = PlayerController.Instance.dashController;
        attackController = PlayerController.Instance.attackController;
        #endregion

        if (jumpController != null)
        {
            jumpController.OnJumpPerformed += HandleJumpAnimation;
        }

        if (attackController != null)
        {
            attackController.OnAttackTriggered += HandleAttackAnimation;
            attackController.OnBulbBounced += HandleJumpAnimation;
        }

        if (dashController != null)
        {
            dashController.OnDashStateChanged += HandleDashAnimation;
        }

        if (wallController != null)
        {
            wallController.OnWallSlideStateChanged += HandleWallSlideAnimation;
            wallController.OnWallJumpStateChanged += HandleWallJumpAnimation;
        }
    }

    private void OnDestroy()
    {
        if (jumpController != null)
        {
            jumpController.OnJumpPerformed -= HandleJumpAnimation;
        }

        if (attackController != null)
        {
            attackController.OnAttackTriggered -= HandleAttackAnimation;
            attackController.OnBulbBounced -= HandleJumpAnimation;
        }

        if (dashController != null)
        {
            dashController.OnDashStateChanged -= HandleDashAnimation;
        }

        if (wallController != null)
        {
            wallController.OnWallSlideStateChanged -= HandleWallSlideAnimation;
            wallController.OnWallJumpStateChanged -= HandleWallJumpAnimation;
        }
    }

    private void Update()
    {
        PlayerAnimation();
    }

    private void HandleJumpAnimation()
    {
        playerAnimator.SetTrigger("PlayerJump");
    }

    private void HandleAttackAnimation(string triggerName)
    {
        playerAnimator.SetTrigger(triggerName);
    }

    private void HandleDashAnimation(bool isDashing)
    {
        playerAnimator.SetBool("isDashing", isDashing);
    }

    private void HandleWallSlideAnimation(bool isSliding)
    {
        playerAnimator.SetBool("isWallSliding", isSliding);
        if (isSliding) playerAnimator.SetBool("isWallJumping", false);
    }

    private void HandleWallJumpAnimation(bool isWallJumping)
    {
        playerAnimator.SetBool("isWallJumping", isWallJumping);
        if (isWallJumping) playerAnimator.SetBool("isWallSliding", false);
    }

    private void PlayerAnimation()
    {
        playerAnimator.SetBool("isGrounded", jumpController.isGrounded);
        playerAnimator.SetBool("isWalled", wallController.isWalled);
        playerAnimator.SetBool("isJumping", jumpController.isJumping);

        if (attackController != null)
        {
            playerAnimator.SetBool("isAttacking", attackController.IsAttacking);
        }

        if (movementController.playerOrientation.x != 0 && jumpController.isGrounded)
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
    }

    public void OnAttackAnimationEnd()
    {
        if (attackController != null)
        {
            attackController.EndAttack();
        }
    }
}