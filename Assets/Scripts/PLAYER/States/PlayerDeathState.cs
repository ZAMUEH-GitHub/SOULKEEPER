using UnityEngine;

public class PlayerDeathState : PlayerBaseState
{
    public PlayerDeathState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player.GetComponent<Rigidbody2D>().linearVelocity = UnityEngine.Vector2.zero;
        player.GetComponent<Rigidbody2D>().gravityScale = 0f;

        player.animController.SetBool("isMoving", false);
        player.animController.SetBool("isDashing", false);
        player.animController.SetBool("isWallSliding", false);
        player.animController.SetBool("isFalling", false);

        player.animController.SetBool("isDead", true); 
    }

    public override void Update()
    {

    }

    public override void FixedUpdate() { }
}