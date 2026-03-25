using UnityEngine;

public class ChargeAttackState : EnemyBaseState
{
    private float attackTimer;

    public ChargeAttackState(EnemyBaseController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.RequestMovementLock("Attack");

        enemy.attackController.isChargingAttack = true;
        enemy.attackController.hasHitObstacle = false;

        if (enemy.animController != null) enemy.animController.SetChargingAttack(true);
        attackTimer = enemy.enemyStats.chargeAttackDuration;
    }

    public override void Update()
    {
        if (TryEnterDeathState()) return;

        if (enemy.attackController.hasHitObstacle)
        {
            enemy.stateMachine.ChangeState(new StunnedState(enemy));
            return;
        }

        float direction = Mathf.Sign(enemy.transform.localScale.x);

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0 && enemy.attackController.isChargingAttack)
        {
            enemy.attackController.isChargingAttack = false;
            enemy.attackController.isAttacking = true;

            if (enemy.animController != null)
            {
                enemy.animController.SetChargingAttack(false);
                enemy.animController.SetAttacking(true);
            }

            attackTimer = enemy.enemyStats.chargeAttackDuration;
        }
        else if (attackTimer <= 0 && enemy.attackController.isAttacking)
        {
            EndAttack();
        }
        else if (enemy.attackController.isChargingAttack)
        {
            enemy.rigidBody.linearVelocity = new Vector2(direction * (enemy.enemyStats.speed * enemy.moveSpeedMultiplier), enemy.rigidBody.linearVelocity.y);
        }
        else if (enemy.attackController.isAttacking)
        {
            enemy.rigidBody.linearVelocity = new Vector2(direction * enemy.enemyStats.chargeAttackSpeed, enemy.rigidBody.linearVelocity.y);
        }
    }

    public override void Exit()
    {
        enemy.ReleaseMovementLock("Attack");

        if (enemy.attackController != null)
        {
            enemy.attackController.isAttacking = false;
            enemy.attackController.isChargingAttack = false;
            enemy.attackController.hasHitObstacle = false;
        }

        if (enemy.animController != null)
        {
            enemy.animController.SetAttacking(false);
            enemy.animController.SetChargingAttack(false);
        }

        enemy.moveSpeedMultiplier = 1f;
    }

    private void EndAttack()
    {
        if (enemy.targetPlayer)
            enemy.stateMachine.ChangeState(new ChaseState(enemy));
        else
            enemy.stateMachine.ChangeState(new PatrolState(enemy));
    }
}