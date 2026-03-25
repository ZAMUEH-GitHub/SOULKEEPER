using UnityEngine;
using static EnemyStatsSO;

public class AlertState : EnemyBaseState
{
    private float alertTimer;
    private const float ALERT_TIMEOUT = 1.0f;

    public AlertState(EnemyBaseController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.Stop();
        enemy.RequestMovementLock("Alert");
        alertTimer = 0f;

        if (enemy.animator != null)
        {
            enemy.animator.SetTrigger("Alert");
        }

        if (player != null)
        {
            enemy.Flip(player.position);
            enemy.lastKnownPlayerPosition = player.position;
        }
    }

    public override void Update()
    {
        if (TryEnterDeathState()) return;

        alertTimer += Time.deltaTime;

        if (alertTimer >= ALERT_TIMEOUT)
        {
            switch (enemy.enemyStats.behaviorType)
            {
                case EnemyBehaviorType.Aggressive:
                    enemy.stateMachine.ChangeState(new ChaseState(enemy));
                    break;
                case EnemyBehaviorType.Fearful:
                    enemy.stateMachine.ChangeState(new FleeState(enemy));
                    break;
                case EnemyBehaviorType.Defensive:
                    enemy.stateMachine.ChangeState(new IdleState(enemy));
                    break;
                default:
                    enemy.stateMachine.ChangeState(new IdleState(enemy));
                    break;
            }
        }
    }

    public override void Exit()
    {
        enemy.ReleaseMovementLock("Alert");
    }
}