/*using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerControllerOLD : Singleton<PlayerControllerOLD>
{
    [Header("Player Stats")]
    public int playerHealth;
    public bool isAlive = true;
    [HideInInspector] public int playerMaxHealth;
    public float playerSpeed;
    [Space(5f)]
    public Vector2 playerOrientation;
    [HideInInspector] public float speedMultiplier;
    private bool isTakingDamage = false;
    public bool isTrapped = false;

    [Header("RigidBody Settings")]
    public float jumpForce;
    public float wallJumpForce;
    public float dashForce;
    public float knockbackForce;

    [Header("Jump Settings")]
    public bool isGrounded;
    public int jumpCount;
    public int maxJumpCount;
    public bool isJumping;
    public float jumpRate = 0.25f;
    [HideInInspector] public float nextJump = 0.0f;

    private float jumpBufferCounter = 0.0f;
    private float jumpBufferTime = 0.10f;

    [Header("Wall Slide Settings")]
    public bool isWallSliding;
    public float wallSlidingSpeed;

    [Header("Wall Jump Settings")]
    public bool isWallJumping;
    public Vector2 wallJumpVector;
    public float wallJumpDuration;
    public float wallJumpRate = 0.35f;
    [HideInInspector] public float nextWallJump = 0.0f;
    [HideInInspector] public float wallJumpDivider = 2;

    private float wallJumpBufferCounter = 0.0f;
    private float wallJumpBufferTime = 0.10f;

    [Header("Dash Settings")]
    public bool isDashing;
    public Vector2 dashVector;
    public float dashDuration;
    public float dashRate = 2.0f;
    [HideInInspector] public float nextDash = 0.0f;

    private float dashBufferCounter = 0.0f;
    private float dashBufferTime = 0.15f;

    [Header("Knockback Settings")]
    public bool isKnockedBack;
    public float knockbackDuration;
    public Vector2 knockbackVector;

    [Header("Damage Control")]
    public float damageRate = 2.0f;
    public float nextDamage = 0.0f;

    [Header("Particle Systems")]
    public ParticleSystem damageParticles;
    public ParticleSystem deathParticles;

    private Rigidbody2D playerRigidBody;
    private CapsuleCollider2D playerCollider;
    private Animator playerAnimator;
    private SpriteRenderer playerSprite;

    [Header("UI References")]
    public CanvasGroup gameOverPanel;
    private float fadeDuration = 1.0f;

    [Header("Public GameObjects")]
    public GameManager gameManager;
    public PlayerJumpController jumpController;
    public PlayerWallController wallController;
    public PlayerAttackController attackController;
    public PlayerPowerUpController powerUpController;
    public GameObject soulObject;
    private Coroutine takeDamageCoroutine;
    private IAltar currentAltar;
    private Vector2 startPosition;

    protected override void Awake()
    {
        base.Awake();
        // Your Player-specific Awake code here
    }

    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        playerAnimator = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();

        isAlive = true;
        playerMaxHealth = playerHealth;
        startPosition = transform.position;
    }

    void Update()
    {
        if (!isAlive) { return; }
        
        PlayerMove();
        speedMultiplier = Mathf.Abs(playerOrientation.x);

        PlayerAnimation();

        StartPlayerBuffers();

        if (jumpBufferCounter > 0 && jumpController.jumpCount > 0 && !isWallJumping && !isWallSliding && nextJump == 0)
        {
            PlayerJump();
            nextJump = jumpRate;
            jumpController.jumpCount--;
            jumpBufferCounter = 0;
        }

        PlayerWallSlide();

        if (powerUpController.wallJumpUnlocked && isWallSliding && wallJumpBufferCounter > 0 && !jumpController.isGrounded && nextWallJump == 0)
        {
            PlayerWallJump();
            nextJump = wallJumpDuration;
            nextWallJump = wallJumpRate;
            wallJumpBufferCounter = 0;
            isWallJumping = true;
            StartCoroutine(CancelPlayerWallJump());
        }

        if (powerUpController.dashUnlocked && dashBufferCounter > 0 && !isWallSliding && !isWallJumping && !wallController.isWalled && nextDash == 0)
        {
            PlayerDash();
            nextDash = dashRate;
            dashBufferCounter = 0;
            isDashing = true;
            StartCoroutine(CancelPlayerDash());
        }

        if (!isTakingDamage) { playerSprite.color = Color.white; }

        isGrounded = jumpController.isGrounded;
        jumpCount = jumpController.jumpCount;
        maxJumpCount = jumpController.maxJumpCount;

        knockbackForce = attackController.knockbackForce;

        gameManager.HealthUpdater(playerHealth);
        gameManager.DashUpdater(nextDash);

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("E key pressed");
        }
    }

    public void PlayerInputMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerOrientation = context.ReadValue<Vector2>();
            PlayerFlip();
        }
        else if (context.canceled)
        {
            playerOrientation = Vector2.zero;
        }
    }

    private void PlayerMove()
    {
        if (!isAlive | isDashing || isWallJumping || isKnockedBack || isTrapped) return;
        
        playerRigidBody.linearVelocity = new Vector2(playerOrientation.x * playerSpeed, playerRigidBody.linearVelocityY);
    }

    public void PlayerInputJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpBufferCounter = jumpBufferTime;
            wallJumpBufferCounter = wallJumpBufferTime;
        }
    }

    private void PlayerJump()
    {
        playerRigidBody.linearVelocity = new Vector2(playerRigidBody.linearVelocityX, jumpForce);
        playerAnimator.SetTrigger("PlayerJump");
        isJumping = true;
        StartCoroutine(CancelPlayerJump());
    }

    private IEnumerator CancelPlayerJump()
    {
        yield return new WaitForSeconds(jumpRate);
        isJumping = false;
    }

    private void PlayerWallSlide()
    {
        if (wallController.isWalled && !jumpController.isGrounded && playerOrientation.x != 0 && !isJumping)
        {
            playerRigidBody.linearVelocity = new Vector2(playerRigidBody.linearVelocityX, Mathf.Clamp(playerRigidBody.linearVelocityY, -wallSlidingSpeed, float.MaxValue));
            jumpController.jumpCount = maxJumpCount - 1;
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void PlayerWallJump()
    {
        playerRigidBody.linearVelocity = new Vector2(-wallJumpVector.x / wallJumpDivider * wallJumpForce, wallJumpForce);
        isWallJumping = true;
        isWallSliding = false;
        PlayerWallJumpFlip();
    }

    private IEnumerator CancelPlayerWallJump()
    {
        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping = false;
        PlayerFlip();
    }

    public void PlayerInputDash(InputAction.CallbackContext context)
    {
        if (!isAlive | isWallSliding || isWallJumping || wallController.isWalled || isTrapped) return;

        if (context.performed)
        {
            dashBufferCounter = dashBufferTime;
        }
    }

    private void PlayerDash()
    {
        playerRigidBody.linearVelocity = new Vector2(dashVector.x * dashForce, 0);
        playerCollider.isTrigger = true;
        playerRigidBody.gravityScale = 0f;
    }

    private IEnumerator CancelPlayerDash()
    {
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        playerCollider.isTrigger = false;
        playerRigidBody.gravityScale = 5f;
    }

    public void PlayerKnockback(Vector2 knockbackVector, float knockbackForce, float knockbackDuration)
    {
        playerRigidBody.AddForce(knockbackVector * knockbackForce, ForceMode2D.Impulse);
        playerRigidBody.linearVelocity = (knockbackVector * knockbackForce);
        isKnockedBack = true;
        StartCoroutine(CancelPlayerKnockback(knockbackDuration));
    }

    public IEnumerator CancelPlayerKnockback(float knockbackDuration)
    {
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
    }

    private void PlayerFlip()
    {
        if (!isAlive | isWallJumping) { return; }

        if (playerOrientation.x > 0)
        {
            transform.localScale = new Vector2(1, 1);
            dashVector = new Vector2(1, 1);
            wallJumpVector = new Vector2(1, 1);
        }
        if (playerOrientation.x < 0)
        {
            transform.localScale = new Vector2(-1, 1);
            dashVector = new Vector2(-1, 1);
            wallJumpVector = new Vector2(-1, 1);
        }
    }

    private void PlayerWallJumpFlip()
    {
        transform.localScale = new Vector2(-wallJumpVector.x, wallJumpVector.y);
    }

    public void PlayerInputInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (currentAltar != null)
        {
            Debug.Log("Interact input received");
            currentAltar.Interact();
        }
    }

    private void PlayerAnimation()
    {
        if (playerOrientation.x != 0 && isGrounded)
        {
            playerAnimator.SetBool("isMoving", true);
            playerAnimator.SetBool("isWallSliding", false);
        }
        else { playerAnimator.SetBool("isMoving", false); }

        if (!isGrounded && playerRigidBody.linearVelocityY < -1 && !isWallSliding)
        {
            playerAnimator.SetBool("isFalling", true);
            playerAnimator.SetBool("isWallSliding", false);
        }
        else { playerAnimator.SetBool("isFalling", false); }

        if (isDashing)
        {
            playerAnimator.SetBool("isDashing", true);
        }
        else { playerAnimator.SetBool("isDashing", false); }

        if (isWallSliding)
        {
            playerAnimator.SetBool("isWallSliding", true);
            playerAnimator.SetBool("PlayerWallJump", false);
        }
        else if (isWallJumping)
        {
            playerAnimator.SetBool("PlayerWallJump", true);
            playerAnimator.SetBool("isWallSliding", false);
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TheCathedralOfTheLost")
        {
            ResetPlayer();
        }
    }

    private void StartPlayerBuffers()
    {
        nextJump = Mathf.Max(0, nextJump - Time.deltaTime);
        nextDash = Mathf.Max(0, nextDash - Time.deltaTime);
        nextWallJump = Mathf.Max(0, nextWallJump - Time.deltaTime);

        nextDamage = Mathf.Max(0, nextDamage - Time.deltaTime);

        jumpBufferCounter = Mathf.Max(0, jumpBufferCounter - Time.deltaTime);
        dashBufferCounter = Mathf.Max(0, dashBufferCounter - Time.deltaTime);
        wallJumpBufferCounter = Mathf.Max(0, wallJumpBufferCounter - Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Wall") || collision.CompareTag("Obstacle"))
        {
            playerCollider.isTrigger = false;
        }

        var altar = collision.GetComponent<IAltar>();
        if (altar != null)
        {
            currentAltar = altar;
            currentAltar.ShowUI();
            Debug.Log("Collided with altar");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other != null && other.CompareTag("MovingObstacle") || other != null && other.CompareTag("Parenting Collider"))
        {
            transform.SetParent(other.transform, true);
            playerCollider.isTrigger = false;
        }

        if (other != null && other.CompareTag("Minos Grab Collider"))
        {
            transform.SetParent(other.transform, true);
            transform.position = other.transform.position;
            playerRigidBody.gravityScale = 0f;
            isTrapped = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other != null && other.CompareTag("MovingObstacle") || other != null && other.CompareTag("Parenting Collider"))
        {
            transform.SetParent(null);
            playerCollider.isTrigger = false;

            DontDestroyOnLoad(gameObject);
        }

        if (other != null && other.CompareTag("Minos Grab Collider"))
        {
            transform.SetParent(null);
            isTrapped = false;
            playerRigidBody.gravityScale = 5f;
            playerCollider.isTrigger = false;

            DontDestroyOnLoad(gameObject);
        }

        var altar = other.GetComponent<IAltar>();
        if (altar != null && altar == currentAltar)
        {
            currentAltar.HideUI();
            currentAltar = null;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Spike") || collision.gameObject.CompareTag("Minos Attack Collider"))
        {
            knockbackVector = (transform.position - collision.transform.position).normalized;

            TakeDamage(1, knockbackVector);
            PlayerKnockback(knockbackVector, knockbackForce, knockbackDuration);
        }

        if (collision.gameObject.CompareTag("Bulb"))
        {
            knockbackVector = (transform.position - collision.transform.position).normalized;
            PlayerKnockback(knockbackVector, knockbackForce * 2, knockbackDuration);
            playerAnimator.SetTrigger("PlayerJump");
        }
    }

    private void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.alpha = 1f;           // Make it visible
            gameOverPanel.interactable = true;  // Allow clicks
            gameOverPanel.blocksRaycasts = true; // Block other input below UI
        }
    }
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        canvasGroup.alpha = startAlpha;
        canvasGroup.interactable = endAlpha > 0f;
        canvasGroup.blocksRaycasts = endAlpha > 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
        canvasGroup.interactable = endAlpha > 0f;
        canvasGroup.blocksRaycasts = endAlpha > 0f;
    }
    public void HideGameOverPanel()
    {
        gameOverPanel.alpha = 0f;
        gameOverPanel.interactable = false;
        gameOverPanel.blocksRaycasts = false;
    }

    public void Heal(int heal)
    {
        playerHealth += heal;
        if (playerHealth > playerMaxHealth)
        {
            playerHealth = playerMaxHealth;
        }

        GameManager.Instance.HealthUpdater(playerHealth);
    }

    public void TakeDamage(int damage, Vector2 damageVector)
    {
        Quaternion particleRotation = Quaternion.FromToRotation(Vector2.up, damageVector);
        
        if (!isDashing && !isTakingDamage && nextDamage <= 0)
        {
            if (takeDamageCoroutine != null)
            {
                StopCoroutine(takeDamageCoroutine);
            }
            takeDamageCoroutine = StartCoroutine(ExecuteTakeDamage(damage));
            Instantiate(damageParticles, transform.position, particleRotation);
        }
    }

    private IEnumerator ExecuteTakeDamage(int damage)
    {
        isTakingDamage = true;
        playerHealth -= damage;
        nextDamage = damageRate;
        playerSprite.color = Color.red;

        GameManager.Instance.HealthUpdater(playerHealth);

        yield return new WaitForSeconds(0.25f);

        playerSprite.color = Color.white;
        isTakingDamage = false;

        if (playerHealth <= 0)
        {
            ShowGameOverPanel();
            Die();
        }
    }

    public void Die()
    {
        isAlive = false;
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        StartCoroutine(FadeCanvasGroup(gameOverPanel, 0f, 1f, fadeDuration));

        for (int i = gameManager.playerScore / 2; i > 0; i--)
        {
            GameObject soul = Instantiate(soulObject, transform.position, Quaternion.identity);
            soul.transform.position = new Vector2(soul.transform.position.x + Random.Range(-2f, 2f), soul.transform.position.y + Random.Range(-1.5f, 2f));
        }
    }

    public void ResetPlayer()
    {
        playerHealth = playerMaxHealth;
        transform.position = startPosition;
    }
}*/