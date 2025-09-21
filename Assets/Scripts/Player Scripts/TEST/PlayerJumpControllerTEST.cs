using UnityEngine;
using System.Collections;

public class PlayerJumpControllerTEST : MonoBehaviour
{
    [Header("Jump Parameters")]
    public float jumpForce;
    private bool jumpInput;
    public bool isGrounded;
    public bool isJumping;
    [Space(5)]
    public int jumpCount;
    public int maxJumpCount;
    [Space(5)]
    public float jumpRate;
    public float nextJump;

    [Header("Coyote Time")]
    public float coyoteTime;
    public float coyoteCount;

    [Header("Jump Buffer")]
    public float bufferTime;
    public float bufferCount;

    private Rigidbody2D playerRB;

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
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

        if (nextJump <= 0 && bufferCount > 0 && (isGrounded || jumpCount > 0 || coyoteCount > 0))
        {
            DoJump();
            nextJump = jumpRate;
            bufferCount = 0;
        }

    }

    public void SetJumpInput(bool jumpInput)
    {
        this.jumpInput = jumpInput;
        PlayerJump(jumpInput);
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
        playerRB.linearVelocity = new Vector2(playerRB.linearVelocityX, jumpForce);
        isJumping = true;
        jumpCount--;
        StartCoroutine(CancelPlayerJump());
    }

    private IEnumerator CancelPlayerJump()
    {
        yield return new WaitForSeconds(jumpRate);
        isJumping = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Obstacle") || collision.CompareTag("MovingObstacle") || collision.CompareTag("Bulb"))
        {
            isGrounded = true;
            jumpCount = maxJumpCount;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Obstacle") || collision.CompareTag("MovingObstacle") || collision.CompareTag("Bulb"))
        {
            isGrounded = false;
        }
    }

    public bool IsGrounded => isGrounded;
    public bool IsJumping => isJumping;
    public int JumpCount => jumpCount;
    public int MaxJump => maxJumpCount;
}
