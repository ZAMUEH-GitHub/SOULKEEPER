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

    [Header("Throw Vectors")]
    public Vector2 throwRightVector = new Vector2(1f, 0.5f).normalized;
    public Vector2 throwLeftVector = new Vector2(-1f, 0.5f).normalized;

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
        bossAnimator = GetComponentInChildren<Animator>();
        attackTimer = 0f;

        playerController = PlayerController.Instance;
        if (playerController != null)
        {
            playerTransform = playerController.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null || bossAnimator == null) return;

        float relativeX = playerTransform.position.x - transform.position.x;

        bossAnimator.SetFloat("Player Position", relativeX);

        if (playerController != null)
        {
            bool isPlayerGrabbed = playerController.GetComponent<PlayerCollisionController>().isTrapped;
            bossAnimator.SetBool("Player Grabbed", isPlayerGrabbed);
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

                bossAnimator.SetBool("Throw Right", throwRight);
                bossAnimator.SetBool("Throw Left", !throwRight);

                bossAnimator.SetTrigger("Grab");
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

    public void ApplyPlayerKnockback()
    {
        if (playerController == null) return;

        var collisionController = playerController.GetComponent<PlayerCollisionController>();
        if (collisionController == null || !collisionController.isTrapped) return;

        Vector2 knockbackVector = throwRight ? throwRightVector : throwLeftVector;

        playerController.damageController.Knockback(knockbackVector, knockbackForce * 2, knockbackDuration);
    }

    public void EndAttack()
    {
        isAttacking = false;
        isChargingAttack = false;

        if (bossAnimator != null)
        {
            bossAnimator.SetBool("Throw Right", false);
            bossAnimator.SetBool("Throw Left", false);
        }
    }
}