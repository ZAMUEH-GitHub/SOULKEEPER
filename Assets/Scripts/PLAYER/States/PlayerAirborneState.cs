public class PlayerAirborneState : PlayerBaseState
{
    public PlayerAirborneState(PlayerController player) : base(player) { }

    public override void Update()
    {
        // 1. Pass inputs
        player.movementController.SetMoveInput(player.moveVector, player.moveInput);
        player.wallController.SetWallInput(player.moveVector, player.moveInput);
        player.dashController.SetDashInput(player.dashInput);
        player.attackController.SetAttackInput(player.attackInput, player.moveVector);

        if (player.jumpInput) player.jumpController.SetJumpInput(true);

        // 2. Execute non-physics logic
        player.movementController.HandleFlip();
        player.jumpController.UpdateJumpState();
        player.dashController.UpdateDashState();
        player.wallController.UpdateWallState();

        // 3. Handle Transitions
        if (player.dashController.IsDashing)
        {
            player.stateMachine.ChangeState(new PlayerDashState(player));
            return;
        }

        if (player.wallController.IsWallSliding)
        {
            player.stateMachine.ChangeState(new PlayerWallSlideState(player));
            return;
        }

        if (player.jumpController.isGrounded && player.jumpController.jumpCount > 0) // Prevents instantly grounding right as we jump
        {
            player.stateMachine.ChangeState(new PlayerGroundedState(player));
            return;
        }
    }

    public override void FixedUpdate()
    {
        // Execute aerial physics (horizontal movement in air)
        player.movementController.ExecuteMove();
    }
}