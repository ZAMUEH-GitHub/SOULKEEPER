using UnityEngine;

public class ChaseState : IMovementState
{
    private readonly EnemyBaseController enemy;

    public ChaseState(EnemyBaseController enemy) { this.enemy = enemy; }

    public void Enter() => enemy.animator.SetBool("isMoving", true);

    public void Update()
    {
        if (enemy.damageController.isKnockedBack) return;
        if (!enemy.targetPlayer)
        {
            enemy.ChangeMovementState(new PatrolState(enemy));
            return;
        }

        float chaseSpeed = enemy.enemyStats.speed *
            (enemy.enemyStats.canRun ? enemy.enemyStats.speedMultiplier : 1f);

        enemy.animator.SetFloat("WalkSpeed", chaseSpeed / enemy.enemyStats.speed);
        enemy.MoveToTarget(new Vector2(enemy.player.position.x, enemy.transform.position.y), chaseSpeed);
        enemy.Flip(enemy.player.position);

        if (!enemy.IsInVerticalAction() &&
            Vector2.Distance(enemy.transform.position, enemy.player.position) <= enemy.enemyStats.attackRange)
        {
            if (enemy.enemyStats.hasChargedAttack)
                enemy.ChangeMovementState(new ChargeAttackState(enemy));
            else if (enemy.enemyStats.canComboAttack)
                enemy.ChangeMovementState(new ComboAttackState(enemy));
            else
                enemy.ChangeMovementState(new SimpleAttackState(enemy));

            return;
        }

        if (enemy.enemyStats.canJump && enemy.jumpController.CanJump &&
            enemy.player.position.y > enemy.transform.position.y + 2f)
        {
            enemy.ChangeVerticalState(new JumpChargeState(enemy));
        }
    }

    public void Exit()
    {
        enemy.animator.SetBool("isMoving", false);
        enemy.animator.SetFloat("WalkSpeed", 1f);
    }
}
