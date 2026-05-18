using UnityEngine;

public class StunnedState : EnemyBaseState
{
    private float stunTimer;

    public StunnedState(EnemyBaseController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.Stop();
        enemy.RequestMovementLock("Stun");

        stunTimer = enemy.enemyStats.stunDuration;

        if (enemy.animController != null)
        {
            enemy.animController.SetStunned(true);
        }
    }

    public override void Update()
    {
        if (TryEnterDeathState()) return;

        stunTimer -= Time.deltaTime;

        if (stunTimer <= 0)
        {
            if (enemy.targetPlayer)
                enemy.stateMachine.ChangeState(new ChaseState(enemy));
            else
                enemy.stateMachine.ChangeState(new PatrolState(enemy));
        }
    }

    public override void Exit()
    {
        enemy.ReleaseMovementLock("Stun");

        if (enemy.animController != null)
        {
            enemy.animController.SetStunned(false);
        }
    }
}