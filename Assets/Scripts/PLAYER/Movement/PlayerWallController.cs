using UnityEngine;
using System.Collections;

public class PlayerWallController : MonoBehaviour, IPlayerSubController
{
    private PlayerStatsSO playerStats;
    public void Initialize(PlayerStatsSO stats) => playerStats = stats;

    [Header("Wall Settings")]
    public bool isWalled, isWallSliding, isWallJumping;

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

    private void Update()
    {
        isWalled = Physics2D.OverlapCircle(wallCheckPoint.position, wallCheckRadius, wallLayer);

        bufferCount = Mathf.Max(0, bufferCount - Time.deltaTime);
        nextWallJump = Mathf.Max(0, nextWallJump - Time.deltaTime);

        if (playerStats == null) return;

        if (playerStats.wallSlideUnlocked)
            PlayerWallSlide();

        if (playerStats.wallJumpUnlocked &&
            IsWallSliding && bufferCount > 0 &&
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
        if (isWalled && !jumpController.isGrounded && moveVector.x != 0 && !jumpController.isJumping)
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x,
                Mathf.Clamp(playerRB.linearVelocity.y, -playerStats.wallSlidingSpeed, float.MaxValue));
            jumpController.jumpCount = jumpController.maxJumpCount;
            isWallSliding = true;
        }
        else isWallSliding = false;
    }

    private void DoWallJump()
    {
        playerRB.linearVelocity = new Vector2(-moveVector.x / playerStats.wallJumpDivider * playerStats.wallJumpForce, playerStats.wallJumpForce);
        isWallJumping = true;
        isWallSliding = false;
        jumpController.jumpCount--;
        transform.localScale = new Vector2(-moveVector.x, 1);
    }

    private IEnumerator CancelPlayerWallJump()
    {
        yield return new WaitForSeconds(playerStats.wallJumpLenght);
        isWallJumping = false;
        movementController.PlayerFlip();
    }

    public bool IsWallSliding => isWallSliding;
    public bool IsWallJumping => isWallJumping;
}
