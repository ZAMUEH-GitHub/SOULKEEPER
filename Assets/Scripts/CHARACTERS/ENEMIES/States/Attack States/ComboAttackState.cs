using UnityEngine;

public class ComboAttackState : IMovementState
{
    private readonly EnemyBaseController enemy;
    private int currentStep;
    private bool cancelCombo;

    public ComboAttackState(EnemyBaseController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.Stop();
        enemy.PauseVertical(true);

        enemy.ResetVerticalStateIfGrounded();

        enemy.attackController.isAttacking = true;
        currentStep = 1;
        cancelCombo = false;

        enemy.animator.SetTrigger("EnemyAttack1");
    }

    public void Update()
    {
        if (!enemy.targetPlayer ||  Vector2.Distance(enemy.transform.position, enemy.player.position) > enemy.enemyStats.attackRange)
        {
            cancelCombo = true;
        }
    }

    public void Exit()
    {
        enemy.attackController.isAttacking = false;

        enemy.PauseVertical(false);
    }

    public void NextComboStep()
    {
        if (cancelCombo)
        {
            enemy.ChangeMovementState(enemy.targetPlayer ? new ChaseState(enemy) : new PatrolState(enemy));
            return;
        }

        currentStep++;

        if (currentStep == 2)
        {
            enemy.Stop();
            enemy.animator.SetTrigger("EnemyAttack2");
        }
        else if (currentStep == 3)
        {
            enemy.Stop();
            enemy.animator.SetTrigger("EnemyAttack3");
        }
        else
        {
            enemy.ChangeMovementState(enemy.targetPlayer ? new ChaseState(enemy) : new PatrolState(enemy));
        }
    }
}
