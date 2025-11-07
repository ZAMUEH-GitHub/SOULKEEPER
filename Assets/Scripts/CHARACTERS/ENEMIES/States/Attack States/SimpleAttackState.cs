using UnityEngine;

public class SimpleAttackState : IMovementState
{
    private readonly EnemyBaseController enemy;
    private float attackTimer;

    public SimpleAttackState(EnemyBaseController enemy) { this.enemy = enemy; }

    public void Enter()
    {
        enemy.Stop();
        enemy.PauseVertical(true);

        enemy.ResetVerticalStateIfGrounded();

        enemy.attackController.isAttacking = true;
        enemy.animator.SetTrigger("EnemyAttack");
        attackTimer = enemy.enemyStats.attackRate;
    }

    public void Update()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0) EndAttack();
    }

    public void Exit()
    {
        enemy.PauseVertical(false);
        enemy.attackController.isAttacking = false;
    }

    public void EndAttack()
    {
        if (enemy.targetPlayer)
            enemy.ChangeMovementState(new ChaseState(enemy));
        else
            enemy.ChangeMovementState(new PatrolState(enemy));
    }
}
