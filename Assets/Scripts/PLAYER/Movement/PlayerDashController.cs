using UnityEngine;
using System.Collections;

public class PlayerDashController : MonoBehaviour, IPlayerSubController
{
    private PlayerStatsSO playerStats;
    public void Initialize(PlayerStatsSO stats) => playerStats = stats;

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
        playerRB = GetComponent<Rigidbody2D>();
        playerCL = GetComponent<CapsuleCollider2D>();
        wallController = GetComponent<PlayerWallController>();
    }

    private void Update()
    {
        bufferCount = Mathf.Max(0, bufferCount - Time.deltaTime);
        nextDash = Mathf.Max(0, nextDash - Time.deltaTime);

        if (playerStats != null && playerStats.dashUnlocked && bufferCount > 0 && nextDash <= 0)
        {
            DoDash();
            nextDash = dashRate;
            bufferCount = 0;
        }

        dashVector = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    }

    public void SetDashInput(bool dashInput)
    {
        this.dashInput = dashInput;
        if (!IsDashing && !wallController.IsWallSliding && dashInput)
            bufferCount = bufferTime;
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

    public bool IsDashing => isDashing;
}
