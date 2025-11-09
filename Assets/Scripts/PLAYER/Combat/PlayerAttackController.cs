using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    private PlayerStatsSO playerStats;

    [Header("AttackSettings")]
    public bool isAttacking;
    private bool attackInput;
    private int playerDamage;
    private float attackRate;
    private float nextAttack;
    private float comboGrace = 0.2f;
    private float attackTimer;
    private float knockbackForce;
    private float knockbackDuration;

    public AttackState currentAttackState = AttackState.Attack1;
    public enum AttackState { Attack1, Attack2, Attack3 }

    private Vector2 playerOrientation;
    public Vector2 attackOrientation;

    [Header("GameObjects & Scripts")]
    public Transform player;
    public BaseScarfController scarfController, scarfController2;

    #region Script & Component References
    private PlayerJumpController jumpController;
    private PlayerWallController wallController;
    private PlayerDashController dashController;
    private PlayerDamageController damageController;

    private Animator playerAnimator;

    #endregion

    private void Awake()
    {
        #region Script and Variable Suscriptions

        playerStats = GameManager.RuntimePlayerStats;
        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<PlayerController>()?.playerBaseStats;
        }

        jumpController = GetComponentInParent< PlayerJumpController>();
        wallController = GetComponentInParent< PlayerWallController>();
        dashController = GetComponentInParent< PlayerDashController>();
        damageController = GetComponentInParent< PlayerDamageController>();
        playerAnimator = GetComponentInParent<Animator>();
        
        playerDamage = playerStats.damage;
        knockbackForce = playerStats.knockback;
        attackRate = playerStats.attackRate;
        knockbackForce = playerStats.knockback;
        knockbackDuration = playerStats.knockbackLenght;
        #endregion
    }

    void Update()
    {
        if (attackTimer <= 0)
        {
            currentAttackState = AttackState.Attack1;
            isAttacking = false;
        }

        nextAttack = Mathf.Max(0, nextAttack - Time.deltaTime);
        attackTimer = Mathf.Max(0, attackTimer - Time.deltaTime);
    }

    public void SetAttackInput(bool attackInput, Vector2 moveVector)
    {
        this.attackInput = attackInput;
        playerOrientation = moveVector;
        PlayerAttack(attackInput);
    }

    public void PlayerAttack(bool attackInput)
    {
        if (playerStats.attackUnlocked)
        
            if (!dashController.isDashing && !wallController.isWallSliding)
            {
                if (attackInput && playerOrientation.y == 0)
                {
                    attackOrientation = (player.localScale.x > 0) ? Vector2.right : Vector2.left;
                    isAttacking = true;
                    PlayerSideAttack();
                }

                if (attackInput && playerOrientation.y > 0)
                {
                    attackOrientation = Vector2.up;
                    isAttacking = true;
                    PlayerUpAttack();
                }

                if (attackInput && playerOrientation.y < 0 && !jumpController.isGrounded)
                {
                    attackOrientation = Vector2.down;
                    isAttacking = true;
                    PlayerDownAttack();
                }
            }
    }

    #region Player Attacks

    private void PlayerSideAttack()
    {
        if (currentAttackState == AttackState.Attack1 && nextAttack == 0)
        {
            SideAttack1();
        }

        if (currentAttackState == AttackState.Attack2 && attackTimer > 0 && nextAttack == 0)
        {
            SideAttack2();
        }

        if (currentAttackState == AttackState.Attack3 && attackTimer > 0 && nextAttack == 0)
        {
            SideAttack3();
        }
    }

        #region Player Side Attacks
        private void SideAttack1()
        {         
            playerAnimator.SetTrigger("PlayerSideAttack1");
            scarfController.SetAttackInput(attackInput, playerOrientation);

            currentAttackState = AttackState.Attack2;

            attackTimer = attackRate + comboGrace;
            nextAttack = Mathf.Max(0f, attackRate - comboGrace);
        }

        private void SideAttack2()
        {
            playerAnimator.SetTrigger("PlayerSideAttack2");
            scarfController2.SetAttackInput(attackInput, playerOrientation);
            
            currentAttackState = AttackState.Attack3;

            attackTimer = attackRate + comboGrace;
            nextAttack = Mathf.Max(0f, attackRate - comboGrace);
        }

        private void SideAttack3()
        {
            playerAnimator.SetTrigger("PlayerSideAttack3");
            scarfController.SetAttackInput(attackInput, playerOrientation);
            scarfController2.SetAttackInput(attackInput, playerOrientation);

            currentAttackState = AttackState.Attack1;

            attackTimer = comboGrace;
            nextAttack = Mathf.Max(0f, attackRate - comboGrace);
        }
    #endregion

    private void PlayerUpAttack()
    {
        if (nextAttack == 0)
        {
            playerAnimator.SetTrigger("PlayerUpAttack");
            scarfController.SetAttackInput(attackInput, playerOrientation);
            scarfController2.SetAttackInput(attackInput, playerOrientation);

            nextAttack = Mathf.Max(0f, attackRate - comboGrace);
        }
    }

    private void PlayerDownAttack()
    {
        if (nextAttack == 0)
        {
            playerAnimator.SetTrigger("PlayerDownAttack");
            scarfController.SetAttackInput(attackInput, playerOrientation);
            scarfController2.SetAttackInput(attackInput, playerOrientation);

            nextAttack = Mathf.Max(0f, attackRate - comboGrace);
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        IKnockbackable knockbackable = collision.GetComponent<IKnockbackable>();

        Vector2 damageVector = (collision.transform.position - player.transform.position).normalized;

        if (damageable != null)
            damageable.TakeDamage(playerDamage, damageVector);

        if (knockbackable != null)
            knockbackable.Knockback(damageVector, knockbackForce, knockbackDuration);

        if (collision.CompareTag("Enemy") || collision.CompareTag("Spike"))
        {
            if (player.position.y > collision.transform.position.y + 1)
            {
                damageController.Knockback(-damageVector, knockbackForce, knockbackDuration);
            }
            else damageController.Knockback(-damageVector, knockbackForce/2 , knockbackDuration);
        }

        if (collision.CompareTag("Bulb"))
        {
            if (player.position.y > collision.transform.position.y + 1)
            {
                damageController.Knockback(-damageVector, knockbackForce * 2, knockbackDuration);
            }
            else damageController.Knockback(-damageVector, knockbackForce, knockbackDuration);
        }
    }

    public bool IsAttacking => isAttacking;
}