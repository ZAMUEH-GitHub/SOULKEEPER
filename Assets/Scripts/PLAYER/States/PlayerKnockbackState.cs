public class PlayerKnockbackState : PlayerBaseState
{
    private float knockbackTimer;

    public PlayerKnockbackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        knockbackTimer = player.damageController.currentKnockbackDuration;
    }

    public override void Update()
    {
        if (base.CheckGlobalTransitions()) return;

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
        // Do nothing. We let the Rigidbody physics handle the knockback force.
    }
}