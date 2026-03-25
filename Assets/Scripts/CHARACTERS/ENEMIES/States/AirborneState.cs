using UnityEngine;

public class AirborneState : EnemyBaseState
{
    private float chargeTimer;
    private bool isCharging;
    private bool hasJumped;

    public AirborneState(EnemyBaseController enemy) : base(enemy) { }

    public override void Enter()
    {
        isCharging = enemy.enemyStats.jumpChargeDuration > 0;
        hasJumped = false;

        if (isCharging)
        {
            chargeTimer = enemy.enemyStats.jumpChargeDuration;
            enemy.Stop();
            enemy.RequestMovementLock("JumpCharge");

            if (enemy.animator != null)
                enemy.animator.SetBool("isChargingJump", true);
        }
        else
        {
            ExecuteJump();
        }
    }

    public override void Update()
    {
        if (TryEnterDeathState()) return;

        if (isCharging)
        {
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0)
            {
                isCharging = false;
                ExecuteJump();
            }
            return;
        }

        if (hasJumped)
        {

            if (enemy.rigidBody.linearVelocity.y <= 0)
            {
                if (enemy.animator != null)
                    enemy.animator.SetBool("isFalling", true);
            }

            if (enemy.rigidBody.linearVelocity.y <= 0 && enemy.jumpController != null && enemy.jumpController.isGrounded)
            {
                Land();
            }
        }
    }

    private void ExecuteJump()
    {
        enemy.ReleaseMovementLock("JumpCharge");

        if (enemy.animator != null)
        {
            enemy.animator.SetBool("isChargingJump", false);
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

        if (enemy.animator != null)
        {
            enemy.animator.SetBool("isChargingJump", false);
            enemy.animator.SetBool("isFalling", false);
        }
    }
}