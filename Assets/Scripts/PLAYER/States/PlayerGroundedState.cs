public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        // Any specific grounded resets can go here
    }

    public override void Update()
    {
        // 1. Pass inputs to controllers
        player.movementController.SetMoveInput(player.moveVector, player.moveInput);
        player.attackController.SetAttackInput(player.attackInput, player.moveVector);
        player.dashController.SetDashInput(player.dashInput);

        if (player.jumpInput) player.jumpController.SetJumpInput(true);
        if (player.interactInput) player.interactController.SetInteractInput(true);

        // 2. Execute non-physics logic
        player.movementController.HandleFlip();
        player.jumpController.UpdateJumpState();
        player.dashController.UpdateDashState();

        // 3. Handle Transitions
        if (player.damageController.isKnockedBack)
        {
            // Future-proofing for your damage state
            // player.stateMachine.ChangeState(new PlayerKnockbackState(player)); 
            return;
        }

        if (player.dashController.IsDashing)
        {
            player.stateMachine.ChangeState(new PlayerDashState(player));
            return;
        }

        // If we jumped or fell off a ledge
        if (!player.jumpController.isGrounded)
        {
            player.stateMachine.ChangeState(new PlayerAirborneState(player));
            return;
        }
    }

    public override void FixedUpdate()
    {
        // Execute grounded physics
        player.movementController.ExecuteMove();
    }
}