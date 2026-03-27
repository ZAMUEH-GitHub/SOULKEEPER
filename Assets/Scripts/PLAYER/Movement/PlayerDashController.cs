using UnityEngine;
using System.Collections;
using System;

public class PlayerDashController : MonoBehaviour, IPlayerSubController
{
    private PlayerStatsSO playerStats;
    public void Initialize(PlayerStatsSO stats) => playerStats = stats;

    public event Action<bool> OnDashStateChanged;

    public float dashForce => playerStats.dashForce;
    public bool isDashing;
    public Vector2 dashVector;
    public float dashLenght => playerStats.dashLenght;
    public float dashRate => playerStats.dashRate;
    private float nextDash;
    public float bufferTime => playerStats.bufferTime;
    private float bufferCount;

    private Rigidbody2D playerRB;
    private CapsuleCollider2D playerCL;

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerCL = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        bufferCount = Mathf.Max(0, bufferCount - Time.deltaTime);
        nextDash = Mathf.Max(0, nextDash - Time.deltaTime);
    }

    public void UpdateDashState()
    {
        if (PlayerController.Instance.dashInput && !IsDashing)
            bufferCount = bufferTime;

        if (playerStats != null && playerStats.dashUnlocked && bufferCount > 0 && nextDash <= 0)
        {
            DoDash();
            nextDash = dashRate;
            bufferCount = 0;
        }

        dashVector = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    }

    private void DoDash()
    {
        playerRB.linearVelocity = new Vector2(dashVector.x * dashForce, 0);
        playerCL.isTrigger = true;
        isDashing = true;
        playerRB.gravityScale = 0;

        OnDashStateChanged?.Invoke(true);
    }

    public void EndDash()
    {
        isDashing = false;
        playerCL.isTrigger = false;
        playerRB.gravityScale = 1;

        OnDashStateChanged?.Invoke(false);
    }

    public bool IsDashing => isDashing;
}