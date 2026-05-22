using UnityEngine;

public class AirborneState : EnemyBaseState
{
    private bool isCharging;
    private bool hasJumped;

    public AirborneState(EnemyBaseController enemy) : base(enemy) { }

    public override void Enter()
    {
        isCharging = enemy.enemyStats.hasJumpCharge;
        hasJumped = false;

        if (isCharging)
        {
            enemy.Stop();
            enemy.RequestMovementLock("JumpCharge");

            if (enemy.animController != null)
                enemy.animController.SetChargingJump(true);
        }
        else
        {
            ExecuteJump();
        }
    }

    public override void Update()
    {
        if (TryEnterDeathState()) return;

        if (isCharging) return;

        if (hasJumped)
        {
            if (enemy.rigidBody.linearVelocity.y <= 0)
            {
                if (enemy.animController != null)
                {
                    enemy.animController.SetJumping(false);
                    enemy.animController.SetFalling(true);
                }
            }

            if (enemy.rigidBody.linearVelocity.y <= 0 && enemy.jumpController != null && enemy.jumpController.isGrounded)
            {
                Land();
            }
        }
    }

    public void ExecuteJump()
    {
        isCharging = false;
        enemy.ReleaseMovementLock("JumpCharge");

        if (enemy.animController != null)
        {
            enemy.animController.SetChargingJump(false);
            enemy.animController.SetJumping(true);
        }

        if (enemy.targetPlayer && player != null)
        {
            float chaseSpeed = enemy.enemyStats.speed * (enemy.enemyStats.canRun ? enemy.enemyStats.speedMultiplier : 1f);
            enemy.MoveToTarget(new Vector2(player.position.x, enemy.transform.position.y), chaseSpeed);
            enemy.Flip(player.position);
        }
        else
        {
            enemy.MoveToTarget(enemy.currentTarget);
            enemy.Flip(enemy.currentTarget);
        }

        if (enemy.jumpController != null)
            enemy.jumpController.DoJump();

        hasJumped = true;
    }

    private void Land()
    {
        if (enemy.targetPlayer)
            enemy.stateMachine.ChangeState(new ChaseState(enemy));
        else
            enemy.stateMachine.ChangeState(new PatrolState(enemy));
    }

    public override void Exit()
    {
        enemy.ReleaseMovementLock("JumpCharge");

        if (enemy.animController != null)
        {
            enemy.animController.SetChargingJump(false);
            enemy.animController.SetFalling(false);
            enemy.animController.SetJumping(false);
        }
    }
}