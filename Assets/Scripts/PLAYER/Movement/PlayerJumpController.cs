using UnityEngine;
using System.Collections;
using System;

public class PlayerJumpController : MonoBehaviour, IPlayerSubController
{
    private PlayerStatsSO playerStats;
    public void Initialize(PlayerStatsSO stats) => playerStats = stats;

    public event Action OnJumpPerformed;

    [Header("Jump Parameters")]
    public float jumpForce => playerStats.jumpForce;
    public bool isGrounded, wasGrounded, isJumping;
    public int jumpCount;
    public int maxJumpCount => playerStats.maxJumpCount;
    public float jumpRate => playerStats.jumpRate;
    public float coyoteTime => playerStats.coyoteTime;
    [HideInInspector] public float nextJump;

    private float coyoteCount;
    private Coroutine jumpCoroutine;

    private Rigidbody2D playerRB;
    private PlayerCollisionController collisionController;
    private PlayerWallController wallController;

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        collisionController = GetComponent<PlayerCollisionController>();
        wallController = PlayerController.Instance.wallController;
    }

    private void Update()
    {
        HandleCounters();
        UpdateGroundedStatus();
    }

    private void UpdateGroundedStatus()
    {
        isGrounded = collisionController.isGrounded;

        if (isGrounded && !wasGrounded) jumpCount = maxJumpCount;
        wasGrounded = isGrounded;
    }

    private void HandleCounters()
    {
        coyoteCount = isGrounded ? coyoteTime : Mathf.Max(0, coyoteCount - Time.deltaTime);
        nextJump = Mathf.Max(0, nextJump - Time.deltaTime);
    }

    public bool CanJump()
    {
        return playerStats != null && playerStats.jumpUnlocked &&
               nextJump <= 0 &&
               (isGrounded || jumpCount > 0 || coyoteCount > 0) &&
               !wallController.IsWallSliding && !wallController.IsWallJumping;
    }

    public void ExecuteJump()
    {
        playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, jumpForce);

        OnJumpPerformed?.Invoke();

        isJumping = true;
        jumpCount--;
        nextJump = jumpRate;

        if (jumpCoroutine != null) StopCoroutine(jumpCoroutine);
        jumpCoroutine = StartCoroutine(CancelPlayerJump());
    }

    private IEnumerator CancelPlayerJump()
    {
        yield return new WaitForSeconds(jumpRate);
        isJumping = false;
    }

    #region Custom Bounce / Pogo Logic
    public void ExecuteBounce(Vector2 direction, float force)
    {
        playerRB.linearVelocity = direction * force;
        OnJumpPerformed?.Invoke();
        jumpCount = maxJumpCount;

        if (PlayerController.Instance.stateMachine.CurrentStateName != "PlayerAirborneState")
        {
            PlayerController.Instance.stateMachine.ChangeState(PlayerController.Instance.airborneState);
        }
    }
    #endregion
}