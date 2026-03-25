using UnityEngine;

public class PatrolState : EnemyBaseState
{
    public PatrolState(EnemyBaseController enemy) : base(enemy) { }

    public override void Enter()
    {
        if (enemy.animator != null)
        {
            enemy.animator.SetBool("isMoving", true);
            enemy.animator.SetFloat("WalkSpeed", 1f);
        }
    }

    public override void Update()
    {
        if (TryEnterDeathState()) return;

        if (enemy.targetPlayer)
        {
            enemy.stateMachine.ChangeState(new ChaseState(enemy));
            return;
        }

        enemy.MoveToTarget(enemy.currentTarget);
        enemy.Flip(enemy.currentTarget);

        if (Vector2.Distance(enemy.transform.position, enemy.currentTarget) < 0.5f)
        {
            if (enemy.patrolController != null)
                enemy.currentTarget = enemy.patrolController.GetNextPatrolPoint();
            else
                enemy.currentTarget = enemy.patrolStart;

            enemy.stateMachine.ChangeState(new IdleState(enemy));
            return;
        }

        if (enemy.enemyStats.canJump && enemy.jumpController != null && enemy.jumpController.CanJump && enemy.currentTarget.y > enemy.transform.position.y + 2f)
        {
            enemy.stateMachine.ChangeState(new AirborneState(enemy));
            return;
        }
    }

    public override void Exit()
    {
        enemy.Stop();
        if (enemy.animator != null)
            enemy.animator.SetBool("isMoving", false);
    }
}