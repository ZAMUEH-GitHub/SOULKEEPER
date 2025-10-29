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
    private PlayerCollisionController playerCollisionController;
    private PlayerDamageController playerDamageController;
    private Transform playerTransform;
    public int idleState;
    private bool throwRight;


    public enum BossAttack { Smash, Grab };
    public BossAttack currentAttack = BossAttack.Smash;

    private MinosController minosController;
    private Animator bossAnimator;

    void Start()
    {
        minosController = GetComponent<MinosController>();
        bossAnimator = GetComponent<Animator>();
        attackTimer = 0f;

        playerCollisionController = GetComponent<PlayerCollisionController>();
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

        IdleState();
        UpdateAttackVariation();
    }

    private void IdleState()
    {
        if (minosController.bossHealth > 75)
        {
            idleState = 0;
        }
        else if (minosController.bossHealth > 65)
        {
            idleState = 1;
        }
        else
        {
            idleState = 2;
        }

        bossAnimator.SetInteger("Idle State", idleState);
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
            /*case BossAttack.Swipe:
                bossAnimator.SetTrigger("Swipe");
                Debug.Log("SWIPE");
                break;*/
            case BossAttack.Grab:
                throwRight = Random.value > 0.5f;
                bossAnimator.SetTrigger("Grab");
                bossAnimator.SetBool("Throw Right", throwRight);
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
        /*if (currentAttack == BossAttack.Swipe)
        {
            float playerY = player.transform.position.y;
            bossAnimator.SetFloat("Player Vertical", playerY);
        }*/
        /* else
         {*/
        float playerX = playerTransform.position.x;
        bossAnimator.SetFloat("Player Horizontal", playerX);
        //}

        bossAnimator.SetBool("Player Grabbed", playerCollisionController.isTrapped);
    }

public void ApplyPlayerKnockback()
{
    float xDirection = throwRight ? 1f : -1f;
    Vector2 knockbackVector = new Vector2(xDirection * 2f, 1f).normalized;
    playerDamageController.Knockback(knockbackVector, knockbackForce*2, knockbackDuration);
}

    public void EndAttack()
    {
        isAttacking = false;
        isChargingAttack = false;
    }
}
