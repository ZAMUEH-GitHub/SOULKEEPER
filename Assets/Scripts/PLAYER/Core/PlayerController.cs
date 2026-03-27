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
    [Space(5)]
    public bool playerInputActive;
    public bool isAlive;

    [Header("Input Buffers")]
    public float jumpBufferTimer;
    public float dashBufferTimer;
    public float attackBufferTimer;
    public float interactBufferTimer;

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

        jumpBufferTimer = Mathf.Max(0, jumpBufferTimer - Time.deltaTime);
        dashBufferTimer = Mathf.Max(0, dashBufferTimer - Time.deltaTime);
        attackBufferTimer = Mathf.Max(0, attackBufferTimer - Time.deltaTime);
        interactBufferTimer = Mathf.Max(0, interactBufferTimer - Time.deltaTime);

        stateMachine?.Update();

        currentState = stateMachine?.CurrentStateName;
    }

    private void FixedUpdate()
    {
        if (!playerInputActive) return;

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
        if (ctx.performed) jumpBufferTimer = playerRuntimeStats.bufferTime;
    }

    public void PlayerInputDash(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) dashBufferTimer = playerRuntimeStats.bufferTime;
    }

    public void PlayerInputAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) attackBufferTimer = playerRuntimeStats.bufferTime;
    }

    public void PlayerInputInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) interactBufferTimer = playerRuntimeStats.bufferTime;
    }
    #endregion

    #region Consume Methods
    public bool ConsumeJumpInput()
    {
        if (jumpBufferTimer > 0) { jumpBufferTimer = 0; return true; }
        return false;
    }

    public bool ConsumeDashInput()
    {
        if (dashBufferTimer > 0) { dashBufferTimer = 0; return true; }
        return false;
    }

    public bool ConsumeAttackInput()
    {
        if (attackBufferTimer > 0) { attackBufferTimer = 0; return true; }
        return false;
    }

    public bool ConsumeInteractInput()
    {
        if (interactBufferTimer > 0) { interactBufferTimer = 0; return true; }
        return false;
    }
    #endregion

    #region Player Input Lock Controls
    public void FreezeAllInputs()
    {
        moveVector = Vector2.zero;
        playerInputActive = false;

        // Wipe buffers on freeze
        jumpBufferTimer = dashBufferTimer = attackBufferTimer = interactBufferTimer = 0;
    }

    public void UnfreezeAllInputs()
    {
        playerInputActive = true;
    }
    #endregion
}