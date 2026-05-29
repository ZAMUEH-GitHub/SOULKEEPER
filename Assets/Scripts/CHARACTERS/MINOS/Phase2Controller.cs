using UnityEngine;

public class Phase2Controller : MonoBehaviour
{
    [Header("Phase 2 Settings")]
    public int smashDamage;
    public int swipeDamage;
    public int grabDamage;
    [Space(5f)]
    public bool isChargingAttack;
    public bool isAttacking;
    public float knockbackForce;
    public float knockbackDuration;
    public float attackRate;
    public float attackTimer;
    [Space(5f)]
    private PlayerController playerController;
    private Transform playerTransform;
    private bool throwRight;

    public enum BossAttack { Smash, Grab, Swipe };
    public BossAttack currentAttack = BossAttack.Smash;

    private MinosController minosController;
    private Animator bossAnimator;

    void Start()
    {
        minosController = GetComponent<MinosController>();
        bossAnimator = GetComponent<Animator>();
        attackTimer = 0f;

        playerController = PlayerController.Instance;
        if (playerController != null)
        {
            playerTransform = playerController.transform;
        }
    }

    public void RunPhase2()
    {
        if (!isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackRate)
            {
                attackTimer = 0f;
                StartAttack();
            }
        }

        UpdateAttackVariation();
    }

    private void StartAttack()
    {
        isAttacking = true;
        isChargingAttack = true;

        ChooseRandomAttack();

        switch (currentAttack)
        {
            case BossAttack.Smash:
                bossAnimator.SetTrigger("Smash");
                Debug.Log("SMASH");
                break;
            case BossAttack.Grab:
                throwRight = Random.value > 0.5f;
                bossAnimator.SetTrigger("Grab");
                bossAnimator.SetBool("Throw Right", throwRight);
                break;
            case BossAttack.Swipe:
                bossAnimator.SetTrigger("Swipe");
                Debug.Log("SWIPE");
                break;
        }
    }

    private void ChooseRandomAttack()
    {
        int attackIndex = Random.Range(0, System.Enum.GetValues(typeof(BossAttack)).Length);
        currentAttack = (BossAttack)attackIndex;
    }

    private void UpdateAttackVariation()
    {
        if (playerTransform == null || playerController == null) return;

        float distanceX = playerTransform.position.x - transform.position.x;
        int positionState = 0;

        if (Mathf.Abs(distanceX) > 2f)
        {
            positionState = distanceX < 0 ? -1 : 1;
        }

        bossAnimator.SetInteger("Player Position", positionState);

        bool isPlayerGrabbed = playerController.GetComponent<PlayerCollisionController>().isTrapped;
        bossAnimator.SetBool("Player Grabbed", isPlayerGrabbed);
    }

    public void ApplyPlayerKnockback()
    {
        if (playerController == null) return;

        float xDirection = throwRight ? 1f : -1f;
        Vector2 knockbackVector = new Vector2(xDirection * 2f, 1f).normalized;

        playerController.damageController.Knockback(knockbackVector, knockbackForce * 2, knockbackDuration);

        var attachment = playerController.GetComponent<PlayerAttachmentController>();
        if (attachment != null)
        {
            attachment.DetachFromPlatform();
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        isChargingAttack = false;
    }
}