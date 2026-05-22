using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;

    private PlayerAttackController attackController;
    private PlayerJumpController jumpController;
    private PlayerWallController wallController;

    private void Start()
    {
        playerAnimator = GetComponentInChildren<Animator>();

        attackController = PlayerController.Instance.attackController;
        jumpController = PlayerController.Instance.jumpController;
        wallController = PlayerController.Instance.wallController;

        if (jumpController != null)
            jumpController.OnJumpPerformed += HandleJumpAnimation;

        if (attackController != null)
        {
            attackController.OnAttackTriggered += HandleAttackAnimation;
            attackController.OnBulbBounced += HandleJumpAnimation;
        }

        if (wallController != null)
            wallController.OnWallJumpStateChanged += HandleWallJumpAnimation;
    }

    private void OnDestroy()
    {
        if (jumpController != null)
            jumpController.OnJumpPerformed -= HandleJumpAnimation;

        if (attackController != null)
        {
            attackController.OnAttackTriggered -= HandleAttackAnimation;
            attackController.OnBulbBounced -= HandleJumpAnimation;
        }

        if (wallController != null)
            wallController.OnWallJumpStateChanged -= HandleWallJumpAnimation;
    }

    public void SetBool(string paramName, bool value)
    {
        if (playerAnimator != null) playerAnimator.SetBool(paramName, value);
    }

    private void HandleJumpAnimation() => playerAnimator.SetTrigger("PlayerJump");
    private void HandleAttackAnimation(string triggerName) => playerAnimator.SetTrigger(triggerName);
    private void HandleWallJumpAnimation(bool isWallJumping) => playerAnimator.SetBool("isWallJumping", isWallJumping);

    public void OnAttackAnimationEnd()
    {
        if (attackController != null) attackController.EndAttack();
    }
}