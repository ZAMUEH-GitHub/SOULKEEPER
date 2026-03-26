public class PlayerDashState : PlayerBaseState
{
    public PlayerDashState(PlayerController player) : base(player) { }

    public override void Update()
    {
        // We do not pass move or jump inputs here, effectively locking them out.
        // We just let the dash controller's internal Coroutine finish its duration.

        if (!player.dashController.IsDashing)
        {
            if (player.jumpController.isGrounded)
            {
                player.stateMachine.ChangeState(new PlayerGroundedState(player));
            }
            else
            {
                player.stateMachine.ChangeState(new PlayerAirborneState(player));
            }
        }
    }

    public override void FixedUpdate()
    {
        // The DashController directly applies velocity during its Coroutine, 
        // so no extra physics calls are needed here.
    }
}