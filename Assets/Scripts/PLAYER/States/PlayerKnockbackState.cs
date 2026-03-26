public class PlayerKnockbackState : PlayerBaseState
{
    public PlayerKnockbackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        // Clear out any residual movement inputs so the player doesn't slide 
        // after the knockback force finishes.
        player.movementController.SetMoveInput(UnityEngine.Vector2.zero, false);
    }

    public override void Update()
    {
        if (base.CheckGlobalTransitions()) return; // Catches Death during Knockback

        // Wait for the PlayerDamageController's CancelKnockback coroutine to finish
        if (!player.damageController.isKnockedBack)
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
        // Do nothing. We let the Rigidbody physics handle the knockback force.
    }
}