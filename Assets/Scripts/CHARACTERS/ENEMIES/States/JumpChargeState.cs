using UnityEngine;

public class JumpChargeState : IEnemyState
{
    private EnemyBaseController enemy;
    private float chargeTimer;

    public JumpChargeState(EnemyBaseController enemy) { this.enemy = enemy; }

    public void Enter()
    {
        chargeTimer = enemy.enemyStats.jumpChargeDuration;
        enemy.Stop();
        enemy.animator.SetBool("isChargingJump", true);
    }

    public void Update()
    {
        chargeTimer -= Time.deltaTime;

        if (chargeTimer <= 0)
        {
            enemy.ChangeState(new JumpState(enemy));
        }
    }

    public void Exit()
    {
        enemy.animator.SetBool("isChargingJump", false);
    }
}
