using UnityEngine;

public class ComboAttackState : IEnemyState
{
    private EnemyBaseController enemy;
    private int currentAttackStep;
    private bool cancelCombo;

    public ComboAttackState(EnemyBaseController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        currentAttackStep = 1;
        cancelCombo = false;
        enemy.attackController.isAttacking = true;

        enemy.Stop();
        enemy.rigidBody.linearVelocity = Vector2.zero;

        enemy.animator.SetTrigger("EnemyAttack1");
    }

    public void Update()
    {
        if (!enemy.targetPlayer || Vector2.Distance(enemy.transform.position, enemy.player.position) > enemy.enemyStats.attackRange)
        {
            cancelCombo = true;
        }
    }

    public void NextComboStep()
    {
        if (cancelCombo)
        {
            enemy.ChangeState(enemy.targetPlayer ? new ChaseState(enemy) : new PatrolState(enemy));
            return;
        }

        currentAttackStep++;

        if (currentAttackStep == 2)
        {
            enemy.Stop();
            enemy.rigidBody.linearVelocity = Vector2.zero;
            enemy.animator.SetTrigger("EnemyAttack2");
        }
        else if (currentAttackStep == 3)
        {
            enemy.Stop();
            enemy.rigidBody.linearVelocity = Vector2.zero;
            enemy.animator.SetTrigger("EnemyAttack3");
        }
        else
        {
            enemy.ChangeState(enemy.targetPlayer ? new ChaseState(enemy) : new PatrolState(enemy));
        }
    }

    public void Exit()
    {
        enemy.attackController.isAttacking = false;
    }
}
