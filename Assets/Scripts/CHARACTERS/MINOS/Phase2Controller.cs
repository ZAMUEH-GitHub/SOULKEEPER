using UnityEngine;

public class Phase2Controller : MonoBehaviour
{
    [Header("Component References")]
    public Animator bossAnimator;
    public Transform bossCenterPoint;
    private Transform playerTransform;

    [Header("Combat Settings")]
    public float timeBetweenAttacks = 2.0f;
    private float attackCooldownTimer;

    public bool isAttacking;
    public bool phase2Active;

    [Header("Throw Knockback Settings")]
    public float throwKnockbackForce = 15f;
    public float throwKnockbackDuration = 0.5f;

    private void Start()
    {
        if (PlayerController.Instance != null)
        {
            playerTransform = PlayerController.Instance.transform;
        }
        else
        {
            Debug.LogError("[Phase2Controller] PlayerController is NULL on Start!");
        }
    }

    public void EnableCombat()
    {
        Debug.Log("[Phase2Controller] EnableCombat Called! Phase 2 is now active.");
        phase2Active = true;
        attackCooldownTimer = timeBetweenAttacks;
        isAttacking = false;
    }

    private void Update()
    {
        if (!phase2Active || playerTransform == null) return;

        float relativeX = playerTransform.position.x - bossCenterPoint.position.x;
        if (relativeX == 0f) relativeX = 0.01f;
        bossAnimator.SetFloat("PlayerRelativeX", relativeX);

        if (isAttacking) return;

        attackCooldownTimer -= Time.deltaTime;

        if (attackCooldownTimer <= 0)
        {
            ChooseAndExecuteAttack();
        }
    }

    private void ChooseAndExecuteAttack()
    {
        isAttacking = true;
        int attackChoice = Random.Range(0, 3);

        Debug.Log($"[Phase2Controller] Sending Attack Trigger! Choice: {attackChoice}, Player X: {bossAnimator.GetFloat("PlayerRelativeX")}");

        switch (attackChoice)
        {
            case 0:
                bossAnimator.SetTrigger("Swipe");
                break;
            case 1:
                bossAnimator.SetTrigger("Smash");
                break;
            case 2:
                bossAnimator.SetTrigger("Grab");
                break;
        }
    }

    public void ApplyPlayerKnockback()
    {
        if (playerTransform == null) return;

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.UnfreezeAllInputs();
        }

        PlayerAttachmentController attachment = playerTransform.GetComponent<PlayerAttachmentController>();
        if (attachment != null)
        {
            attachment.DetachFromPlatform();
        }

        IKnockbackable knockbackable = playerTransform.GetComponent<IKnockbackable>();
        if (knockbackable != null)
        {
            float directionX = Mathf.Sign(playerTransform.position.x - bossCenterPoint.position.x);
            Vector2 knockbackDir = new Vector2(directionX, 0.5f).normalized;

            knockbackable.Knockback(knockbackDir, throwKnockbackForce, throwKnockbackDuration);
        }
    }

    public void PlayerGrabbed()
    {
        if (bossAnimator != null)
        {
            bossAnimator.SetBool("HasGrabbedPlayer", true);
        }
    }

    public void EndAttack()
    {
        Debug.Log("[Phase2Controller] EndAttack Event Received! Resetting for next attack.");
        isAttacking = false;
        attackCooldownTimer = timeBetweenAttacks;

        if (bossAnimator != null)
        {
            bossAnimator.SetBool("HasGrabbedPlayer", false);
        }
    }
}