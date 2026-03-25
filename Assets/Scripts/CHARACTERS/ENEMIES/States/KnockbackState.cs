using UnityEngine;

public class KnockbackState : EnemyBaseState
{
    private Vector2 knockbackVector;
    private float knockbackForce;
    private float duration;
    private float elapsed;

    public KnockbackState(EnemyBaseController enemy, Vector2 knockbackVector, float knockbackForce, float knockbackDuration) : base(enemy)
    {
        this.knockbackVector = knockbackVector;
        this.knockbackForce = knockbackForce;
        this.duration = knockbackDuration;
    }

    public override void Enter()
    {
        enemy.ForceClearAllMovementLocks();

        enemy.rigidBody.linearVelocity = new Vector2(knockbackVector.x * knockbackForce, knockbackVector.y * knockbackForce + knockbackForce);

        if (enemy.animController != null)
        {
            // enemy.animController.TriggerHit(); 
        }

        elapsed = 0f;
    }

    public override void Update()
    {
        if (TryEnterDeathState()) return;

        elapsed += Time.deltaTime;

        if (elapsed >= duration)
        {
            if (enemy.targetPlayer)
                enemy.stateMachine.ChangeState(new ChaseState(enemy));
            else
                enemy.stateMachine.ChangeState(new PatrolState(enemy));
        }
    }

    public override void Exit()
    {
        enemy.Stop();
    }
}