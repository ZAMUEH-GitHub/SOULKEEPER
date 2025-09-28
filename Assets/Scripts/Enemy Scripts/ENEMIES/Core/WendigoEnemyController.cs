using UnityEngine;
using System.Collections;

public class WendigoEnemyController : MonoBehaviour, IEnemy
{
    public EnemyStatsSO enemyStats;

    private int enemyHealth;
    private float enemySpeed;
    private int enemyScore;
    public GameObject soulObject;
    public bool isAlive;

    [Header("Target Settings")]
    public bool targetPlayer;
    public bool isIdling;
    public float idleDuration;
    private Vector2 patrolStart;
    private Vector2 currentTarget;
    private Vector2 playerTarget;
    public GameObject patrolTarget;

    #region Script and Component References

    private EnemyJumpController jumpController;
    private EnemyWallController wallController;
    private EnemyAttackController attackController;
    private EnemyDamageController damageController;

    private GameObject player;

    private Rigidbody2D enemyRigidBody;
    private Animator enemyAnimator;
    #endregion

    void Start()
    {
        #region Script, Component and Variable Suscriptions

        enemyRigidBody = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();

        jumpController = GetComponent<EnemyJumpController>();
        wallController = GetComponent<EnemyWallController>();
        attackController = GetComponentInChildren<EnemyAttackController>();
        damageController = GetComponent<EnemyDamageController>();

        player = GameObject.FindGameObjectWithTag("Player");

        enemyHealth = enemyStats.health;
        enemySpeed = enemyStats.speed;
        enemyScore = enemyStats.score;
        #endregion

        patrolStart = transform.position;
        currentTarget = patrolTarget.transform.position;

        isAlive = true;
    }

    void Update()
    {
        if (!isAlive) return;
        
        playerTarget = player.transform.position;

        if (!isIdling)
        {
            if (targetPlayer)
            {
                MoveToTarget(playerTarget);
                enemySpeed = 6;
                enemyAnimator.SetFloat("WalkSpeed", 3f);
            }
            else 
            {
                Patrol(); 
                enemySpeed = 2;
                enemyAnimator.SetFloat("WalkSpeed", 1f);
            }
        }

        jumpController.EnemyJump(isAlive, currentTarget);
        attackController.EnemyAttack(playerTarget);
        EnemyFlip();
        EnemyAnimation();
    }

    private void Patrol()
    {
        if (Vector2.Distance(transform.position, currentTarget) < 0.5f)
        {
            if (!isIdling)
            {
                StartCoroutine(ChangePatrolTarget());
            }
        }
        MoveToTarget(currentTarget);
    }

    private IEnumerator ChangePatrolTarget()
    {
        isIdling = true;
        enemyRigidBody.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(idleDuration);
        currentTarget = (currentTarget == (Vector2)patrolTarget.transform.position) ? patrolStart : patrolTarget.transform.position;
        isIdling = false;
    }

    private void MoveToTarget(Vector2 target)
    {
        if (!isAlive || jumpController.isChargingJump || damageController.isKnockedBack || attackController.isAttacking || isIdling) return;

        Vector2 direction = (target - (Vector2)transform.position).normalized;
        enemyRigidBody.linearVelocity = new Vector2(direction.x * enemySpeed, enemyRigidBody.linearVelocityY);

        currentTarget = target;
    }

    public void SetTargetPlayer(bool targetPlayer)
    {
        this.targetPlayer = targetPlayer;
    }

    private void EnemyFlip()
    {
        if (!isAlive || attackController.isAttacking) return;

        Vector2 targetPosition = targetPlayer ? playerTarget : currentTarget;

        if (targetPosition.x > transform.position.x)
        {
            transform.localScale = new Vector2(1, 1);
        }
        else if (targetPosition.x < transform.position.x)
        {
            transform.localScale = new Vector2(-1, 1);
        }
    }

    private void EnemyAnimation()
    {
        if (!isAlive) return;
        
        if (jumpController.isGrounded && Mathf.Abs(enemyRigidBody.linearVelocityX) > 0.1f)
        {
            enemyAnimator.SetBool("isMoving", true);
        }
        else { enemyAnimator.SetBool("isMoving", false); }

        if (!jumpController.isGrounded && (enemyRigidBody.linearVelocityY) < -0.1f)
        {
            enemyAnimator.SetBool("isFalling", true);
        }
        else { enemyAnimator.SetBool("isFalling", false); }

        enemyAnimator.SetBool("isChargingJump", jumpController.isChargingJump);
    }
}
