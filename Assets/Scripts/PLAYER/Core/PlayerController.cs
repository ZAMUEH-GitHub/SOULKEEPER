using System.Collections;
using Unity.VisualScripting;
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
    public PlayerStatsSO playerBaseStats;
    public PlayerStatsSO playerRuntimeStats;
    public static PlayerController Instance { get; private set; }

    [Header("Player Input")]
    public Vector2 moveVector;  
    public bool moveInput;
    public bool jumpInput;
    public bool dashInput;
    public bool attackInput;
    public bool interactInput;
    [Space(5)]
    public bool playerInputActive;

    #region Player Script & Component References

    private PlayerMovementController movementController;
    private PlayerJumpController jumpController;
    private PlayerWallController wallController;
    private PlayerDashController dashController;
    private PlayerAttackController attackController;
    private PlayerInteractController interactController;

    #endregion

    private void Awake()
    {
        Instance = this;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (playerRuntimeStats == null)
            playerRuntimeStats = playerBaseStats.Clone();
    }

    void Start()
    {
        #region Player Script & Component Subscriptions

        movementController = GetComponent<PlayerMovementController>();
        jumpController = GetComponent<PlayerJumpController>();
        wallController = GetComponent<PlayerWallController>();
        dashController = GetComponent<PlayerDashController>();
        attackController = GetComponentInChildren<PlayerAttackController>();
        interactController = GetComponent<PlayerInteractController>();

        #endregion

        playerInputActive = true;
    }

    void Update()
    {
        if (!playerInputActive) { return; }
        
        movementController.SetMoveInput(moveVector, moveInput);
        wallController.SetWallInput(moveVector, moveInput);
        dashController.SetDashInput(dashInput);
        attackController.SetAttackInput(attackInput, moveVector);
        interactController.SetInteractInput(interactInput);

        if (jumpInput)
        {
            if (wallController.IsWallSliding)
            {
                wallController.SetWallJumpInput(jumpInput);
            }
            else
            {
                jumpController.SetJumpInput(jumpInput);
            }

            jumpInput = false;
        }

        if (dashInput)
            dashInput = false;

        if (attackInput)
            attackInput = false;

        if (interactInput)
            interactInput = false; 
    }

    #region Player Input Management

    public void PlayerInputMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveVector = context.ReadValue<Vector2>();
            moveInput = true;
        }
        else if (context.canceled)
        {
            moveVector = Vector2.zero;
            moveInput = false;
        }
    }

    public void PlayerInputJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpInput = true;
        }
    }

    public void PlayerInputDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            dashInput = true;
        }
    }
    
    public void PlayerInputAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            attackInput = true;
        }
    }

    public void PlayerInputInteract(InputAction.CallbackContext context)
    { 
        if (context.performed)
        {
            interactInput = true;
        }
    }

    #endregion

    #region Player Input Lock Controls

    public void FreezeAllInputs()
    {
        playerInputActive = false;

        moveVector = Vector2.zero;
        moveInput = false;
        jumpInput = false;
        dashInput = false;
        attackInput = false;
        interactInput = false;

        movementController.SetMoveInput(Vector2.zero, false);
        wallController.SetWallInput(Vector2.zero, false);
        dashController.SetDashInput(false);
        attackController.SetAttackInput(false, Vector2.zero);
        interactController.SetInteractInput(false);
    }

    public void UnfreezeAllInputs()
    {
        playerInputActive = true;
    }

    #endregion
}