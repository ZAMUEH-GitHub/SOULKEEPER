using UnityEngine;

public class DeathState : IMovementState
{
    private readonly EnemyBaseController enemy;
    private const string CORPSE_LAYER = "Corpse";

    public DeathState(EnemyBaseController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.Stop();
        enemy.PauseMovement(true);
        enemy.PauseVertical(true);

        if (enemy.rigidBody != null)
        {
            enemy.rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (LayerMask.NameToLayer(CORPSE_LAYER) != -1)
            enemy.gameObject.layer = LayerMask.NameToLayer(CORPSE_LAYER);

        enemy.animator.SetTrigger("EnemyDeath");

        if (enemy.verticalStateMachine != null)
            enemy.verticalStateMachine.ChangeState(null);
    }

    public void Update() { }

    public void Exit() { }
}
