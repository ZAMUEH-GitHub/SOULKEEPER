/*using UnityEngine;

public class OLDEnemyController : MonoBehaviour
{
    public float enemySpeed;
    public int enemyHealth = 5;
    public int enemyScore = 1;

    public float xLimit;
    private float xPositiveLimit;
    private float xNegativeLimit;
    
    public Vector2 enemyOrientation;

    private CapsuleCollider2D enemyCollider;
    private Rigidbody2D enemyRigidBody;

    private GameManager gameManager;

    void Start()
    {
        enemyCollider = GetComponent<CapsuleCollider2D>();
        enemyRigidBody = GetComponent<Rigidbody2D>();

        gameManager = GameObject.Find("GAME MANAGER").GetComponent<GameManager>();

        enemyOrientation = Vector2.right;

        xPositiveLimit = transform.position.x + xLimit;
        xNegativeLimit = transform.position.x - xLimit;
    }

    void Update()
    {
        transform.Translate(enemyOrientation * enemySpeed * Time.deltaTime);

        if (transform.position.x > xPositiveLimit)
        {
            enemyOrientation = Vector2.left;
        }
        if (transform.position.x < xNegativeLimit)
        {
            enemyOrientation = Vector2.right;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            enemyHealth --;

            if (enemyHealth <= 0)
            {
                gameManager.ScoreUpdater(enemyScore);
                Destroy(gameObject);
            }
        }
    }
}
*/