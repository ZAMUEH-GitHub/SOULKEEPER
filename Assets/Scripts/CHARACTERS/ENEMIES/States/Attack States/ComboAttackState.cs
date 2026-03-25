using UnityEngine;

public class ComboAttackState : EnemyBaseState
{
    private float graceTimer;
    private int comboStep = 1;
    private bool inGracePeriod = false;

    private float comboGraceDuration = 0.5f;

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

        if (inGracePeriod)
        {
            graceTimer -= Time.deltaTime;

            float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.player.position);

            if (distanceToPlayer <= enemy.enemyStats.attackRange)
            {
                comboStep++;
                ExecuteAttack();
            }
            else if (graceTimer <= 0)
            {
                EndAttack();
            }
        }
    }

    public override void Exit()
    {
        enemy.ReleaseMovementLock("Attack");

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
        inGracePeriod = false;

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
        if (comboStep >= 3)
        {
            EndAttack();
        }
        else
        {
            inGracePeriod = true;
            graceTimer = comboGraceDuration;

            if (enemy.attackController != null)
                enemy.attackController.isAttacking = false;
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