public class PlayerAirborneState : PlayerBaseState
{
    public PlayerAirborneState(PlayerController player) : base(player) { }

    public override void Update()
    {
        player.attackController.UpdateAttackState();
        player.movementController.HandleFlip();

        player.wallController.UpdateWallState();
        if (player.wallController.IsWallSliding)
        {
            player.stateMachine.ChangeState(player.wallSlideState);
            return;
        }

        if (player.dashController.CanDash() && player.ConsumeDashInput())
        {
            player.stateMachine.ChangeState(player.dashState);
            return;
        }

        if (player.jumpController.CanJump() && player.ConsumeJumpInput())
        {
            player.jumpController.ExecuteJump();
            return;
        }

        if (player.jumpController.isGrounded && player.jumpController.jumpCount > 0)
        {
            player.stateMachine.ChangeState(player.groundedState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        player.movementController.ExecuteMove();
    }
}