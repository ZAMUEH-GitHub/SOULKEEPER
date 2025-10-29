using UnityEngine;

public class EnemyBaseController : MonoBehaviour, IEnemy
{
    [Header("Debug States")]
    [SerializeField] private string currentMovementState;
    [SerializeField] private string currentVerticalState;

    [Header("Stats & References")]
    public EnemyStatsSO enemyStats;
    public Transform patrolTarget;

    #region Cached References
    [HideInInspector] public Rigidbody2D rigidBody { get; private set; }
    [HideInInspector] public Animator animator { get; private set; }
    [HideInInspector] public EnemyJumpController jumpController { get; private set; }
    [HideInInspector] public EnemyAttackController attackController { get; private set; }
    [HideInInspector] public EnemyDamageController damageController { get; private set; }
    [HideInInspector] public Transform player { get; private set; }
    #endregion

    [Header("Runtime Variables")]
    public bool isAlive = true;
    public bool isDead = false;
    public bool targetPlayer = false;
    public Vector2 patrolStart;
    public Vector2 currentTarget;
    public float enemySpeed;

    public bool canRunMovementState = true;
    public bool canRunVerticalState = true;

    public EnemyMovementStateMachine movementStateMachine;
    public EnemyVerticalStateMachine verticalStateMachine;

    void Start()
    {
        #region Script and Component Subscription

        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        jumpController = GetComponent<EnemyJumpController>();
        attackController = GetComponentInChildren<EnemyAttackController>();
        damageController = GetComponent<EnemyDamageController>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        enemySpeed = enemyStats.speed;
        patrolStart = transform.position;
        currentTarget = patrolTarget != null ? patrolTarget.position : transform.position;
        #endregion

        movementStateMachine = new EnemyMovementStateMachine();
        verticalStateMachine = new EnemyVerticalStateMachine();

        movementStateMachine.Initialize(new IdleState(this));
        verticalStateMachine.Initialize(null);
    }

    void Update()
    {
        if (!isAlive || isDead) return;

        if (canRunMovementState)
            movementStateMachine?.Update();

        if (canRunVerticalState)
            verticalStateMachine?.Update();

        currentMovementState = movementStateMachine?.CurrentStateName;
        currentVerticalState = verticalStateMachine?.CurrentStateName;
    }

    public void PauseMovement(bool pause)
    {
        canRunMovementState = !pause;
    }

    public void PauseVertical(bool pause)
    {
        canRunVerticalState = !pause;

        if (!pause)
            ResetVerticalStateIfGrounded();
    }

    public bool IsInVerticalAction()
    {
        if (verticalStateMachine == null || verticalStateMachine.CurrentState == null)
            return false;

        string stateName = verticalStateMachine.CurrentStateName;

        return stateName == nameof(JumpChargeState)
            || stateName == nameof(JumpState)
            || stateName == nameof(FallingState);
    }

    public void ResetVerticalStateIfGrounded()
    {
        if (jumpController != null && jumpController.isGrounded)
        {
            verticalStateMachine.ChangeState(null);
            animator.SetBool("isFalling", false);
        }
    }

    public void ChangeMovementState(IMovementState newState) => movementStateMachine.ChangeState(newState);

    public void ChangeVerticalState(IVerticalState newState) => verticalStateMachine.ChangeState(newState);

    public void SetTargetPlayer(bool target) => targetPlayer = target;

    public void MoveToTarget(Vector2 target, float speedOverride = -1f)
    {
        if (damageController != null && damageController.isKnockedBack) return;

        float speed = (speedOverride > 0) ? speedOverride : enemyStats.speed;
        float direction = Mathf.Sign(target.x - transform.position.x);
        Vector2 newVelocity = new Vector2(direction * speed, rigidBody.linearVelocity.y);
        rigidBody.linearVelocity = newVelocity;
    }

    public void Stop()
    {
        rigidBody.linearVelocity = new Vector2(0f, rigidBody.linearVelocity.y);
    }

    public void Flip(Vector2 targetPosition)
    {
        if (attackController != null && attackController.isAttacking) return;

        transform.localScale = new Vector2(targetPosition.x > transform.position.x ? 1f : -1f, 1f);
    }

    public void EndAttackAnimation()
    {
        if (movementStateMachine?.CurrentState is SimpleAttackState simple)
            simple.EndAttack();

        else if (movementStateMachine?.CurrentState is ComboAttackState combo)
            combo.NextComboStep();

        else if (movementStateMachine?.CurrentState is ChargeAttackState charge)
            charge.FinishCharge();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        isAlive = false;

        PauseMovement(true);
        PauseVertical(true);

        if (rigidBody != null) rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        if (attackController != null) attackController.enabled = false;
        if (damageController != null) damageController.enabled = false;

        movementStateMachine.ChangeState(new DeathState(this));
        verticalStateMachine.ChangeState(null);
    }

    public void Destroy()
    {
        if (damageController == null) return;

        Instantiate(damageController.deathParticles, transform.position, Quaternion.identity);

        for (int i = damageController.enemyScore; i > 0; i--)
        {
            GameObject soul = Instantiate(damageController.soulObject, transform.position, Quaternion.identity);
            soul.transform.position = new Vector2(
                soul.transform.position.x + Random.Range(-2f, 2f),
                soul.transform.position.y + Random.Range(-2f, -0.5f)
            );
        }

        Destroy(gameObject);
    }
}
