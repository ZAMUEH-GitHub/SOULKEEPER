using UnityEngine;
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
    [HideInInspector] public float nextDash;

    private Rigidbody2D playerRB;
    private CapsuleCollider2D playerCL;

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerCL = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        nextDash = Mathf.Max(0, nextDash - Time.deltaTime);
        dashVector = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    }

    public bool CanDash()
    {
        return playerStats != null && playerStats.dashUnlocked && nextDash <= 0;
    }

    public void ExecuteDash()
    {
        playerRB.linearVelocity = new Vector2(dashVector.x * dashForce, 0);
        playerCL.isTrigger = true;
        isDashing = true;
        playerRB.gravityScale = 0;
        nextDash = dashRate;

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