using UnityEngine;

public class MovingEnemyController : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int enemyHealth;
    public int enemySpeed;
    public int enemyScore;

    [Header("Patrol Settings")]
    public float movingEnemyLimit;
    [Range(-1, 1)] public int vectorUpDown;
    [Range(-1, 1)] public int vectorLeftRight;

    [HideInInspector] public Vector2 movingEnemyVector; 
    private float xPositiveLimit, xNegativeLimit;
    private float yPositiveLimit, yNegativeLimit;

    private CapsuleCollider2D enemyCollider;
    private Rigidbody2D enemyRigidBody;

    private GameManager gameManager;

    void Start()
    {
        enemyCollider = GetComponent<CapsuleCollider2D>();
        enemyRigidBody = GetComponent<Rigidbody2D>();

        gameManager = GameObject.Find("GAME MANAGER").GetComponent<GameManager>();

        movingEnemyVector = new Vector2(vectorLeftRight, vectorUpDown);

        xPositiveLimit = transform.position.x + movingEnemyLimit;
        xNegativeLimit = transform.position.x - movingEnemyLimit;
        yPositiveLimit = transform.position.y + movingEnemyLimit;
        yNegativeLimit = transform.position.y - movingEnemyLimit;
    }

    void Update()
    {
        transform.Translate(movingEnemyVector * enemySpeed * Time.deltaTime);

        if (transform.position.x > xPositiveLimit || transform.position.y > yPositiveLimit)
        {
            movingEnemyVector *= -1;
        }

        if (transform.position.x < xNegativeLimit || transform.position.y < yNegativeLimit)
        {
            movingEnemyVector *= -1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player Attack Collider") || collision.CompareTag("Projectile"))
        {
            TakeDamage(1);
        }
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
    }
}
