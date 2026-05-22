using UnityEngine;

public abstract class PlayerBaseState : IPlayerState
{
    protected PlayerController player;

    protected PlayerBaseState(PlayerController player)
    {
        this.player = player;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }

    protected bool CheckGlobalTransitions()
    {
        if (!player.isAlive && player.stateMachine.CurrentStateName != "PlayerDeathState")
        {
            player.stateMachine.ChangeState(new PlayerDeathState(player));
            return true;
        }

        if (player.damageController.isKnockedBack && player.stateMachine.CurrentStateName != "PlayerKnockbackState")
        {
            player.stateMachine.ChangeState(new PlayerKnockbackState(player));
            return true;
        }

        return false;
    }
}