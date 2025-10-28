using UnityEngine;

public class DeathState : IMovementState
{
    private readonly EnemyBaseController enemy;

    public DeathState(EnemyBaseController enemy) { this.enemy = enemy; }

    public void Enter()
    {
        enemy.Stop();
        enemy.rigidBody.simulated = false;

        if (enemy.verticalStateMachine != null)
        {
            enemy.verticalStateMachine.ChangeState(null);
            enemy.verticalStateMachine = null;
        }

        enemy.animator.SetTrigger("EnemyDeath");
    }

    public void Update() { }

    public void Exit() { }
}
