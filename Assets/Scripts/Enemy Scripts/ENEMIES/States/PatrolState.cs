using UnityEngine;

public class PatrolState : IEnemyState
{
    private EnemyBaseController enemy;

    public PatrolState(EnemyBaseController enemy) { this.enemy = enemy; }
    public void Enter() { }
    public void Update() { }
    public void Exit() { }
}
