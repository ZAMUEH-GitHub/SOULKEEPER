using UnityEngine;

public class AttackingState : IEnemyState
{
    private EnemyBaseController enemy;
    private float attackTimer;

    public AttackingState(EnemyBaseController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.attackController.isAttacking = true;

        enemy.animator.SetBool("isAttacking", true);

        attackTimer = enemy.enemyStats.chargeAttackDuration;

        float dir = Mathf.Sign(enemy.transform.localScale.x);
        enemy.rigidBody.linearVelocity = new Vector2(dir * enemy.enemyStats.chargeAttackSpeed, enemy.rigidBody.linearVelocity.y);
    }

    public void Update()
    {
        attackTimer -= Time.deltaTime;

        float dir = Mathf.Sign(enemy.transform.localScale.x);
        enemy.rigidBody.linearVelocity = new Vector2(dir * enemy.enemyStats.chargeAttackSpeed, enemy.rigidBody.linearVelocity.y);

        if (attackTimer <= 0)
        {
            if (enemy.targetPlayer)
                enemy.ChangeState(new ChaseState(enemy));
            else
                enemy.ChangeState(new PatrolState(enemy));
        }
    }

    public void Exit()
    {
        enemy.attackController.isAttacking = false;
        enemy.animator.SetBool("isAttacking", false);
    }
}
