using UnityEngine;

public class AttackingState : IMovementState
{
    private readonly EnemyBaseController enemy;
    private float attackTimer;

    public AttackingState(EnemyBaseController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.PauseVertical(true);

        enemy.ResetVerticalStateIfGrounded();

        enemy.attackController.isAttacking = true;
        enemy.animator.SetBool("isAttacking", true);

        attackTimer = enemy.enemyStats.chargeAttackDuration;

        float dir = Mathf.Sign(enemy.transform.localScale.x);
        enemy.rigidBody.linearVelocity = new Vector2(
            dir * enemy.enemyStats.chargeAttackSpeed,
            enemy.rigidBody.linearVelocity.y
        );
    }

    public void Update()
    {
        attackTimer -= Time.deltaTime;

        float dir = Mathf.Sign(enemy.transform.localScale.x);
        enemy.rigidBody.linearVelocity = new Vector2(
            dir * enemy.enemyStats.chargeAttackSpeed,
            enemy.rigidBody.linearVelocity.y
        );

        if (attackTimer <= 0)
            EndAttack();
    }

    public void Exit()
    {
        enemy.attackController.isAttacking = false;
        enemy.animator.SetBool("isAttacking", false);

        enemy.PauseVertical(false);
    }

    private void EndAttack()
    {
        if (enemy.targetPlayer)
            enemy.ChangeMovementState(new ChaseState(enemy));
        else
            enemy.ChangeMovementState(new PatrolState(enemy));
    }
}
