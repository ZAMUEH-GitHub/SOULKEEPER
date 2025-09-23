using UnityEngine;
using System.Collections;

public class PlayerWallControllerTEST : MonoBehaviour
{
    [Header("Wall Settings")]
    public bool isWalled;
    public bool isWallSliding;
    public float wallSlidingSpeed;

    [Header("Wall Jump Settings")]
    public float wallJumpForce;
    public bool isWallJumping;
    public float wallJumpLenght;
    public float wallJumpDivider;
    [Space(5)]
    public float wallJumpRate;
    public float nextWallJump;

    [Header("Wall Jump Buffer")]
    public float bufferTime;
    public float bufferCount;

    private Vector2 moveVector;
    private bool moveInput;
    private bool wallJumpInput;

    private PlayerMovementControllerTEST movementController;
    private PlayerJumpControllerTEST jumpController;
    private Rigidbody2D playerRB;

    private void Awake()
    {
        movementController = GetComponent<PlayerMovementControllerTEST>();
        jumpController = GetComponent<PlayerJumpControllerTEST>();
        playerRB = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
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

        PlayerWallSlide();

        if (isWallSliding && bufferCount > 0 && !jumpController.isGrounded && jumpController.jumpCount > 0 && nextWallJump <= 0)
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
        PlayerWallJump(jumpInput);
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
        if (isWalled && !jumpController.isGrounded && moveVector.x != 0 && !jumpController.isJumping)
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocityX, Mathf.Clamp(playerRB.linearVelocityY, -wallSlidingSpeed, float.MaxValue));
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Obstacle") || collision.CompareTag("MovingObstacle"))
        {
            isWalled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Obstacle") || collision.CompareTag("MovingObstacle"))
        {
            isWalled = false;
        }
    }

    public bool IsWalled => isWalled;
    public bool IsWallSliding => isWallSliding;
    public bool IsWallJumping => isWallJumping;
}
