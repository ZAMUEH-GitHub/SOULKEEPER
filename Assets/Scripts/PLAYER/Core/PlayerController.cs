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

    [Header("Debug States")]
    [SerializeField] private string currentState;

    [Header("Player Input")]
    public Vector2 moveVector;
    public bool moveInput;
    public bool jumpInput;
    public bool dashInput;
    public bool attackInput;
    public bool interactInput;
    [Space(5)]
    public bool playerInputActive;
    public bool isAlive;

    // --- NEW: State Machine Hub ---
    public PlayerStateMachine stateMachine;

    // CHANGED: Made sub-controllers public so states can access them
    [HideInInspector] public PlayerMovementController movementController;
    [HideInInspector] public PlayerJumpController jumpController;
    [HideInInspector] public PlayerWallController wallController;
    [HideInInspector] public PlayerDashController dashController;
    [HideInInspector] public PlayerAttackController attackController;
    [HideInInspector] public PlayerInteractController interactController;
    [HideInInspector] public PlayerDamageController damageController;
    [HideInInspector] public PlayerAnimationController animController;

    private List<IPlayerSubController> subControllers = new();

    #region Unity Lifecycle
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Debug.LogWarning("[PlayerController] Duplicate instance detected but kept alive by PlayerRoot persistence.");

        var session = SessionManager.Instance;

        if (session != null && session.HasActiveSession)
        {
            playerRuntimeStats = session.RuntimeStats;
        }
        else
        {
            if (playerBaseStats != null)
            {
                playerRuntimeStats = playerBaseStats.Clone();
                Debug.LogWarning("[PlayerController] No active session found! Using a cloned fallback base stats.");
            }
            else
            {
                Debug.LogError("[PlayerController] No PlayerStatsSO assigned or found!");
            }
        }

        #region Player SubControllers
        movementController = GetComponent<PlayerMovementController>();
        jumpController = GetComponent<PlayerJumpController>();
        wallController = GetComponent<PlayerWallController>();
        dashController = GetComponent<PlayerDashController>();
        attackController = GetComponentInChildren<PlayerAttackController>();
        interactController = GetComponent<PlayerInteractController>();
        damageController = GetComponent<PlayerDamageController>();
        animController = GetComponent<PlayerAnimationController>();

        subControllers.AddRange(GetComponents<IPlayerSubController>());
        #endregion

        // --- NEW: Initialize State Machine ---
        stateMachine = new PlayerStateMachine();
    }

    private void Start()
    {
        foreach (var sub in subControllers)
            sub.Initialize(playerRuntimeStats);

        playerInputActive = true;

        stateMachine.Initialize(new PlayerGroundedState(this));
    }

    private void Update()
    {
        if (!playerInputActive) return;

        // --- NEW: Route update to the active state ---
        stateMachine?.Update();

        // --- NEW: Update the inspector debug string ---
        currentState = stateMachine?.CurrentStateName;

        // Clear single-frame inputs after the state machine has processed them
        jumpInput = dashInput = attackInput = interactInput = false;
    }

    private void FixedUpdate()
    {
        if (!playerInputActive) return;

        // --- NEW: Route physics updates to the active state ---
        stateMachine?.FixedUpdate();
    }
    #endregion

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
    }

    public void UnfreezeAllInputs()
    {
        playerInputActive = true;
    }
    #endregion
}