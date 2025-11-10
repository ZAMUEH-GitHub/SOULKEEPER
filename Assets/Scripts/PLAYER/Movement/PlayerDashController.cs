using UnityEngine;
using System.Collections;

public class PlayerDashController : MonoBehaviour
{
    [SerializeField] private PlayerStatsSO playerStats;

    [Header("Dash Settings")]
    public float dashForce => playerStats.dashForce;
    private bool dashInput;
    public bool isDashing;
    public Vector2 dashVector;
    public float dashLenght => playerStats.dashLenght;
    public float dashRate => playerStats.dashRate;
    private float nextDash;

    public float bufferTime => playerStats.bufferTime;
    private float bufferCount;

    private Rigidbody2D playerRB;
    private CapsuleCollider2D playerCL;

    private PlayerWallController wallController;

    private void Awake()
    {
        #region Script and Variable Subscriptions

        var controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            playerStats = controller.playerRuntimeStats;
        }

        playerRB = GetComponent<Rigidbody2D>();
        playerCL = GetComponent<CapsuleCollider2D>();

        wallController = GetComponent<PlayerWallController>();

        #endregion
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

        if (playerStats.dashUnlocked)

            if (bufferCount > 0 && nextDash <= 0)
            {
                DoDash();
                nextDash = dashRate;
                bufferCount = 0;
            }

        if (transform.localScale.x > 0)
        {
            dashVector = Vector2.right;
        }
        else dashVector = Vector2.left;
    }

    public void SetDashInput(bool dashInput)
    {
        this.dashInput = dashInput;
        PlayerDash(this.dashInput);
    }

    public void PlayerDash(bool dashInput)
    {
        if (IsDashing || wallController.IsWallSliding) return;

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
