using UnityEngine;
using System;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private PlayerStatsSO playerStats;

    public event Action<string> OnAttackTriggered;
    public event Action OnBulbBounced;

    [Header("Attack Settings")]
    public bool isAttacking;
    private bool attackInput;
    private int playerDamage => playerStats != null ? playerStats.damage : 1;
    private float attackRate => playerStats != null ? playerStats.attackRate : 1f;
    private float nextAttack;
    private float comboGrace = 0.2f;
    private float attackTimer;
    private float knockbackForce => playerStats != null ? playerStats.knockback : 5f;
    private float knockbackDuration => playerStats != null ? playerStats.knockbackLenght : 0.2f;

    public AttackState currentAttackState = AttackState.Attack1;
    public enum AttackState { Attack1, Attack2, Attack3 }

    private Vector2 playerOrientation;
    public Vector2 attackOrientation;

    [Header("Thrust & Retract Settings")]
    [SerializeField, Tooltip("Time in seconds to thrust outward to full attackRange.")]
    private float attackExtendDuration = 0.05f;
    [SerializeField, Tooltip("Time in seconds to pull back to the starting position.")]
    private float attackRetractDuration = 0.06f;
    private float currentTravelTime;

    [Header("GameObjects & Scripts")]
    public Transform player;
    public BaseScarfController scarfController, scarfController2;

    [Header("Attack Collider Setup")]
    private Vector2 defaultColliderPosition;
    private Collider2D attackCollider;

    #region Script & Component References
    private PlayerJumpController jumpController;
    private PlayerDamageController damageController;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        attackCollider = GetComponent<Collider2D>();
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }

        defaultColliderPosition = transform.localPosition;
    }

    private void Start()
    {
        var controller = PlayerController.Instance;
        if (controller != null)
        {
            playerStats = controller.playerRuntimeStats;
            jumpController = controller.jumpController;
            damageController = controller.damageController;
        }
        else
        {
            Debug.LogWarning("[PlayerAttackController] PlayerController.Instance is null on Start!");
        }
    }

    private void Update()
    {
        if (attackTimer <= 0)
        {
            currentAttackState = AttackState.Attack1;

            if (isAttacking)
            {
                EndAttack();
            }
        }

        nextAttack = Mathf.Max(0, nextAttack - Time.deltaTime);
        attackTimer = Mathf.Max(0, attackTimer - Time.deltaTime);

        if (isAttacking && playerStats != null)
        {
            currentTravelTime += Time.deltaTime;
            float progress = 0f;

            if (currentTravelTime <= attackExtendDuration)
            {
                progress = Mathf.Clamp01(currentTravelTime / attackExtendDuration);
            }
            else if (currentTravelTime <= attackExtendDuration + attackRetractDuration)
            {
                float retractTime = currentTravelTime - attackExtendDuration;
                progress = 1f - Mathf.Clamp01(retractTime / attackRetractDuration);
            }
            else
            {
                progress = 0f;
                if (attackCollider != null && attackCollider.enabled)
                {
                    attackCollider.enabled = false;
                }
            }

            UpdateAttackColliderPosition(progress);
        }
    }

    public void UpdateAttackState()
    {
        if (PlayerController.Instance == null) return;

        attackInput = PlayerController.Instance.ConsumeAttackInput();
        playerOrientation = PlayerController.Instance.moveVector;
        PlayerAttack(attackInput);
    }
    #endregion

    #region Player Attack Logic
    public void PlayerAttack(bool attackInput)
    {
        if (playerStats != null && playerStats.attackUnlocked && attackInput)
        {
            if (playerOrientation.y >= 0.5f)
            {
                attackOrientation = Vector2.up;
                PlayerUpAttack();
            }
            else if (playerOrientation.y <= -0.5f && jumpController != null && !jumpController.isGrounded)
            {
                attackOrientation = Vector2.down;
                PlayerDownAttack();
            }
            else
            {
                attackOrientation = Vector2.right;
                PlayerSideAttack();
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
        else if (currentAttackState == AttackState.Attack2 && attackTimer > 0 && nextAttack == 0)
        {
            SideAttack2();
        }
        else if (currentAttackState == AttackState.Attack3 && attackTimer > 0 && nextAttack == 0)
        {
            SideAttack3();
        }
    }

    #region Player Side Attacks
    private void SideAttack1()
    {
        isAttacking = true;
        currentTravelTime = 0f;
        if (attackCollider != null) attackCollider.enabled = true;
        OnAttackTriggered?.Invoke("PlayerSideAttack1");
        if (scarfController != null) scarfController.SetAttackInput(attackInput, playerOrientation);

        currentAttackState = AttackState.Attack2;

        attackTimer = attackRate + comboGrace;
        nextAttack = Mathf.Max(0f, attackRate - comboGrace);
    }

    private void SideAttack2()
    {
        isAttacking = true;
        currentTravelTime = 0f;
        if (attackCollider != null) attackCollider.enabled = true;
        OnAttackTriggered?.Invoke("PlayerSideAttack2");
        if (scarfController2 != null) scarfController2.SetAttackInput(attackInput, playerOrientation);

        currentAttackState = AttackState.Attack3;

        attackTimer = attackRate + comboGrace;
        nextAttack = Mathf.Max(0f, attackRate - comboGrace);
    }

    private void SideAttack3()
    {
        isAttacking = true;
        currentTravelTime = 0f;
        if (attackCollider != null) attackCollider.enabled = true;
        OnAttackTriggered?.Invoke("PlayerSideAttack3");
        if (scarfController != null) scarfController.SetAttackInput(attackInput, playerOrientation);
        if (scarfController2 != null) scarfController2.SetAttackInput(attackInput, playerOrientation);

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
            currentTravelTime = 0f;
            if (attackCollider != null) attackCollider.enabled = true;
            OnAttackTriggered?.Invoke("PlayerUpAttack");
            if (scarfController != null) scarfController.SetAttackInput(attackInput, playerOrientation);
            if (scarfController2 != null) scarfController2.SetAttackInput(attackInput, playerOrientation);

            attackTimer = attackRate + comboGrace;
            nextAttack = Mathf.Max(0f, attackRate - comboGrace);
        }
    }

    private void PlayerDownAttack()
    {
        if (nextAttack == 0)
        {
            isAttacking = true;
            currentTravelTime = 0f;
            if (attackCollider != null) attackCollider.enabled = true;
            OnAttackTriggered?.Invoke("PlayerDownAttack");
            if (scarfController != null) scarfController.SetAttackInput(attackInput, playerOrientation);
            if (scarfController2 != null) scarfController2.SetAttackInput(attackInput, playerOrientation);

            attackTimer = attackRate + comboGrace;
            nextAttack = Mathf.Max(0f, attackRate - comboGrace);
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player == null || damageController == null) return;

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
        currentTravelTime = 0f;
        if (attackCollider != null) attackCollider.enabled = false;
        transform.localPosition = defaultColliderPosition;
    }
    #endregion

    public void UpdateAttackColliderPosition(float progress)
    {
        if (playerStats == null) return;

        Vector2 baseOrigin = defaultColliderPosition;

        if (attackOrientation == Vector2.up)
        {
            baseOrigin = new Vector2(0f, Mathf.Abs(defaultColliderPosition.x));
        }
        else if (attackOrientation == Vector2.down)
        {
            baseOrigin = new Vector2(0f, -Mathf.Abs(defaultColliderPosition.x));
        }

        transform.localPosition = baseOrigin + (attackOrientation * (progress * playerStats.attackRange));
    }

    public bool IsAttacking => isAttacking;
}