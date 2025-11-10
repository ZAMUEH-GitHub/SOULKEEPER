using UnityEngine;
using System.Collections;

public class PlayerWallController : MonoBehaviour
{
    [SerializeField] private PlayerStatsSO playerStats;

    [Header("Wall Settings")]
    public bool isWalled;
    public bool isWallSliding;
    public float wallSlidingSpeed => playerStats.wallSlidingSpeed;

    [Header("Wall Jump Settings")]
    public bool isWallJumping;
    public float wallJumpForce => playerStats.wallJumpForce;
    public float wallJumpLenght => playerStats.wallJumpLenght;
    public float wallJumpDivider => playerStats.wallJumpDivider;

    public float wallJumpRate => playerStats.wallJumpRate;
    private float nextWallJump;

    public float bufferTime => playerStats.bufferTime;
    private float bufferCount;

    [Header("Wall Check (OverlapCircle)")]
    public Transform wallCheckPoint;
    public float wallCheckRadius = 0.2f;
    public LayerMask wallLayer;

    #region Script, Component and Variable References

    private Vector2 moveVector;
    private bool moveInput;
    private bool wallJumpInput;

    private PlayerMovementController movementController;
    private PlayerJumpController jumpController;
    private Rigidbody2D playerRB;
    #endregion

    private void Awake()
    {
        #region Script, Component and Variable Suscriptions

        var controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            playerStats = controller.playerRuntimeStats;
        }

        movementController = GetComponent<PlayerMovementController>();
        jumpController = GetComponent<PlayerJumpController>();
        playerRB = GetComponent<Rigidbody2D>();


        #endregion
    }

    private void Update()
    {
        isWalled = Physics2D.OverlapCircle(wallCheckPoint.position, wallCheckRadius, wallLayer);

        #region WallJump Counters
        if (bufferCount > 0)
        {
            bufferCount -= Time.deltaTime;
            if (bufferCount < 0)
                bufferCount = 0;
        }

        if (nextWallJump > 0)
        {
            nextWallJump -= Time.deltaTime;
            if (nextWallJump < 0)
                nextWallJump = 0;
        }
        #endregion

        if (playerStats.wallSlideUnlocked) PlayerWallSlide();

        if (playerStats.wallJumpUnlocked)

            if (IsWallSliding && bufferCount > 0 && !jumpController.IsGrounded && jumpController.jumpCount > 0 && nextWallJump <= 0)
            {
                DoWallJump();
                jumpController.nextJump = wallJumpLenght;
                nextWallJump = wallJumpRate;
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
        PlayerWallJump(wallJumpInput);
    }

    public void PlayerWallJump(bool jumpInput)
    {
        if (jumpInput)
        {
            bufferCount = bufferTime;
        }
    }

    private void PlayerWallSlide()
    {
        if (IsWalled && !jumpController.IsGrounded && moveVector.x != 0 && !jumpController.IsJumping)
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, Mathf.Clamp(playerRB.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
            jumpController.jumpCount = jumpController.maxJumpCount;
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void DoWallJump()
    {
        playerRB.linearVelocity = new Vector2(-moveVector.x / wallJumpDivider * wallJumpForce, wallJumpForce);
        isWallJumping = true;
        isWallSliding = false;
        jumpController.jumpCount--;
        PlayerWallJumpFlip();
    }

    private IEnumerator CancelPlayerWallJump()
    {
        yield return new WaitForSeconds(wallJumpLenght);
        isWallJumping = false;
        movementController.PlayerFlip();
    }

    private void PlayerWallJumpFlip()
    {
        transform.localScale = new Vector2(-moveVector.x, 1);
    }

    private void OnDrawGizmosSelected()
    {
        if (wallCheckPoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(wallCheckPoint.position, wallCheckRadius);
    }

    public bool IsWalled => isWalled;
    public bool IsWallSliding => isWallSliding;
    public bool IsWallJumping => isWallJumping;
}
