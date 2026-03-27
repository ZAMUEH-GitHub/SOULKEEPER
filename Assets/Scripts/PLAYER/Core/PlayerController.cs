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

    #region Cached Variables and Controllers
    public PlayerStateMachine stateMachine;

    public PlayerGroundedState groundedState;
    public PlayerAirborneState airborneState;
    public PlayerDashState dashState;
    public PlayerWallSlideState wallSlideState;
    public PlayerKnockbackState knockbackState;
    public PlayerDeathState deathState;

    [HideInInspector] public PlayerMovementController movementController;
    [HideInInspector] public PlayerJumpController jumpController;
    [HideInInspector] public PlayerWallController wallController;
    [HideInInspector] public PlayerDashController dashController;
    [HideInInspector] public PlayerAttackController attackController;
    [HideInInspector] public PlayerInteractController interactController;
    [HideInInspector] public PlayerDamageController damageController;
    [HideInInspector] public PlayerAnimationController animController;

    private List<IPlayerSubController> subControllers = new();
    #endregion

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

        #region Player SubControllers & States
        movementController = GetComponent<PlayerMovementController>();
        jumpController = GetComponent<PlayerJumpController>();
        wallController = GetComponent<PlayerWallController>();
        dashController = GetComponent<PlayerDashController>();
        attackController = GetComponentInChildren<PlayerAttackController>();
        interactController = GetComponent<PlayerInteractController>();
        damageController = GetComponent<PlayerDamageController>();
        animController = GetComponent<PlayerAnimationController>();

        subControllers.AddRange(GetComponents<IPlayerSubController>());

        stateMachine = new PlayerStateMachine();

        groundedState = new PlayerGroundedState(this);
        airborneState = new PlayerAirborneState(this);
        dashState = new PlayerDashState(this);
        wallSlideState = new PlayerWallSlideState(this);
        knockbackState = new PlayerKnockbackState(this);
        deathState = new PlayerDeathState(this);
        #endregion
    }

    private void Start()
    {
        foreach (var sub in subControllers)
            sub.Initialize(playerRuntimeStats);

        playerInputActive = true;

        stateMachine.Initialize(groundedState);
    }

    private void Update()
    {
        if (!playerInputActive) return;

        stateMachine?.Update();

        currentState = stateMachine?.CurrentStateName;
    }

    private void FixedUpdate()
    {
        if (!playerInputActive) return;

        stateMachine?.FixedUpdate();
    }

    private void LateUpdate()
    {
        jumpInput = dashInput = attackInput = interactInput = false;
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
        moveVector = Vector2.zero;
        playerInputActive = false;
    }

    public void UnfreezeAllInputs()
    {
        playerInputActive = true;
    }
    #endregion
}