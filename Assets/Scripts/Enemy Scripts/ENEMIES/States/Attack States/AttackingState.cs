using UnityEngine;

public class AttackingState : IEnemyState
{
    private EnemyBaseController enemy;

    public AttackingState(EnemyBaseController enemy) { this.enemy = enemy; }
    public void Enter() { }
    public void Update() { }
    public void Exit() { }
}
