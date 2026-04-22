using UnityEngine;
using System;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private PlayerStatsSO playerStats;

    public event Action<string> OnAttackTriggered;
    public event Action OnBulbBounced;

    [Header("AttackSettings")]
    public bool isAttacking;
    private bool attackInput;
    private int playerDamage => playerStats.damage;
    private float attackRate => playerStats.attackRate;
    private float nextAttack;
    private float comboGrace = 0.2f;
    private float attackTimer;
    private float knockbackForce => playerStats.knockback;
    private float knockbackDuration => playerStats.knockbackLenght;

    public AttackState currentAttackState = AttackState.Attack1;
    public enum AttackState { Attack1, Attack2, Attack3 }

    private Vector2 playerOrientation;
    public Vector2 attackOrientation;

    [Header("GameObjects & Scripts")]
    public Transform player;
    public BaseScarfController scarfController, scarfController2;

    [Header("Attack Collider Setup")]
    private Vector2 defaultColliderPosition;

    #region Script & Component References
    private PlayerJumpController jumpController;
    private PlayerDamageController damageController;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        #region Script and Variable Suscriptions

        var controller = PlayerController.Instance;
        if (controller != null)
        {
            playerStats = controller.playerRuntimeStats;
        }

        jumpController = PlayerController.Instance.jumpController;
        damageController = PlayerController.Instance.damageController;
        #endregion

        defaultColliderPosition = transform.localPosition;
    }

    void Update()
    {
        if (attackTimer <= 0)
        {
            currentAttackState = AttackState.Attack1;

            if (isAttacking)
            {
                isAttacking = false;
                transform.localPosition = defaultColliderPosition;
            }
        }

        nextAttack = Mathf.Max(0, nextAttack - Time.deltaTime);
        attackTimer = Mathf.Max(0, attackTimer - Time.deltaTime);
    }

    public void UpdateAttackState()
    {
        attackInput = PlayerController.Instance.ConsumeAttackInput();
        playerOrientation = PlayerController.Instance.moveVector;
        PlayerAttack(attackInput);
    }
    #endregion

    #region Player Attack Logic
    public void PlayerAttack(bool attackInput)
    {
        if (playerStats.attackUnlocked)
        {
            if (attackInput && playerOrientation.y == 0)
            {
                attackOrientation = Vector2.right;
                PlayerSideAttack();
            }

            if (attackInput && playerOrientation.y > 0)
            {
                attackOrientation = Vector2.up;
                PlayerUpAttack();
            }

            if (attackInput && playerOrientation.y < 0 && !jumpController.isGrounded)
            {
                attackOrientation = Vector2.down;
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
        isAttacking = true;
        OnAttackTriggered?.Invoke("PlayerSideAttack1");
        scarfController.SetAttackInput(attackInput, playerOrientation);

        currentAttackState = AttackState.Attack2;

        attackTimer = attackRate + comboGrace;
        nextAttack = Mathf.Max(0f, attackRate - comboGrace);
    }

    private void SideAttack2()
    {
        isAttacking = true;
        OnAttackTriggered?.Invoke("PlayerSideAttack2");
        scarfController2.SetAttackInput(attackInput, playerOrientation);

        currentAttackState = AttackState.Attack3;

        attackTimer = attackRate + comboGrace;
        nextAttack = Mathf.Max(0f, attackRate - comboGrace);
    }

    private void SideAttack3()
    {
        isAttacking = true;
        OnAttackTriggered?.Invoke("PlayerSideAttack3");
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
            isAttacking = true;
            OnAttackTriggered?.Invoke("PlayerUpAttack");
            scarfController.SetAttackInput(attackInput, playerOrientation);
            scarfController2.SetAttackInput(attackInput, playerOrientation);

            attackTimer = attackRate + comboGrace;
            nextAttack = Mathf.Max(0f, attackRate - comboGrace);
        }
    }

    private void PlayerDownAttack()
    {
        if (nextAttack == 0)
        {
            isAttacking = true;
            OnAttackTriggered?.Invoke("PlayerDownAttack");
            scarfController.SetAttackInput(attackInput, playerOrientation);
            scarfController2.SetAttackInput(attackInput, playerOrientation);

            attackTimer = attackRate + comboGrace;
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
            else damageController.Knockback(-damageVector, knockbackForce / 2, knockbackDuration);
        }

        if (collision.CompareTag("Bulb"))
        {
            OnBulbBounced?.Invoke();

            if (player.position.y > collision.transform.position.y + 1)
            {
                damageController.Knockback(-damageVector, knockbackForce * 2, knockbackDuration);
            }
            else damageController.Knockback(-damageVector, knockbackForce * 1.5f, knockbackDuration);
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        transform.localPosition = defaultColliderPosition;
    }
    #endregion

    public void UpdateAttackColliderPosition(float progress)
    {
        if (playerStats != null)
        {
            transform.localPosition = defaultColliderPosition + (attackOrientation * (progress * playerStats.attackRange));
        }
    }
    public bool IsAttacking => isAttacking;
}