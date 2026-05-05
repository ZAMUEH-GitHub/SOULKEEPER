public class PlayerDashState : PlayerBaseState
{
    private float dashTimer;

    public PlayerDashState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.dashController.ExecuteDash();
        dashTimer = player.dashController.dashLenght;

        player.animController.SetBool("isDashing", true);
    }

    public override void Exit()
    {
        player.animController.SetBool("isDashing", false);
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

    public override void FixedUpdate() { }
}