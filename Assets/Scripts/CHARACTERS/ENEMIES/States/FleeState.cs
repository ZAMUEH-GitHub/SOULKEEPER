using UnityEngine;

public class FleeState : EnemyBaseState
{
    private float safeDistance;
    private float fleeDirection;

    public FleeState(EnemyBaseController enemy) : base(enemy)
    {
        safeDistance = enemy.enemyStats.visionRange * 1.5f;
    }

    public override void Enter()
    {
        if (enemy.animController != null)
        {
            enemy.animController.SetMoving(true);
        }
    }

    public override void Update()
    {
        if (TryEnterDeathState()) return;

        if (player == null)
        {
            enemy.stateMachine.ChangeState(new PatrolState(enemy));
            return;
        }

        fleeDirection = Mathf.Sign(enemy.transform.position.x - player.position.x);

        Vector2 fleeTarget = new Vector2(enemy.transform.position.x + fleeDirection * 5f, enemy.transform.position.y);

        float fleeSpeed = enemy.enemyStats.speed * (enemy.enemyStats.canRun ? enemy.enemyStats.speedMultiplier : 1.2f);

        if (enemy.animController != null)
        {
            enemy.animController.SetWalkSpeed(fleeSpeed / enemy.enemyStats.speed);
        }

        enemy.MoveToTarget(fleeTarget, fleeSpeed);
        enemy.Flip(fleeTarget);

        if (enemy.enemyStats.canJump && enemy.jumpController != null && enemy.jumpController.CanJump && enemy.currentTarget.y > enemy.transform.position.y + 2f)
        {
            enemy.stateMachine.ChangeState(new AirborneState(enemy));
            return;
        }

        float distanceToPlayer = Vector2.Distance(enemy.transform.position, player.position);
        if (distanceToPlayer > safeDistance)
        {
            if (enemy.canPatrol)
                enemy.stateMachine.ChangeState(new PatrolState(enemy));
            else
                enemy.stateMachine.ChangeState(new IdleState(enemy));
        }
    }

    public override void Exit()
    {
        enemy.Stop();
        if (enemy.animController != null)
        {
            enemy.animController.SetMoving(false);
            enemy.animController.SetWalkSpeed(1f);
        }
    }
}