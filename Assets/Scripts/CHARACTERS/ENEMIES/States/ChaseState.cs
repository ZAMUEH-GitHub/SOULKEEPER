using UnityEngine;
using static PlayerAttackController;

public class ChaseState : EnemyBaseState
{
    public ChaseState(EnemyBaseController enemy) : base(enemy) { }

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

        if (!enemy.targetPlayer)
        {
            enemy.lastKnownPlayerPosition = enemy.player.position;
            enemy.stateMachine.ChangeState(new SearchState(enemy));
            return;
        }

        float chaseSpeed = enemy.enemyStats.speed *
            (enemy.enemyStats.canRun ? enemy.enemyStats.speedMultiplier : 1f);

        if (enemy.animController != null)
        {
            enemy.animController.SetWalkSpeed(chaseSpeed / enemy.enemyStats.speed);
        }

        Vector2 targetPos = new Vector2(enemy.player.position.x, enemy.transform.position.y);
        enemy.MoveToTarget(targetPos, chaseSpeed);
        enemy.Flip(enemy.player.position);

        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.player.position);
        if (distanceToPlayer <= enemy.enemyStats.attackRange)
        {
            switch (enemy.enemyStats.attackType)
            {
                case EnemyStatsSO.EnemyAttackType.Simple:
                    enemy.stateMachine.ChangeState(new SimpleAttackState(enemy));
                    break;
                case EnemyStatsSO.EnemyAttackType.Charge:
                    enemy.stateMachine.ChangeState(new ChargeAttackState(enemy));
                    break;
                case EnemyStatsSO.EnemyAttackType.Combo:
                    enemy.stateMachine.ChangeState(new ComboAttackState(enemy));
                    break;
            }
            return;
        }

        if (enemy.enemyStats.canJump && enemy.jumpController != null && enemy.jumpController.CanJump && enemy.player.position.y > enemy.transform.position.y + 2f)
        {
            enemy.stateMachine.ChangeState(new AirborneState(enemy));
            return;
        }
    }

    public override void Exit()
    {
        if (enemy.animController != null)
        {
            enemy.animController.SetMoving(false);
            enemy.animController.SetWalkSpeed(1f);
        }
    }
}