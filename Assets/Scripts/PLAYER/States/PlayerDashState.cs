using UnityEngine;

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
        if (player.dashController.IsDashing)
        {
            player.dashController.EndDash();
        }

        player.animController.SetBool("isDashing", false);
    }

    public override void Update()
    {
        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0f)
        {
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