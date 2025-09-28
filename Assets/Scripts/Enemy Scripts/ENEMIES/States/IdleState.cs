using UnityEngine;

public class IdleState : IEnemyState
{
    private EnemyBaseController enemy;

    public IdleState(EnemyBaseController enemy) { this.enemy = enemy; }
    public void Enter() { }
    public void Update() { }
    public void Exit() { }
}
