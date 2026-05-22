using UnityEngine;

public class ChargeAttackState : EnemyBaseState
{
    private float lungeTimer;
    private bool isLunging;
    private bool hasImpacted;

    public ChargeAttackState(EnemyBaseController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.RequestMovementLock("Attack");

        enemy.attackController.hasHitObstacle = false;
        enemy.attackController.isChargingAttack = false;
        enemy.attackController.isAttacking = false;

        isLunging = false;
        hasImpacted = false;

        if (enemy.animController != null)
            enemy.animController.SetChargingAttack(true);
    }

    public override void Update()
    {
        if (TryEnterDeathState()) return;

        float direction = Mathf.Sign(enemy.transform.localScale.x);

        if (hasImpacted)
        {
            enemy.rigidBody.linearVelocity = new Vector2(0, enemy.rigidBody.linearVelocity.y);
            return;
        }

        if (isLunging)
        {
            if (enemy.attackController.hasHitObstacle)
            {
                hasImpacted = true;
                isLunging = false;
                enemy.attackController.isAttacking = false;

                enemy.rigidBody.linearVelocity = new Vector2(0, enemy.rigidBody.linearVelocity.y);

                if (enemy.animController != null)
                {
                    enemy.animController.SetAttacking(false);
                    enemy.animController.SetImpacting(true);
                }
                return;
            }

            lungeTimer -= Time.deltaTime;
            enemy.rigidBody.linearVelocity = new Vector2(direction * (enemy.enemyStats.chargeAttackSpeed * enemy.moveSpeedMultiplier), enemy.rigidBody.linearVelocity.y);

            if (lungeTimer <= 0)
            {
                EndAttack();
            }
        }
        else
        {
            enemy.rigidBody.linearVelocity = new Vector2(direction * (enemy.enemyStats.speed * enemy.moveSpeedMultiplier), enemy.rigidBody.linearVelocity.y);
        }
    }

    public void StartLunge()
    {
        isLunging = true;
        lungeTimer = enemy.enemyStats.chargeAttackDuration;

        enemy.attackController.isAttacking = true;

        if (enemy.animController != null)
        {
            enemy.animController.SetChargingAttack(false);
            enemy.animController.SetAttacking(true);
        }
    }

    public void ImpactFinished()
    {
        enemy.stateMachine.ChangeState(new StunnedState(enemy));
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
            enemy.animController.SetImpacting(false);
        }

        enemy.moveSpeedMultiplier = 1f;
        enemy.nextAttackTimer = enemy.enemyStats.attackRate;
    }

    private void EndAttack()
    {
        if (enemy.targetPlayer)
            enemy.stateMachine.ChangeState(new ChaseState(enemy));
        else
            enemy.stateMachine.ChangeState(new PatrolState(enemy));
    }
}