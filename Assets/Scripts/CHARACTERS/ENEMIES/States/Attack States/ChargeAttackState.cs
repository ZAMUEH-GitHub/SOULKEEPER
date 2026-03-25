using UnityEngine;

public class ChargeAttackState : EnemyBaseState
{
    private float attackTimer;

    public ChargeAttackState(EnemyBaseController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.Stop();
        enemy.RequestMovementLock("Attack");

        enemy.attackController.isChargingAttack = true;
        if (enemy.animator != null) enemy.animator.SetBool("isChargingAttack", true);
        attackTimer = enemy.enemyStats.chargeAttackDuration;
    }

    public override void Update()
    {
        if (TryEnterDeathState()) return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0 && enemy.attackController.isChargingAttack)
        {
            enemy.attackController.isChargingAttack = false;
            enemy.attackController.isAttacking = true;

            if (enemy.animator != null)
            {
                enemy.animator.SetBool("isChargingAttack", false);
                enemy.animator.SetBool("isAttacking", true);
            }

            attackTimer = enemy.enemyStats.chargeAttackDuration;

            float dir = Mathf.Sign(enemy.transform.localScale.x);
            enemy.rigidBody.linearVelocity = new Vector2(dir * enemy.enemyStats.chargeAttackSpeed, enemy.rigidBody.linearVelocity.y);
        }
        else if (attackTimer <= 0 && enemy.attackController.isAttacking)
        {
            EndAttack();
        }
    }

    public override void Exit()
    {
        enemy.ReleaseMovementLock("Attack");

        if (enemy.attackController != null)
        {
            enemy.attackController.isAttacking = false;
            enemy.attackController.isChargingAttack = false;
        }

        if (enemy.animator != null)
        {
            enemy.animator.SetBool("isAttacking", false);
            enemy.animator.SetBool("isChargingAttack", false);
        }
    }

    private void EndAttack()
    {
        if (enemy.targetPlayer)
            enemy.stateMachine.ChangeState(new ChaseState(enemy));
        else
            enemy.stateMachine.ChangeState(new PatrolState(enemy));
    }
}