using UnityEngine;

public class ChargeAttackState : IEnemyState
{
    private EnemyBaseController enemy;

    public ChargeAttackState(EnemyBaseController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.Stop();
        enemy.attackController.isChargingAttack = true;

        enemy.animator.SetBool("isChargingAttack", true);
    }

    public void Update()
    {
    }

    public void Exit()
    {
        enemy.attackController.isChargingAttack = false;
        enemy.animator.SetBool("isChargingAttack", false);
    }

    public void FinishCharge()
    {
        enemy.ChangeState(new AttackingState(enemy));
    }
}
