using UnityEngine;

public class JumpState : IVerticalState
{
    private readonly EnemyBaseController enemy;

    public JumpState(EnemyBaseController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        if (enemy.jumpController != null)
            enemy.jumpController.DoJump();

        enemy.PauseMovement(false);

        enemy.animator.SetTrigger("EnemyJump");
    }

    public void Update()
    {
        if (enemy.rigidBody.linearVelocity.y <= 0)
        if (enemy.rigidBody.linearVelocity.y <= 0)
        {
            enemy.ChangeVerticalState(new FallingState(enemy));
        }
    }

    public void Exit()
    {
    }
}
