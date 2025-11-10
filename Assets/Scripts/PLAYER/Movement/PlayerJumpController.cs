using UnityEngine;
using System.Collections;

public class PlayerJumpController : MonoBehaviour
{
    [SerializeField] private PlayerStatsSO playerStats;

    [Header("Jump Parameters")]
    public float jumpForce => playerStats.jumpForce;
    private bool jumpInput;
    public bool isGrounded;
    private bool wasGrounded;
    public bool isJumping;
    [Space(5)]
    public int jumpCount;
    public int maxJumpCount => playerStats.maxJumpCount;
    public float jumpRate => playerStats.jumpRate;
    [HideInInspector] public float nextJump;

    public float coyoteTime => playerStats.coyoteTime;
    private float coyoteCount;

    public float bufferTime => playerStats.bufferTime;
    private float bufferCount;

    [Header("Ground Check (OverlapCircle)")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D playerRB;
    private Animator playerAnimator;

    private void Awake()
    {
        #region Script and Variable Subscriptions

        var controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            playerStats = controller.playerRuntimeStats;
        }

        playerRB = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponentInParent<Animator>();

        #endregion
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        if (IsGrounded && !wasGrounded)
        {
            jumpCount = maxJumpCount;
        }

        wasGrounded = isGrounded;

        #region Jump Counters
        if (isGrounded)
        {
            coyoteCount = coyoteTime;
        }
        else
        {
            coyoteCount -= Time.deltaTime;
            if (coyoteCount < 0)
                coyoteCount = 0;
        }

        if (bufferCount > 0)
        {
            bufferCount -= Time.deltaTime;
            if (bufferCount < 0)
                bufferCount = 0;
        }

        if (nextJump > 0)
        {
            nextJump -= Time.deltaTime;
            if (nextJump < 0)
                nextJump = 0;
        }
        #endregion

        if (playerStats.jumpUnlocked)

            if (nextJump <= 0 && bufferCount > 0 && (IsGrounded || jumpCount > 0 || coyoteCount > 0))
            {
                DoJump();
                nextJump = jumpRate;
                bufferCount = 0;
            }
    }

    public void SetJumpInput(bool jumpInput)
    {
        this.jumpInput = jumpInput;
        PlayerJump(this.jumpInput);
    }

    public void PlayerJump(bool jumpInput)
    {
        if (jumpInput)
        {
            bufferCount = bufferTime;
        }
    }

    private void DoJump()
    {
        playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, jumpForce);
        playerAnimator.SetTrigger("PlayerJump");
        isJumping = true;
        jumpCount--;
        StartCoroutine(CancelPlayerJump());
    }

    private IEnumerator CancelPlayerJump()
    {
        yield return new WaitForSeconds(jumpRate);
        isJumping = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }

    public bool IsGrounded => isGrounded;
    public bool IsJumping => isJumping;
    public int JumpCount => jumpCount;
    public int MaxJump => maxJumpCount;
}
