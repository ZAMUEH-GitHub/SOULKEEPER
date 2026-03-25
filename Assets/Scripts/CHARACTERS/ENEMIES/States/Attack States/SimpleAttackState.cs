using UnityEngine;

public class SimpleAttackState : EnemyBaseState
{
    private float attackTimer;

    public SimpleAttackState(EnemyBaseController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.Stop();
        enemy.RequestMovementLock("Attack");

        enemy.attackController.isAttacking = true;
        if (enemy.animator != null) enemy.animator.SetTrigger("EnemyAttack");
        attackTimer = enemy.enemyStats.attackRate;
    }

    public override void Update()
    {
        if (TryEnterDeathState()) return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            EndAttack();
        }
    }

    public override void Exit()
    {
        enemy.ReleaseMovementLock("Attack");

        if (enemy.attackController != null)
        {
            enemy.attackController.isAttacking = false;
        }

        if (enemy.animator != null)
        {
            enemy.animator.SetBool("isAttacking", false);
        }
    }

    private void EndAttack()
    {
        if (enemy.targetPlayer)
            enemy.stateMachine.ChangeState(new ChaseState(enemy));
        else
            enemy.stateMachine.ChangeState(new PatrolState(enemy));
    }
}