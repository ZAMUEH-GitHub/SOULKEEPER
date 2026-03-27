public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        // Any specific grounded resets can go here
    }

    public override void Update()
    {
        player.attackController.UpdateAttackState();
        player.movementController.HandleFlip();
        player.jumpController.UpdateJumpState();
        player.dashController.UpdateDashState();
        player.wallController.UpdateWallState();

        if (player.damageController.isKnockedBack)
        {
            // player.stateMachine.ChangeState(player.knockbackState); 
            return;
        }

        if (player.dashController.IsDashing)
        {
            player.stateMachine.ChangeState(player.dashState);
            return;
        }

        if (!player.jumpController.isGrounded)
        {
            player.stateMachine.ChangeState(player.airborneState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        player.movementController.ExecuteMove();
    }
}