public class PlayerWallSlideState : PlayerBaseState
{
    public PlayerWallSlideState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.animController.SetBool("isWallSliding", true);
    }

    public override void Exit()
    {
        player.animController.SetBool("isWallSliding", false);
    }

    public override void Update()
    {
        player.wallController.UpdateWallState();

        if (player.wallController.IsWallJumping || !player.wallController.IsWallSliding)
        {
            player.stateMachine.ChangeState(player.airborneState);
            return;
        }

        if (player.jumpController.isGrounded)
        {
            player.stateMachine.ChangeState(player.groundedState);
            return;
        }
    }

    public override void FixedUpdate() => player.wallController.ExecuteWallPhysics();
}