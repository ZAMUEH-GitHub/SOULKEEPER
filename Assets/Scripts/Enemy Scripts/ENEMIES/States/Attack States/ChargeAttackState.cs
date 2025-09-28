using UnityEngine;

public class ChargeAttackState : IEnemyState
{
    private EnemyBaseController enemy;

    public ChargeAttackState(EnemyBaseController enemy) { this.enemy = enemy; }
    public void Enter() { }
    public void Update() { }
    public void Exit() { }
}
