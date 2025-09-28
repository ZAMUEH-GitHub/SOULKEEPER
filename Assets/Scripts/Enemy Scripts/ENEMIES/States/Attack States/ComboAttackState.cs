using UnityEngine;

public class ComboAttackState : IEnemyState
{
    private EnemyBaseController enemy;

    public ComboAttackState(EnemyBaseController enemy) { this.enemy = enemy; }
    public void Enter() { }
    public void Update() { }
    public void Exit() { }
}
