using UnityEngine;

public class ChargeAttackState : IMovementState
{
    private readonly EnemyBaseController enemy;
    private float chargeTimer;

    public ChargeAttackState(EnemyBaseController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.Stop();
        enemy.PauseVertical(true);

        enemy.ResetVerticalStateIfGrounded();

        enemy.attackController.isChargingAttack = true;
        enemy.animator.SetBool("isChargingAttack", true);

        chargeTimer = enemy.enemyStats.chargeAttackDuration;
    }

    public void Update()
    {
        chargeTimer -= Time.deltaTime;

        if (chargeTimer <= 0)
            FinishCharge();
    }

    public void Exit()
    {
        enemy.attackController.isChargingAttack = false;
        enemy.animator.SetBool("isChargingAttack", false);
    }

    public void FinishCharge()
    {
        enemy.ChangeMovementState(new AttackingState(enemy));
    }
}
