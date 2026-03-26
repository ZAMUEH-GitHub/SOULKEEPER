public class PlayerDeathState : PlayerBaseState
{
    public PlayerDeathState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        // Zero out inputs just to be absolutely certain
        player.movementController.SetMoveInput(UnityEngine.Vector2.zero, false);
    }

    public override void Update()
    {
        // Rest in peace. Do absolutely nothing.
    }

    public override void FixedUpdate()
    {
        // Do absolutely nothing.
    }
}