using UnityEngine;

public class PlayerControllerOLD : MonoBehaviour
{
    /*[Header ("Player Stats")]
    public float playerSpeed;
    public int playerHealth;

    [Header ("RB Force Settings")]
    public float jumpForce;
    public float dashForce;

    [HideInInspector] public Vector2 playerOrientation;

    private Rigidbody2D playerRigidBody;
    private CapsuleCollider2D playerCollider;

    private float nextJump = 0.0f;
    private float jumpRate = 0.35f;

    private float jumpBufferCounter = 0.0f;
    private float jumpBufferTime = 0.35f;

    private float nextDash = 0.0f;
    private float dashRate = 2f;

    private float dashBufferCounter = 0.0f;
    private float dashBufferTime = 0.25f;

    private float nextDamage = 0.0f;
    private float damageRate = 1.5f;

    private GameManager gameManager;
    private PlayerJumpController jumpController;
    
    void Start()
    {      
        playerCollider = GetComponent<CapsuleCollider2D>();
        playerRigidBody = GetComponent<Rigidbody2D>();

        gameManager = GameObject.Find("GAME MANAGER").GetComponent<GameManager>();
        jumpController = GameObject.Find("Player Jump Collider").GetComponent<PlayerJumpController>();
    }

    void Update()
    {
        // Player Movement with Translate
        transform.Translate(Input.GetAxis("Horizontal") * playerOrientation * playerSpeed * Time.deltaTime);

        // Jump on Up Arrow with Buffer Counters
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            jumpBufferCounter = jumpBufferTime; // Reset Buffer timer
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime; // Decrease over Time
        }

        // Allow Jump if within Buffer Time
        if (jumpBufferCounter > 0 && jumpController.jumpCount > 0 && Time.time > nextJump)
        {
            Jump();
            nextJump = Time.time + jumpRate;
            jumpController.jumpCount--;
            jumpBufferCounter = 0; // Reset after Successful Jump
        }

        // Dash on LeftShift with Buffer Counters
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            dashBufferCounter = dashBufferTime; // Reset Buffer timer
        }
        else
        {
            dashBufferCounter -= Time.deltaTime; // Decrease over Time
        }

        // Allow Dash if within Buffer Time
        if (dashBufferCounter > 0 && Time.time > nextDash)
        {
            Dash();
            nextDash = Time.time + dashRate;
            dashBufferCounter = 0; // Reset after Successful Dash
        }

        // Defines the Vector2 for the Projectiles
        if (Input.GetAxis("Horizontal") > 0)
        {
            playerOrientation = new Vector2(1,0); //Vector2.right
            transform.localRotation = Quaternion.Euler(0,0,0);
        }
        if (Input.GetAxis("Horizontal") < 0)
        {
            playerOrientation = new Vector2(-1,0); //Vector2.left
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        // Send Player Health value to Game Manager
        gameManager.HealthUpdater(playerHealth);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Defines the actions once Player collides with Enemy
        if (collision.gameObject.tag == "Enemy" && Time.time > nextDamage)
        {
            TakeDamage(1);
            nextDamage = Time.time + damageRate;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Sets the Moving Obstacle as Parent on contact to take Transform
        if (other.gameObject.tag == "MovingObstacle")
        {
            transform.SetParent(other.transform, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Removes the parent for the Player once contact stops
        if (collision.gameObject.tag == "MovingObstacle")
        {
            transform.SetParent(null);
        }
    }
    
    private void Jump()
    {
        //playerRigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        playerRigidBody.linearVelocity = new Vector2(playerRigidBody.linearVelocityX, jumpForce);
    }

    private void Dash()
    {
        playerRigidBody.AddForce(playerOrientation * dashForce, ForceMode2D.Impulse);
    }

    private void TakeDamage(int damage)
    {
        playerHealth -= damage; // Damage (float damage) depends on the OnCollisionEnter2D

        if (playerHealth < 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }*/
}
