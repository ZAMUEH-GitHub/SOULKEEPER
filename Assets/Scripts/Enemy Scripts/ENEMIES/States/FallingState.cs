using UnityEngine;

public class FallingState : IEnemyState
{
    private EnemyBaseController enemy;

    public FallingState(EnemyBaseController enemy) { this.enemy = enemy; }

    public void Enter()
    {
        enemy.animator.SetBool("isFalling", true);
    }

    public void Update()
    {
        if (enemy.jumpController.isGrounded)
        {
            enemy.animator.SetBool("isFalling", false);

            if (enemy.targetPlayer)
                enemy.ChangeState(new ChaseState(enemy));
            else
                enemy.ChangeState(new PatrolState(enemy));
        }
    }

    public void Exit()
    {
        enemy.animator.SetBool("isFalling", false);
    }
}
