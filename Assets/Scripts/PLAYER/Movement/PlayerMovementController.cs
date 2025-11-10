using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private PlayerStatsSO playerStats;

    [Header("Movement Settings")]
    public float playerSpeed => playerStats.speed;
    public Vector2 playerOrientation;
    public bool isMoving;

    private Rigidbody2D playerRB;

    private PlayerDashController dashController;
    private PlayerWallController wallController;
    private PlayerDamageController damageController;

    private void Awake()
    {
        #region Script and Variable Subscriptions

        var controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            playerStats = controller.playerRuntimeStats;
        }

        playerRB = GetComponent<Rigidbody2D>();

        dashController = GetComponent<PlayerDashController>();
        wallController = GetComponent<PlayerWallController>();
        damageController = GetComponent<PlayerDamageController>();

        #endregion
    }

    private void Update()
    {
        PlayerMove();
        PlayerFlip();
    }

    public void SetMoveInput(Vector2 moveVector, bool moveInput)
    {
        playerOrientation = moveVector;
        isMoving = moveInput;
    }

    private void PlayerMove()
    {
        if (damageController.isKnockedBack || dashController.IsDashing || wallController.IsWallJumping) return;

        playerRB.linearVelocity = new Vector2(playerOrientation.x * playerSpeed, playerRB.linearVelocityY);
    }

    public void PlayerFlip()
    {
        if (wallController.IsWallJumping) return;
        
        if (playerOrientation.x > 0)
        {
            transform.localScale = new Vector2(1, 1);
        }
        if (playerOrientation.x < 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }
    }

    public bool IsMoving => isMoving;
}
