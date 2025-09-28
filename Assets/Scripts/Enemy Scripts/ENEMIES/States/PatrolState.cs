using UnityEngine;

public class PatrolState : IEnemyState
{
    private EnemyBaseController enemy;

    public PatrolState(EnemyBaseController enemy) { this.enemy = enemy; }

    public void Enter()
    {
        enemy.animator.SetBool("isMoving", true);
        enemy.animator.SetFloat("WalkSpeed", 1f);
    }

    public void Update()
    {
        if (enemy.targetPlayer)
        {
            enemy.ChangeState(new ChaseState(enemy));
            return;
        }

        if (enemy.enemyStats.canJump && enemy.jumpController.CanJump && enemy.currentTarget.y > enemy.transform.position.y + 2f)
        {
            enemy.ChangeState(new JumpChargeState(enemy));
            return;
        }

        enemy.MoveToTarget(enemy.currentTarget);
        enemy.Flip(enemy.currentTarget);

        if (Vector2.Distance(enemy.transform.position, enemy.currentTarget) < 0.5f)
        {
            enemy.currentTarget = (enemy.currentTarget == (Vector2)enemy.patrolTarget.position)
                ? enemy.patrolStart
                : enemy.patrolTarget.position;

            enemy.ChangeState(new IdleState(enemy));
        }
    }

    public void Exit()
    {
        enemy.Stop();
    }
}
