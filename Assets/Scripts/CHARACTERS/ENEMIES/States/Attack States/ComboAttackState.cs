using UnityEngine;

public class ComboAttackState : EnemyBaseState
{
    private float attackTimer;
    private float graceTimer;
    private int comboStep = 1;
    private bool inGracePeriod = false;

    private float comboGraceDuration = 0.5f;

    public ComboAttackState(EnemyBaseController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.Stop();
        enemy.RequestMovementLock("Attack");

        comboStep = 1;
        ExecuteAttack();
    }

    public override void Update()
    {
        if (TryEnterDeathState()) return;

        if (!inGracePeriod)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
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
        }
        else
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

        if (enemy.animator != null)
        {
            enemy.animator.SetBool("isAttacking", false);
        }
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

        if (enemy.animator != null)
        {
            enemy.animator.SetBool("isAttacking", true);
            enemy.animator.SetTrigger("EnemyAttack" + comboStep);
        }

        attackTimer = enemy.enemyStats.attackRate;
    }

    private void EndAttack()
    {
        if (enemy.targetPlayer)
            enemy.stateMachine.ChangeState(new ChaseState(enemy));
        else
            enemy.stateMachine.ChangeState(new PatrolState(enemy));
    }
}