using UnityEngine;

public class PatrolState : IMovementState
{
    private readonly EnemyBaseController enemy;

    public PatrolState(EnemyBaseController enemy) { this.enemy = enemy; }

    public void Enter()
    {
        enemy.animator.SetBool("isMoving", true);
        enemy.animator.SetFloat("WalkSpeed", 1f);
    }

    public void Update()
    {
        if (enemy.damageController.isKnockedBack) return;

        if (enemy.targetPlayer)
        {
            enemy.ChangeMovementState(new ChaseState(enemy));
            return;
        }

        enemy.MoveToTarget(enemy.currentTarget);
        enemy.Flip(enemy.currentTarget);

        if (Vector2.Distance(enemy.transform.position, enemy.currentTarget) < 0.5f)
        {
            enemy.currentTarget = (enemy.currentTarget == (Vector2)enemy.patrolTarget.position)
                ? enemy.patrolStart
                : enemy.patrolTarget.position;

            enemy.ChangeMovementState(new IdleState(enemy));
        }

        if (enemy.enemyStats.canJump && enemy.jumpController.CanJump &&
            enemy.currentTarget.y > enemy.transform.position.y + 2f)
        {
            enemy.ChangeVerticalState(new JumpChargeState(enemy));
        }
    }

    public void Exit()
    {
        enemy.Stop();
        enemy.animator.SetBool("isMoving", false);
    }
}
