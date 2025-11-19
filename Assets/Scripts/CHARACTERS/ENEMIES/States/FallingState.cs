using UnityEngine;

public class FallingState : IVerticalState
{
    private readonly EnemyBaseController enemy;

    public FallingState(EnemyBaseController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.animator.SetBool("isFalling", true);
    }

    public void Update()
    {
        if (enemy.jumpController.isGrounded)
        {
            enemy.animator.SetBool("isFalling", false);

            enemy.ChangeVerticalState(null);
        }
    }

    public void Exit()
    {
        enemy.animator.SetBool("isFalling", false);
    }
}
