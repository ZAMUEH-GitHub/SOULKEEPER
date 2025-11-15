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
    #endregion

    private void Start()
    {
        #region Player Script & Component Subscriptions

        playerRigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();

        movementController = GetComponent<PlayerMovementController>();
        jumpController = GetComponent<PlayerJumpController>();
        wallController = GetComponent<PlayerWallController>();
        dashController = GetComponent<PlayerDashController>();

        #endregion
    }

    private void Update()
    {
        PlayerAnimation();
    }

    private void PlayerAnimation()
    {
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
}
