using UnityEngine;

public class ChaseState : IEnemyState
{
    private EnemyBaseController enemy;

    public ChaseState(EnemyBaseController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.animator.SetBool("isMoving", true);
    }

    public void Update()
    {
        if (enemy.damageController.isKnockedBack) return;

        if (!enemy.targetPlayer)
        {
            enemy.ChangeState(new PatrolState(enemy));
            return;
        }

        if (enemy.enemyStats.canJump && enemy.jumpController.CanJump && enemy.player.position.y > enemy.transform.position.y + 2f)
        {
            enemy.ChangeState(new JumpChargeState(enemy));
            return;
        }

        float chaseSpeed = enemy.enemyStats.speed;

        if (enemy.enemyStats.canRun)
        {
            chaseSpeed *= enemy.enemyStats.speedMultiplier;
            enemy.animator.SetFloat("WalkSpeed", enemy.enemyStats.speedMultiplier);
        }
        else
        {
            enemy.animator.SetFloat("WalkSpeed", 1f);
        }

        enemy.MoveToTarget(new Vector2(enemy.player.position.x, enemy.transform.position.y), chaseSpeed);
        enemy.Flip(enemy.player.position);

        if (Vector2.Distance(enemy.transform.position, enemy.player.position) <= enemy.enemyStats.attackRange)
        {
            if (enemy.enemyStats.hasChargedAttack)
                enemy.ChangeState(new ChargeAttackState(enemy));
            else if (enemy.enemyStats.canComboAttack)
                enemy.ChangeState(new ComboAttackState(enemy));
            else
                enemy.ChangeState(new SimpleAttackState(enemy));
        }
    }



    public void Exit()
    {
        enemy.animator.SetBool("isMoving", false);
        enemy.animator.SetFloat("WalkSpeed", 1f);
    }
}
