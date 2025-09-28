using UnityEngine;

public class DeathState : IEnemyState
{
    private EnemyBaseController enemy;

    public DeathState(EnemyBaseController enemy) { this.enemy = enemy; }
    public void Enter() { }
    public void Update() { }
    public void Exit() { }
}
