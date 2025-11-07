using UnityEngine;

public class JumpChargeState : IVerticalState
{
    private readonly EnemyBaseController enemy;
    private float chargeTimer;

    public JumpChargeState(EnemyBaseController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        chargeTimer = enemy.enemyStats.jumpChargeDuration;

        enemy.Stop();
        enemy.PauseMovement(true);

        enemy.animator.SetBool("isChargingJump", true);
    }

    public void Update()
    {
        chargeTimer -= Time.deltaTime;

        if (chargeTimer <= 0)
        {
            enemy.ChangeVerticalState(new JumpState(enemy));
        }
    }

    public void Exit()
    {
        enemy.animator.SetBool("isChargingJump", false);
    }
}
