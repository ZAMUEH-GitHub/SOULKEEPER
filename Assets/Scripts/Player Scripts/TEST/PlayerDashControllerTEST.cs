using UnityEngine;
using System.Collections;

public class PlayerDashControllerTEST : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashForce;
    private bool dashInput;
    public bool isDashing;
    public Vector2 dashVector;
    public float dashLenght;
    [Space(5)]
    public float dashRate;
    public float nextDash;

    [Header("Dash Buffer")]
    public float bufferTime;
    public float bufferCount;

    private Rigidbody2D playerRB;
    private CapsuleCollider2D playerCL;

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerCL = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        #region Dash Counters

        if (bufferCount > 0)
        {
            bufferCount -= Time.deltaTime;
            if (bufferCount < 0 )
                bufferCount = 0;
        }

        if (nextDash > 0)
        {
            nextDash -= Time.deltaTime;
            if (nextDash < 0 )
                nextDash = 0;
        }
        #endregion

        if (bufferCount > 0 && nextDash <= 0)
        {
            DoDash();
            nextDash = dashRate;
            bufferCount = 0;
        }
    }

    public void SetDashInput(Vector2 moveVector, bool dashInput)
    {
        dashVector = moveVector;
        this.dashInput = dashInput;
        PlayerDash(dashInput);
    }

    public void PlayerDash(bool dashInput)
    {
        if (dashInput)
        {
            bufferCount = bufferTime;
        }
    }

    private void DoDash()
    {
        playerRB.linearVelocity = new Vector2(dashVector.x * dashForce, 0);
        playerCL.isTrigger = true;
        isDashing = true;
        playerRB.gravityScale = 0;
        StartCoroutine(CancelPlayerDash());
    }
    private IEnumerator CancelPlayerDash()
    {
        yield return new WaitForSeconds(dashLenght);
        isDashing = false;
        playerCL.isTrigger = false;
        playerRB.gravityScale = 5;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Wall") || collision.CompareTag("Obstacle"))
        {
            playerCL.isTrigger = false;
        }
    }

    public bool IsDashing => isDashing;
}
