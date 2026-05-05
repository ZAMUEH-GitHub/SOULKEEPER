public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.jumpController.jumpCount = player.jumpController.maxJumpCount;
    }

    public override void Update()
    {
        player.movementController.HandleFlip();

        player.attackController.UpdateAttackState();

        if (player.damageController.isKnockedBack)
        {
            // player.stateMachine.ChangeState(player.knockbackState); 
            return;
        }

        if (player.dashController.CanDash() && player.ConsumeDashInput())
        {
            player.stateMachine.ChangeState(player.dashState);
            return;
        }

        if (player.jumpController.jumpCount > 0 && player.ConsumeJumpInput())
        {
            player.jumpController.ExecuteJump();
            player.stateMachine.ChangeState(player.airborneState);
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