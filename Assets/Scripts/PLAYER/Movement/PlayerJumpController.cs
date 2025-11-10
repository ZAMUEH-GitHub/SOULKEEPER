using UnityEngine;
using System.Collections;

public class PlayerJumpController : MonoBehaviour, IPlayerSubController
{
    private PlayerStatsSO playerStats;
    public void Initialize(PlayerStatsSO stats) => playerStats = stats;

    [Header("Jump Parameters")]
    public float jumpForce => playerStats.jumpForce;
    private bool jumpInput;
    public bool isGrounded, wasGrounded, isJumping;
    public int jumpCount;
    public int maxJumpCount => playerStats.maxJumpCount;
    public float jumpRate => playerStats.jumpRate;
    public float coyoteTime => playerStats.coyoteTime;
    public float bufferTime => playerStats.bufferTime;
    [HideInInspector] public float nextJump;

    private float coyoteCount, bufferCount;

    [Header("Ground Check (Overlap Circle)")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D playerRB;
    private Animator playerAnimator;

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        if (isGrounded && !wasGrounded) jumpCount = maxJumpCount;
        wasGrounded = isGrounded;

        HandleCounters();

        if (playerStats != null && playerStats.jumpUnlocked &&
            nextJump <= 0 && bufferCount > 0 &&
            (isGrounded || jumpCount > 0 || coyoteCount > 0))
        {
            DoJump();
            nextJump = jumpRate;
            bufferCount = 0;
        }
    }

    private void HandleCounters()
    {
        coyoteCount = isGrounded ? coyoteTime : Mathf.Max(0, coyoteCount - Time.deltaTime);
        bufferCount = Mathf.Max(0, bufferCount - Time.deltaTime);
        nextJump = Mathf.Max(0, nextJump - Time.deltaTime);
    }

    public void SetJumpInput(bool jumpInput)
    {
        this.jumpInput = jumpInput;
        if (jumpInput) bufferCount = bufferTime;
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
}
