using UnityEngine;

[RequireComponent(typeof(EnemyBaseController))]
public class EnemyAnimationController : MonoBehaviour
{
    [Header("Animation Settings")]
    public Animator animator;

    [Header("Idle Variants")]
    public float idleBreakChance = 0.5f;
    public int idleVariantMin = 2;
    public int idleVariantMax = 3;

    [HideInInspector] public bool idleRollConsumed;

    public event System.Action OnAlertAnimationEndEvent;
    private EnemyBaseController enemy;
    private bool wasMovingLastFrame;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        enemy = GetComponent<EnemyBaseController>();
    }

    #region Movement Logic
    public void SetMoving(bool isMoving)
    {
        if (animator == null) return;

        animator.SetBool("IsMoving", isMoving);
        HandleIdleEntry(isMoving);
    }

    public void SetWalkSpeed(float speed)
    {
        if (animator != null) animator.SetFloat("Speed", speed);
    }

    public void SetGrounded(bool isGrounded)
    {
        if (animator != null) animator.SetBool("IsGrounded", isGrounded);
    }
    #endregion

    #region Attack Logic
    public void TriggerEnemyAttack()
    {
        if (animator != null) animator.SetTrigger("Attack");
    }

    public void SetAttacking(bool isAttacking)
    {
        if (animator != null) animator.SetBool("IsAttacking", isAttacking);
    }

    public void SetChargingAttack(bool isCharging)
    {
        if (animator != null) animator.SetBool("IsChargingAttack", isCharging);
    }

    public void TriggerComboAttack(int comboStep)
    {
        if (animator != null) animator.SetTrigger("Attack" + comboStep);
    }
    #endregion

    #region Airborne Logic
    public void SetChargingJump(bool isCharging)
    {
        if (animator != null) animator.SetBool("IsChargingJump", isCharging);
    }

    public void TriggerEnemyJump()
    {
        if (animator != null) animator.SetTrigger("Jump");
    }

    public void SetJumping(bool isJumping)
    {
        if (animator != null) animator.SetBool("IsJumping", isJumping);
    }

    public void SetFalling(bool isFalling)
    {
        if (animator != null) animator.SetBool("IsFalling", isFalling);
    }
    #endregion

    #region Alert Logic
    public void TriggerAlert()
    {
        if (animator != null) animator.SetTrigger("Alert");
    }
    public void OnAlertAnimationEnd()
    {
        OnAlertAnimationEndEvent?.Invoke();
    }
    #endregion

    #region Idle Logic
    private void HandleIdleEntry(bool isCurrentlyMoving)
    {
        if (animator == null) return;

        if (idleRollConsumed)
        {
            wasMovingLastFrame = isCurrentlyMoving;
            return;
        }

        if (!isCurrentlyMoving && wasMovingLastFrame)
        {
            if (Random.value <= idleBreakChance)
            {
                int variant = Random.Range(idleVariantMin, idleVariantMax + 1);
                animator.SetInteger("IdleVariant", variant);
                animator.SetTrigger("IdleBreak");
            }
            else
            {
                animator.SetInteger("IdleVariant", 1);
                animator.ResetTrigger("IdleBreak");
            }

            idleRollConsumed = true;
        }
        else if (isCurrentlyMoving)
        {
            idleRollConsumed = false;
        }

        wasMovingLastFrame = isCurrentlyMoving;
    }

    public void ForceIdleBreak()
    {
        if (animator == null) return;

        int variant = Random.Range(idleVariantMin, idleVariantMax + 1);
        animator.SetInteger("IdleVariant", variant);
        animator.SetTrigger("IdleBreak");

        idleRollConsumed = true;
    }
    #endregion

    #region Damage & Death Logic
    public void TriggerHit()
    {
        if (animator != null) animator.SetTrigger("Hit");
    }
    public void TriggerDeath()
    {
        if (animator != null) animator.SetTrigger("Death");
    }

    public void SetStunned(bool isStunned)
    {
        if (animator != null) animator.SetBool("IsStunned", isStunned);
    }

    public void SetImpacting(bool isImpacting)
    {
        if (animator != null) animator.SetBool("IsImpacting", isImpacting);
    }
    #endregion
}