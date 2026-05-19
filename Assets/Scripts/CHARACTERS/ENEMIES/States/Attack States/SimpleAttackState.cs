using UnityEngine;

public class SimpleAttackState : EnemyBaseState
{
    public SimpleAttackState(EnemyBaseController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.RequestMovementLock("Attack");

        enemy.attackController.isAttacking = true;
        if (enemy.animController != null)
        {
            enemy.animController.SetAttacking(true);

            if (enemy.animController.animator != null)
            {
                int randomAttackIndex = Random.Range(0, enemy.enemyStats.simpleAttackVariations);
                enemy.animController.animator.SetInteger("AttackIndex", randomAttackIndex);
            }

            enemy.animController.TriggerEnemyAttack();
        }
    }

    public override void Update()
    {
        if (TryEnterDeathState()) return;

        float direction = Mathf.Sign(enemy.transform.localScale.x);
        enemy.rigidBody.linearVelocity = new Vector2(direction * (enemy.enemyStats.speed * enemy.moveSpeedMultiplier), enemy.rigidBody.linearVelocity.y);
    }

    public override void Exit()
    {
        enemy.ReleaseMovementLock("Attack");

        enemy.nextAttackTimer = enemy.enemyStats.attackRate;

        if (enemy.attackController != null)
        {
            enemy.attackController.isAttacking = false;
        }

        if (enemy.animController != null)
        {
            enemy.animController.SetAttacking(false);
        }

        enemy.moveSpeedMultiplier = 1f;
    }

    public void AnimationFinished()
    {
        EndAttack();
    }

    private void EndAttack()
    {
        if (enemy.targetPlayer)
            enemy.stateMachine.ChangeState(new ChaseState(enemy));
        else
            enemy.stateMachine.ChangeState(new PatrolState(enemy));
    }
}