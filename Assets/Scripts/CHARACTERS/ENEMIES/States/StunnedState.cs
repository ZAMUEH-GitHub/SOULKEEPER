using UnityEngine;

public class StunnedState : EnemyBaseState
{
    public StunnedState(EnemyBaseController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.Stop();
        enemy.RequestMovementLock("Stun");

        if (enemy.animController != null)
        {
            enemy.animController.TriggerStun();
        }
    }

    public override void Update()
    {
        if (TryEnterDeathState()) return;
    }

    public override void Exit()
    {
        enemy.ReleaseMovementLock("Stun");
    }

    public void AnimationFinished()
    {
        if (enemy.targetPlayer)
            enemy.stateMachine.ChangeState(new ChaseState(enemy));
        else
            enemy.stateMachine.ChangeState(new PatrolState(enemy));
    }
}