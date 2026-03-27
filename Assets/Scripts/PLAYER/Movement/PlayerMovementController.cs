using UnityEngine;

public class PlayerMovementController : MonoBehaviour, IPlayerSubController
{
    private PlayerStatsSO playerStats;

    [Header("Movement Settings")]
    public float playerSpeed => playerStats.speed;
    public Vector2 playerOrientation;
    public bool isMoving;

    private Rigidbody2D playerRB;
    private PlayerWallController wallController;

    public void Initialize(PlayerStatsSO stats) => playerStats = stats;

    #region Unity Lifecycle
    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();

        wallController = PlayerController.Instance.wallController;
    }

    private void Update()
    {
        playerOrientation = PlayerController.Instance.moveVector;
        isMoving = PlayerController.Instance.moveInput;
    }
    #endregion

    #region Movement & Flip Logic
    public void HandleFlip()
    {
        PlayerFlip();
    }

    public void ExecuteMove()
    {
        PlayerMove();
    }

    private void PlayerMove()
    {
        if (wallController.IsWallJumping) return;
        playerRB.linearVelocity = new Vector2(playerOrientation.x * playerSpeed, playerRB.linearVelocityY);
    }

    public void PlayerFlip()
    {
        if (wallController.IsWallJumping) return;
        if (playerOrientation.x > 0) transform.localScale = new Vector2(1, 1);
        else if (playerOrientation.x < 0) transform.localScale = new Vector2(-1, 1);
    }
    #endregion

    public bool IsMoving => isMoving;
}