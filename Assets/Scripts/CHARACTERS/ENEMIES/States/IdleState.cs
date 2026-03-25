using UnityEngine;

public class IdleState : EnemyBaseState
{
    private float idleTimer;

    public IdleState(EnemyBaseController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.Stop();
        idleTimer = enemy.enemyStats.idleDuration;

        if (enemy.animController != null)
            enemy.animController.SetMoving(false);
    }

    public override void Update()
    {
        if (TryEnterDeathState()) return;

        if (enemy.targetPlayer)
        {
            enemy.stateMachine.ChangeState(new ChaseState(enemy));
            return;
        }

        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0)
        {
            enemy.stateMachine.ChangeState(new PatrolState(enemy));
        }
    }

    public override void Exit() { }
}