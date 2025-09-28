using UnityEngine;

public class SimpleAttackState : IEnemyState
{
    private EnemyBaseController enemy;

    public SimpleAttackState(EnemyBaseController enemy) { this.enemy = enemy; }
    public void Enter() { }
    public void Update() { }
    public void Exit() { }
}
