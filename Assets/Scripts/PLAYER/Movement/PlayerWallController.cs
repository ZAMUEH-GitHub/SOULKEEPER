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
    private float nextWallJump;
    private Vector2 moveVector => PlayerController.Instance.moveVector;

    private PlayerMovementController movementController;
    private PlayerJumpController jumpController;
    private Rigidbody2D playerRB;
    private PlayerCollisionController collisionController;

    #region Unity Lifecycle
    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        collisionController = GetComponent<PlayerCollisionController>();

        movementController = PlayerController.Instance.movementController;
        jumpController = PlayerController.Instance.jumpController;
    }

    private void Update()
    {
        nextWallJump = Mathf.Max(0, nextWallJump - Time.deltaTime);
    }
    #endregion

    #region Wall Sliding Logic
    public void UpdateWallState()
    {
        bool isPressingAgainstWall = moveVector.x != 0 && Mathf.Sign(moveVector.x) == Mathf.Sign(transform.localScale.x);

        isWalled = collisionController.isWalled && isPressingAgainstWall;

        if (playerStats == null) return;

        bool wasWallSliding = isWallSliding;

        if (playerStats.wallSlideUnlocked && isWalled && !jumpController.isGrounded && !jumpController.isJumping && !isWallJumping)
        {
            isWallSliding = true;
            jumpController.jumpCount = jumpController.maxJumpCount;
        }
        else
        {
            isWallSliding = false;
        }

        if (isWallSliding != wasWallSliding)
        {
            OnWallSlideStateChanged?.Invoke(isWallSliding);
        }

        if (playerStats.wallJumpUnlocked &&
            isWallSliding &&
            !jumpController.isGrounded && jumpController.jumpCount > 0 &&
            nextWallJump <= 0)
        {
            if (PlayerController.Instance.ConsumeJumpInput())
            {
                DoWallJump();
                jumpController.nextJump = playerStats.wallJumpLenght;
                nextWallJump = playerStats.wallJumpRate;
                isWallJumping = true;
                StartCoroutine(CancelPlayerWallJump());
            }
        }
    }

    public void ExecuteWallPhysics()
    {
        if (playerStats == null) return;

        if (isWallSliding)
        {
            float pushVelocity = moveVector.x * movementController.playerSpeed;

            playerRB.linearVelocity = new Vector2(pushVelocity,
                Mathf.Clamp(playerRB.linearVelocity.y, -playerStats.wallSlidingSpeed, float.MaxValue));
        }
    }
    #endregion

    #region Wall Jump Logic
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
    #endregion

    public bool IsWallSliding => isWallSliding;
    public bool IsWallJumping => isWallJumping;
}