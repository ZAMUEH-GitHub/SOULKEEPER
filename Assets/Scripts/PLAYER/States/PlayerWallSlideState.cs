public class PlayerWallSlideState : PlayerBaseState
{
    public PlayerWallSlideState(PlayerController player) : base(player) { }

    public override void Update()
    {
        // 1. Pass inputs
        player.wallController.SetWallInput(player.moveVector, player.moveInput);

        if (player.jumpInput) player.wallController.SetWallJumpInput(true);

        // 2. Execute wall logic checks
        player.wallController.UpdateWallState();

        // 3. Handle Transitions
        if (player.wallController.IsWallJumping || !player.wallController.IsWallSliding)
        {
            player.stateMachine.ChangeState(new PlayerAirborneState(player));
            return;
        }

        if (player.jumpController.isGrounded)
        {
            player.stateMachine.ChangeState(new PlayerGroundedState(player));
            return;
        }
    }

    public override void FixedUpdate()
    {
        // Execute wall sliding friction/gravity
        player.wallController.ExecuteWallPhysics();
    }
}