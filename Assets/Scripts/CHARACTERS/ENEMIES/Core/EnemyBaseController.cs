using UnityEngine;
using System;
using System.Collections.Generic;

public class EnemyBaseController : MonoBehaviour, IEnemy
{
    [Header("Debug States")]
    [SerializeField] private string currentState;
    [SerializeField] private List<string> activeLocks = new List<string>();

    [Header("Stats & References")]
    public EnemyStatsSO enemyStats;
    public Transform patrolTarget;

    [Header("Detection Settings")]
    public LayerMask obstacleLayer;

    #region Cached References
    [HideInInspector] public Rigidbody2D rigidBody { get; private set; }
    [HideInInspector] public Animator animator { get; private set; }
    [HideInInspector] public EnemyJumpController jumpController { get; private set; }
    [HideInInspector] public EnemyAttackController attackController { get; private set; }
    [HideInInspector] public EnemyDamageController damageController { get; private set; }
    [HideInInspector] public EnemyAnimationController animController { get; private set; }
    [HideInInspector] public EnemyPatrolController patrolController { get; private set; }
    [HideInInspector] public EnemyAudioController audioController { get; private set; }
    [HideInInspector] public Transform player { get; private set; }

    [HideInInspector] public Vector2 lastKnownPlayerPosition;
    [HideInInspector] public bool canSearch = true;
    [HideInInspector] public bool canPatrol = true;
    #endregion

    [Header("Runtime Variables")]
    public bool isAlive = true;
    public bool isDead = false;
    public bool targetPlayer = false;
    public Vector2 patrolStart;
    public Vector2 currentTarget;
    public float enemySpeed;
    public float moveSpeedMultiplier = 1f;
    [HideInInspector] public float nextAttackTimer;

    public EnemyStateMachine stateMachine;

    private HashSet<string> movementLocks = new HashSet<string>();

    public event Action<EnemyBaseController> OnEnemyDied;

    #region Unity Lifecycle
    void Start()
    {
        #region Script and Variable Suscriptions
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        jumpController = GetComponent<EnemyJumpController>();
        attackController = GetComponentInChildren<EnemyAttackController>();
        damageController = GetComponent<EnemyDamageController>();
        animController = GetComponent<EnemyAnimationController>();
        patrolController = GetComponent<EnemyPatrolController>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        enemySpeed = enemyStats.speed;
        patrolStart = transform.position;
        currentTarget = patrolTarget != null ? patrolTarget.position : transform.position;
        #endregion

        stateMachine = new EnemyStateMachine();

        stateMachine.Initialize(new IdleState(this));
    }

    void Update()
    {
        if (nextAttackTimer > 0) nextAttackTimer -= Time.deltaTime;

        stateMachine?.Update();
        currentState = stateMachine?.CurrentStateName;

        if (!isAlive || isDead) return;

        UpdateDetection();
    }
    #endregion

    #region Movement Lock Logic
    public bool CanMove => movementLocks.Count == 0;

    public void RequestMovementLock(string lockName)
    {
        movementLocks.Add(lockName);
        UpdateActiveLocksDisplay();
    }

    public void ReleaseMovementLock(string lockName)
    {
        movementLocks.Remove(lockName);
        UpdateActiveLocksDisplay();
    }

    public void ForceClearAllMovementLocks()
    {
        movementLocks.Clear();
        UpdateActiveLocksDisplay();
    }

    private void UpdateActiveLocksDisplay()
    {
        activeLocks.Clear();
        activeLocks.AddRange(movementLocks);
    }
    #endregion

    #region Enemy Move Logic
    public void SetTargetPlayer(bool target) => targetPlayer = target;

    public void MoveToTarget(Vector2 target, float speedOverride = -1f)
    {
        if (!CanMove) return;

        float speed = (speedOverride > 0) ? speedOverride : enemyStats.speed;

        speed *= moveSpeedMultiplier;

        float direction = Mathf.Sign(target.x - transform.position.x);
        Vector2 newVelocity = new Vector2(direction * speed, rigidBody.linearVelocity.y);
        rigidBody.linearVelocity = newVelocity;
    }

    public void Stop() => rigidBody.linearVelocity = new Vector2(0f, rigidBody.linearVelocity.y);

    public void Flip(Vector2 targetPosition)
    {
        if (!CanMove || (attackController != null && attackController.isAttacking)) return;
        transform.localScale = new Vector2(targetPosition.x > transform.position.x ? 1f : -1f, 1f);
    }
    #endregion

    #region Detection Logic
    private void UpdateDetection()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (player == null) return;
        }

        float activeVisionAngle = targetPlayer ? enemyStats.chaseAngle : enemyStats.visionAngle;
        float activeVisionRange = targetPlayer ? enemyStats.chaseRange : enemyStats.visionRange;

        targetPlayer = CanSeePlayer(activeVisionAngle, activeVisionRange);
    }

    public bool CanSeePlayer(float visionAngle, float visionRange)
    {
        if (player == null) return false;

        Vector2 directionToPlayer = (player.position - transform.position);
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > visionRange)
        {
            return false;
        }

        if (distanceToPlayer > enemyStats.proximityRange)
        {
            Vector2 facingDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            float angleToPlayer = Vector2.Angle(facingDirection, directionToPlayer);

            if (angleToPlayer > visionAngle / 2f)
            {
                return false;
            }
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, obstacleLayer);

        if (hit.collider != null)
        {
            return false;
        }

        return true;
    }
    #endregion

    #region Enemy Death Logic
    public void Die()
    {
        if (isDead) return;
        isDead = true;
        isAlive = false;

        ForceClearAllMovementLocks();
        Stop();

        if (rigidBody != null) rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        if (attackController != null) attackController.enabled = false;

        if (animator != null)
        {
            animator.SetBool("IsDead", true);
            animator.SetTrigger("Death");
        }
        stateMachine.ChangeState(new DeathState(this));
    }

    public void Destroy()
    {
        if (damageController == null) return;

        if (damageController.deathSound != null)
            audioController.audioSource.PlayOneShot(damageController.deathSound);

        if (damageController.deathParticles != null)
            Instantiate(damageController.deathParticles, new Vector2(transform.position.x, transform.position.y - 1), Quaternion.identity);

        for (int i = damageController.enemyScore; i > 0; i--)
        {
            GameObject soul = Instantiate(damageController.soulObject, transform.position, Quaternion.identity);
            soul.transform.position = new Vector2(
                soul.transform.position.x + UnityEngine.Random.Range(-2f, 2f),
                soul.transform.position.y + UnityEngine.Random.Range(-2f, -0.5f)
            );
        }

        OnEnemyDied?.Invoke(this);

        Destroy(gameObject);
    }
    #endregion

    #region Animation Events
    public void FinishJumpCharge()
    {
        if (stateMachine.CurrentState is AirborneState airborneState)
        {
            airborneState.ExecuteJump();
        }
    }

    public void EndAttack()
    {
        if (stateMachine.CurrentState is SimpleAttackState simpleState)
        {
            simpleState.AnimationFinished();
        }
        else if (stateMachine.CurrentState is ComboAttackState comboState)
        {
            comboState.AnimationFinished();
        }
    }

    public void StartChargeLunge()
    {
        if (stateMachine.CurrentState is ChargeAttackState chargeState)
        {
            chargeState.StartLunge();
        }
    }

    public void ImpactFinished()
    {
        if (stateMachine.CurrentState is ChargeAttackState chargeState)
        {
            chargeState.ImpactFinished();
        }
    }
    #endregion
}