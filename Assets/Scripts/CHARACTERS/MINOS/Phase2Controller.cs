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
    private bool isAttacking;
    private bool phase2Active;

    [Header("Throw Knockback Settings")]
    public float throwKnockbackForce = 15f;
    public float throwKnockbackDuration = 0.5f;

    private void Start()
    {
        if (PlayerController.Instance != null)
        {
            playerTransform = PlayerController.Instance.transform;
        }
    }

    public void EnableCombat()
    {
        phase2Active = true;
        attackCooldownTimer = timeBetweenAttacks;
    }

    private void Update()
    {
        if (!phase2Active || isAttacking || playerTransform == null) return;

        attackCooldownTimer -= Time.deltaTime;

        if (attackCooldownTimer <= 0)
        {
            ChooseAndExecuteAttack();
        }
    }

    private void ChooseAndExecuteAttack()
    {
        isAttacking = true;

        float relativeX = playerTransform.position.x - bossCenterPoint.position.x;
        bossAnimator.SetFloat("PlayerRelativeX", relativeX);

        int attackChoice = Random.Range(0, 3);

        switch (attackChoice)
        {
            case 0:
                bossAnimator.SetTrigger("Swipe");
                break;
            case 1:
                bossAnimator.SetTrigger("Smash_Charge");
                break;
            case 2:
                bossAnimator.SetTrigger("Grab_Charge");
                break;
        }
    }

    public void ApplyPlayerKnockback()
    {
        if (playerTransform == null) return;

        IKnockbackable knockbackable = playerTransform.GetComponent<IKnockbackable>();
        if (knockbackable != null)
        {
            float directionX = Mathf.Sign(playerTransform.position.x - bossCenterPoint.position.x);
            Vector2 knockbackDir = new Vector2(directionX, 0.5f).normalized;

            knockbackable.Knockback(knockbackDir, throwKnockbackForce, throwKnockbackDuration);
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        attackCooldownTimer = timeBetweenAttacks;
    }
}