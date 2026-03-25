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
    private bool wasMovingLastFrame;
    private EnemyBaseController enemy;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        enemy = GetComponent<EnemyBaseController>();
    }

    private void Update()
    {
        UpdateMovementParameters();
        HandleIdleEntry();
    }

    private void UpdateMovementParameters()
    {
        if (animator == null || enemy.rigidBody == null) return;

        float currentSpeed = Mathf.Abs(enemy.rigidBody.linearVelocity.x);

        animator.SetFloat("Speed", currentSpeed);

        bool isMoving = currentSpeed > 0.05f;

        float walkThreshold = (enemy.enemyStats != null) ? enemy.enemyStats.speed + 0.2f : 2f;
        bool isRunning = currentSpeed >= walkThreshold;
        bool isWalking = isMoving && !isRunning;

        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsRunning", isRunning);

        wasMovingLastFrame = isMoving;
    }

    private void HandleIdleEntry()
    {
        if (animator == null) return;

        bool isMoving = animator.GetBool("IsMoving");

        if (idleRollConsumed)
        {
            wasMovingLastFrame = isMoving;
            return;
        }

        if (!isMoving && wasMovingLastFrame)
        {
            if (Random.value <= idleBreakChance)
            {
                int variant = Random.Range(idleVariantMin, idleVariantMax + 1);
                animator.SetInteger("IdleVariant", variant);
                animator.ResetTrigger("IdleBreak");
                animator.SetTrigger("IdleBreak");
            }
            else
            {
                animator.SetInteger("IdleVariant", 1);
                animator.ResetTrigger("IdleBreak");
            }

            idleRollConsumed = true;
        }

        wasMovingLastFrame = isMoving;
    }

    public void ForceIdleBreak()
    {
        if (animator == null) return;

        int variant = Random.Range(idleVariantMin, idleVariantMax + 1);

        animator.SetInteger("IdleVariant", variant);
        animator.ResetTrigger("IdleBreak");
        animator.SetTrigger("IdleBreak");

        idleRollConsumed = true;
    }

    public void TriggerAlert()
    {
        if (animator == null) return;
        animator.ResetTrigger("Alert");
        animator.SetTrigger("Alert");
    }

    public void OnAlertAnimationEnd()
    {
        OnAlertAnimationEndEvent?.Invoke();
    }

    public void SetIsJumping(bool value)
    {
        if (animator == null) return;

        bool current = animator.GetBool("IsJumping");
        if (current != value)
        {
            animator.SetBool("IsJumping", value);
        }
    }

    public void TriggerHit()
    {
        if (animator == null) return;
        animator.ResetTrigger("Hit");
        animator.SetTrigger("Hit");
    }

    public void TriggerAttack()
    {
        if (animator == null) return;
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");
    }

    public void SetIsAttacking(bool isAttacking)
    {
        if (animator == null) return;
        animator.SetBool("IsAttacking", isAttacking);
    }
}