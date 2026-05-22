public class PlayerKnockbackState : PlayerBaseState
{
    private float knockbackTimer;

    public PlayerKnockbackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        knockbackTimer = player.damageController.currentKnockbackDuration;

        player.animController.SetBool("isMoving", false);
        player.animController.SetBool("isDashing", false);
        player.animController.SetBool("isWallSliding", false);
    }

    public override void Exit()
    {

    }

    public override void Update()
    {
        knockbackTimer -= UnityEngine.Time.deltaTime;

        if (knockbackTimer <= 0f)
        {
            player.damageController.EndKnockback();

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

    }
}