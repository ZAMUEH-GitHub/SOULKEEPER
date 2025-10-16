using UnityEngine;

public class IdleState : IEnemyState
{
    private EnemyBaseController enemy;
    private float idleTimer;

    public IdleState(EnemyBaseController enemy) { this.enemy = enemy; }

    public void Enter()
    {
        enemy.Stop();
        idleTimer = enemy.enemyStats.idleDuration;
        enemy.animator.SetBool("isMoving", false);
    }

    public void Update()
    {
        idleTimer -= Time.deltaTime;

        if (enemy.targetPlayer)
        {
            enemy.ChangeState(new ChaseState(enemy));
            return;
        }

        if (idleTimer <= 0)
        {
            enemy.ChangeState(new PatrolState(enemy));
        }
    }

    public void Exit() { }
}
