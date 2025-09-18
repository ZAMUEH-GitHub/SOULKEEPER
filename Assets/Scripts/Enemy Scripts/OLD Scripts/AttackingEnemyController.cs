using System.Collections;
using UnityEngine;

public class AttackingEnemyController : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int enemyHealth;
    public int enemySpeed;
    public float enemyJumpForce;
    public int enemyScore;

    [Header("Target Settings")]
    public GameObject playerTarget;
    public Vector2 enemyTarget;
    [Space(10f)]
    public Vector2 playerTargetVector;
    public Vector2 enemyStartPosition;

    [Header("Knockback Settings")]
    public bool isKnockedBack;
    public float knockbackDuration;
    public Vector2 knockbackVector;

    [Header("Game Objects")]
    public GameObject enemyFolder;

    private float nextJump = 0.0f;
    private float jumpRate = 1.5f;

    private CapsuleCollider2D enemyCollider;
    private Rigidbody2D enemyRigidBody;
    public EnemyJumpController enemyJumpController;

    private PlayerController playerController;
    public GameManager gameManager;

    void Start()
    {
        enemyCollider = GetComponent<CapsuleCollider2D>();
        enemyRigidBody = GetComponent<Rigidbody2D>();

        playerController = GameObject.Find("PLAYER").GetComponent<PlayerController>();

        enemyStartPosition = transform.position;
        enemyTarget = transform.position;
    }

    void Update()
    {
        playerTargetVector = playerTarget.transform.position;

        EnemyTarget();

        if (enemyTarget.y > transform.position.y + 2 && enemyJumpController.isGrounded == true && Time.time > nextJump)
        {
            EnemyJump();
            nextJump = Time.time + jumpRate;
        }
    }

    public void EnemyTarget()
    {
        if (!isKnockedBack)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget, enemySpeed * Time.deltaTime);
        }
    }

    public void EnemyJump()
    {
        enemyRigidBody.AddForce(Vector2.up * enemyJumpForce, ForceMode2D.Impulse);
    }

    public void EnemyKnockback(Vector2 knockbackVector, float knockbackForce)
    {
        //enemyRigidBody.AddForce(knockbackVector * knockbackForce, ForceMode2D.Impulse);
        enemyRigidBody.linearVelocity = (knockbackVector * knockbackForce);
        isKnockedBack = true;
        StartCoroutine(CancelEnemyKnockback());
    }

    private IEnumerator CancelEnemyKnockback()
    {
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
    }

    public void TakeDamage(int damage)
    {
        enemyHealth -= damage;

        if (enemyHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        gameManager.ScoreUpdater(enemyScore);
        Destroy(gameObject);
        Destroy(enemyFolder);
    }
}
