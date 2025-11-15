using UnityEngine;

public class IdleState : IMovementState
{
    private readonly EnemyBaseController enemy;
    private float idleTimer;

    public IdleState(EnemyBaseController enemy) { this.enemy = enemy; }

    public void Enter()
    {
        enemy.Stop();
        idleTimer = enemy.enemyStats.idleDuration;
        enemy.animator.SetBool("isMoving", false);
    }

    public void Update()
    {
        idleTimer -= Time.deltaTime;

        if (enemy.targetPlayer)
        {
            enemy.ChangeMovementState(new ChaseState(enemy));
            return;
        }

        if (idleTimer <= 0)
        {
            enemy.ChangeMovementState(new PatrolState(enemy));
        }

        if (enemy.enemyStats.canJump && enemy.jumpController.CanJump &&
            enemy.currentTarget.y > enemy.transform.position.y + 2f)
        {
            enemy.ChangeVerticalState(new JumpChargeState(enemy));
        }
    }

    public void Exit() { }
}
