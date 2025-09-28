using UnityEngine;

public class ChaseState : IEnemyState
{
    private EnemyBaseController enemy;

    public ChaseState(EnemyBaseController enemy) { this.enemy = enemy; }
    public void Enter() { }
    public void Update() { }
    public void Exit() { }
}
