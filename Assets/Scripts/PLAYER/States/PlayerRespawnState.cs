using UnityEngine;

public class PlayerRespawnState : PlayerBaseState
{
    private float wakeUpTimer;

    public PlayerRespawnState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        wakeUpTimer = 0.5f;

        player.GetComponent<Rigidbody2D>().gravityScale = 1f;

        // player.animController.SetTrigger("Respawn");
    }

    public override void Update()
    {
        wakeUpTimer -= UnityEngine.Time.deltaTime;

        if (wakeUpTimer <= 0f)
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