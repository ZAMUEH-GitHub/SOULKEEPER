public class JumpState : IEnemyState
{
    private EnemyBaseController enemy;

    public JumpState(EnemyBaseController enemy) { this.enemy = enemy; }

    public void Enter()
    {
        if (enemy.jumpController != null)
            enemy.jumpController.DoJump();
    }

    public void Update()
    {
        if (enemy.rigidBody.linearVelocity.y <= 0)
        {
            enemy.ChangeState(new FallingState(enemy));
        }
    }

    public void Exit() { }
}
