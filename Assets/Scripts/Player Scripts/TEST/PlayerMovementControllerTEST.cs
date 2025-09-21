using UnityEngine;

public class PlayerMovementControllerTEST : MonoBehaviour
{
    [Header("Movement Settings")]
    public float playerSpeed;
    public Vector2 playerOrientation;
    public bool isMoving;

    private Rigidbody2D playerRB;

    private PlayerDashControllerTEST dashController;
    private PlayerWallControllerTEST wallController;

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();

        dashController = GetComponent<PlayerDashControllerTEST>();
        wallController = GetComponent<PlayerWallControllerTEST>();
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
        if (dashController.IsDashing || wallController.IsWallJumping) return;

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
