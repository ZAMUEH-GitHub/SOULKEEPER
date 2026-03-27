public class PlayerAirborneState : PlayerBaseState
{
    public PlayerAirborneState(PlayerController player) : base(player) { }

    public override void Update()
    {
        player.attackController.UpdateAttackState();
        player.movementController.HandleFlip();

        player.wallController.UpdateWallState();
        player.jumpController.UpdateJumpState();

        player.dashController.UpdateDashState();

        if (player.dashController.IsDashing)
        {
            player.stateMachine.ChangeState(player.dashState);
            return;
        }

        if (player.wallController.IsWallSliding)
        {
            player.stateMachine.ChangeState(player.wallSlideState);
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