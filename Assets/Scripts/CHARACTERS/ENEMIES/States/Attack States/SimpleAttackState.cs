using UnityEngine;

public class SimpleAttackState : IEnemyState
{
    private EnemyBaseController enemy;
    private float attackTimer;

    public SimpleAttackState(EnemyBaseController enemy) { this.enemy = enemy; }

    public void Enter()
    {
        enemy.Stop();

        enemy.attackController.isAttacking = true;

        enemy.animator.SetTrigger("EnemyAttack");

        attackTimer = enemy.enemyStats.attackRate;
    }

    public void Update()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            EndAttack();
        }
    }

    public void Exit()
    {
        enemy.attackController.isAttacking = false;
    }

    public void EndAttack()
    {
        if (enemy.targetPlayer)
            enemy.ChangeState(new ChaseState(enemy));
        else
            enemy.ChangeState(new PatrolState(enemy));
    }
}
