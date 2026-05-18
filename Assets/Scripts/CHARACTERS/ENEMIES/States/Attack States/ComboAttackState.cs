using UnityEngine;

public class ComboAttackState : EnemyBaseState
{
    private int comboStep = 1;

    public ComboAttackState(EnemyBaseController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.RequestMovementLock("Attack");
        comboStep = 1;
        ExecuteAttack();
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

    private void ExecuteAttack()
    {
        if (enemy.player != null)
        {
            enemy.Flip(enemy.player.position);
        }

        if (enemy.attackController != null)
            enemy.attackController.isAttacking = true;

        if (enemy.animController != null)
        {
            enemy.animController.SetAttacking(true);
            enemy.animController.TriggerComboAttack(comboStep);
        }
    }

    public void AnimationFinished()
    {
        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.player.position);

        float requiredRange = enemy.enemyStats.attackRange;

        if (enemy.enemyStats.comboStepRanges != null && (comboStep - 1) < enemy.enemyStats.comboStepRanges.Length)
        {
            requiredRange = enemy.enemyStats.comboStepRanges[comboStep - 1];
        }

        if (comboStep < enemy.enemyStats.maxComboSteps && distanceToPlayer <= requiredRange)
        {
            comboStep++;
            ExecuteAttack();
        }
        else
        {
            EndAttack();
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