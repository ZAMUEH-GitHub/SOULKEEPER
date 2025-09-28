using UnityEngine;

public class JumpState : IEnemyState
{
    private EnemyBaseController enemy;

    public JumpState(EnemyBaseController enemy) { this.enemy = enemy; }
    public void Enter() { }
    public void Update() { }
    public void Exit() { }
}
