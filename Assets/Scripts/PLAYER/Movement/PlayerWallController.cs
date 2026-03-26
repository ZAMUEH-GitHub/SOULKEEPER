using UnityEngine;
using System.Collections;
using System;

public class PlayerWallController : MonoBehaviour, IPlayerSubController
{
    private PlayerStatsSO playerStats;
    public void Initialize(PlayerStatsSO stats) => playerStats = stats;

    public event Action<bool> OnWallSlideStateChanged;
    public event Action<bool> OnWallJumpStateChanged;

    [Header("Wall Settings")]
    public bool isWalled;
    public bool isWallSliding;
    public bool isWallJumping;
    private float nextWallJump, bufferCount;
    private Vector2 moveVector;
    private bool moveInput, wallJumpInput;

    [Header("Wall Check (Overlap Circle)")]
    public Transform wallCheckPoint;
    public float wallCheckRadius = 0.2f;
    public LayerMask wallLayer;

    private PlayerMovementController movementController;
    private PlayerJumpController jumpController;
    private Rigidbody2D playerRB;

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        movementController = GetComponent<PlayerMovementController>();
        jumpController = GetComponent<PlayerJumpController>();
    }

    public void UpdateWallState()
    {
        isWalled = Physics2D.OverlapCircle(wallCheckPoint.position, wallCheckRadius, wallLayer);

        bufferCount = Mathf.Max(0, bufferCount - Time.deltaTime);
        nextWallJump = Mathf.Max(0, nextWallJump - Time.deltaTime);

        if (playerStats == null) return;

        // --- CHANGED: Evaluate the slide condition here in Update ---
        bool wasWallSliding = isWallSliding;

        if (playerStats.wallSlideUnlocked && isWalled && !jumpController.isGrounded && moveVector.x != 0 && !jumpController.isJumping && !isWallJumping)
        {
            isWallSliding = true;
            jumpController.jumpCount = jumpController.maxJumpCount; // Reset jumps when clinging
        }
        else
        {
            isWallSliding = false;
        }

        if (isWallSliding != wasWallSliding)
        {
            OnWallSlideStateChanged?.Invoke(isWallSliding);
        }

        // --- Wall Jump Check ---
        if (playerStats.wallJumpUnlocked &&
            isWallSliding && bufferCount > 0 &&
            !jumpController.isGrounded && jumpController.jumpCount > 0 &&
            nextWallJump <= 0)
        {
            DoWallJump();
            jumpController.nextJump = playerStats.wallJumpLenght;
            nextWallJump = playerStats.wallJumpRate;
            bufferCount = 0;
            isWallJumping = true;
            StartCoroutine(CancelPlayerWallJump());
        }
    }

    public void ExecuteWallPhysics()
    {
        if (playerStats == null) return;

        // --- CHANGED: Only apply the velocity clamp here ---
        if (isWallSliding)
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x,
                Mathf.Clamp(playerRB.linearVelocity.y, -playerStats.wallSlidingSpeed, float.MaxValue));
        }
    }

    public void SetWallInput(Vector2 moveVector, bool moveInput)
    {
        this.moveVector = moveVector;
        this.moveInput = moveInput;
    }

    public void SetWallJumpInput(bool jumpInput)
    {
        wallJumpInput = jumpInput;
        if (jumpInput) bufferCount = playerStats.bufferTime;
    }

    private void PlayerWallSlide()
    {
        bool wasWallSliding = isWallSliding;

        if (isWalled && !jumpController.isGrounded && moveVector.x != 0 && !jumpController.isJumping && !isWallJumping)
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x,
                Mathf.Clamp(playerRB.linearVelocity.y, -playerStats.wallSlidingSpeed, float.MaxValue));
            jumpController.jumpCount = jumpController.maxJumpCount;
            isWallSliding = true;
        }
        else isWallSliding = false;

        if (isWallSliding != wasWallSliding)
        {
            OnWallSlideStateChanged?.Invoke(isWallSliding);
        }
    }

    private void DoWallJump()
    {
        playerRB.linearVelocity = new Vector2(-moveVector.x / playerStats.wallJumpDivider * playerStats.wallJumpForce, playerStats.wallJumpForce);
        isWallJumping = true;
        isWallSliding = false;
        jumpController.jumpCount--;
        transform.localScale = new Vector2(-moveVector.x, 1);

        OnWallJumpStateChanged?.Invoke(true);
    }

    private IEnumerator CancelPlayerWallJump()
    {
        yield return new WaitForSeconds(playerStats.wallJumpLenght);
        isWallJumping = false;
        movementController.PlayerFlip();

        OnWallJumpStateChanged?.Invoke(false);
    }

    public bool IsWallSliding => isWallSliding;
    public bool IsWallJumping => isWallJumping;
}