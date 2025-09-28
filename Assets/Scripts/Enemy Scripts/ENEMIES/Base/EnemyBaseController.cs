using UnityEngine;

public class EnemyBaseController : MonoBehaviour, IEnemy
{
    [SerializeField] private string currentStateDebug;

    [Header("Stats & References")]
    public EnemyStatsSO enemyStats;
    public Transform patrolTarget;

    #region Script and Component References

    [HideInInspector] public Rigidbody2D rigidBody { get; private set; }
    [HideInInspector] public Animator animator { get; private set; }
    [HideInInspector] public EnemyJumpController jumpController { get; private set; }
    [HideInInspector] public EnemyAttackController attackController { get; private set; }
    [HideInInspector] public EnemyDamageController damageController { get; private set; }
    [HideInInspector] public Transform player { get; private set; }
    #endregion

    [Header("Runtime Variables")]
    public bool isAlive = true;
    public bool targetPlayer = false;
    public Vector2 patrolStart;
    public Vector2 currentTarget;
    public float enemySpeed;
    public bool isDead = false;

    public EnemyStateMachine stateMachine;

    void Start()
    {
        #region Script and Component Suscriptions

        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        jumpController = GetComponent<EnemyJumpController>();
        attackController = GetComponentInChildren<EnemyAttackController>();
        damageController = GetComponent<EnemyDamageController>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        #endregion

        enemySpeed = enemyStats.speed;

        patrolStart = transform.position;
        currentTarget = patrolTarget.position;

        stateMachine = new EnemyStateMachine();
        stateMachine.Initialize(new IdleState(this));
    }

    void Update()
    {
        if (!isAlive || isDead) return;
        stateMachine.Update();
        currentStateDebug = stateMachine.CurrentStateName;
    }

    public void ChangeState(IEnemyState newState)
    {
        stateMachine.ChangeState(newState);
    }

    public void SetTargetPlayer(bool target) => targetPlayer = target;

    public void MoveToTarget(Vector2 target, float speedOverride = -1f)
    {
        if (damageController.isKnockedBack) return;

        float speed = (speedOverride > 0) ? speedOverride : enemyStats.speed;

        float dir = Mathf.Sign(target.x - transform.position.x);

        Vector2 newVelocity = new Vector2(dir * speed, rigidBody.linearVelocity.y);
        rigidBody.linearVelocity = newVelocity;
    }

    public void Stop()
    {
        rigidBody.linearVelocity = new Vector2(0, rigidBody.linearVelocity.y);
    }

    public void Flip(Vector2 targetPosition)
    {
        if (attackController.isAttacking) return;

        if (targetPosition.x > transform.position.x)
            transform.localScale = new Vector2(1, 1);
        else if (targetPosition.x < transform.position.x)
            transform.localScale = new Vector2(-1, 1);
    }

    public void OnComboStep()
    {
        if (stateMachine.CurrentState is ComboAttackState comboState)
        {
            comboState.NextComboStep();
        }
    }

    public void EndAttackAnimation()
    {
        if (stateMachine != null && stateMachine.CurrentState is SimpleAttackState attackState)
        {
            attackState.EndAttack();
        }
    }

    public void FinishCharge()
    {
        if (stateMachine.CurrentState is ChargeAttackState chargeState)
        {
            chargeState.FinishCharge();
        }
    }

    public void Die()
    {
        isDead = true;
        ChangeState(new DeathState(this));
    }
}
