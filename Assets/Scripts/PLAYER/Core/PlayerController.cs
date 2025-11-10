using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

#region Require Component
[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerJumpController))]
[RequireComponent(typeof(PlayerWallController))]
[RequireComponent(typeof(PlayerDashController))]
[RequireComponent(typeof(PlayerDamageController))]
[RequireComponent(typeof(PlayerInteractController))]
[RequireComponent(typeof(PlayerAnimationController))]
[RequireComponent(typeof(PlayerCollisionController))]
[RequireComponent(typeof(PlayerPowerUpController))]
#endregion

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Player Stats")]
    public PlayerStatsSO playerBaseStats;
    public PlayerStatsSO playerRuntimeStats;

    [Header("Player Input")]
    public Vector2 moveVector;
    public bool moveInput;
    public bool jumpInput;
    public bool dashInput;
    public bool attackInput;
    public bool interactInput;
    [Space(5)]
    public bool playerInputActive;

    #region Player SubControllers

    private List<IPlayerSubController> subControllers = new();

    private PlayerMovementController movementController;
    private PlayerWallController wallController;
    private PlayerDashController dashController;
    private PlayerAttackController attackController;
    private PlayerInteractController interactController;

    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (GameManager.RuntimePlayerStats == null)
        {
            GameManager.Instance.StartNewGame();
        }

        playerRuntimeStats = GameManager.RuntimePlayerStats ?? playerBaseStats.Clone();

        #region Player SubControllers

        movementController = GetComponent<PlayerMovementController>();
        wallController = GetComponent<PlayerWallController>();
        dashController = GetComponent<PlayerDashController>();
        attackController = GetComponentInChildren<PlayerAttackController>();
        interactController = GetComponent<PlayerInteractController>();

        subControllers.AddRange(GetComponents<IPlayerSubController>());
        #endregion
    }

    private void Start()
    {
        foreach (var sub in subControllers)
            sub.Initialize(playerRuntimeStats);

        playerInputActive = true;
    }

    private void Update()
    {
        if (!playerInputActive) return;

        movementController.SetMoveInput(moveVector, moveInput);
        wallController.SetWallInput(moveVector, moveInput);
        dashController.SetDashInput(dashInput);
        attackController.SetAttackInput(attackInput, moveVector);
        interactController.SetInteractInput(interactInput);

        if (jumpInput)
        {
            if (wallController.IsWallSliding)
                wallController.SetWallJumpInput(true);
            else
                GetComponent<PlayerJumpController>().SetJumpInput(true);

            jumpInput = false;
        }

        dashInput = attackInput = interactInput = false;
    }

    #region Player Input Callbacks
    public void PlayerInputMove(InputAction.CallbackContext ctx)
    {
        moveVector = ctx.ReadValue<Vector2>();
        moveInput = ctx.performed;
    }

    public void PlayerInputJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) jumpInput = true;
    }

    public void PlayerInputDash(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) dashInput = true;
    }

    public void PlayerInputAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) attackInput = true;
    }

    public void PlayerInputInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) interactInput = true;
    }
    #endregion

    #region Player Input Lock Controls
    public void FreezeAllInputs()
    {
        playerInputActive = false;
        moveVector = Vector2.zero;
        moveInput = jumpInput = dashInput = attackInput = interactInput = false;

        movementController.SetMoveInput(Vector2.zero, false);
        wallController.SetWallInput(Vector2.zero, false);
        dashController.SetDashInput(false);
        attackController.SetAttackInput(false, Vector2.zero);
        interactController.SetInteractInput(false);
    }

    public void UnfreezeAllInputs() => playerInputActive = true;
    #endregion
}
