public class PlayerDashState : PlayerBaseState
{
    private float dashTimer;

    public PlayerDashState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        dashTimer = player.dashController.dashLenght;
    }

    public override void Update()
    {
        dashTimer -= UnityEngine.Time.deltaTime;

        if (dashTimer <= 0f)
        {
            player.dashController.EndDash();

            if (player.jumpController.isGrounded)
            {
                player.stateMachine.ChangeState(player.groundedState);
            }
            else
            {
                player.stateMachine.ChangeState(player.airborneState);
            }
        }
    }

    public override void FixedUpdate()
    {
        // The DashController directly applies velocity when it starts, 
        // so no extra physics calls are needed here.
    }
}