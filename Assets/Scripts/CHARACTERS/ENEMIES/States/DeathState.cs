using UnityEngine;

public class DeathState : IEnemyState
{
    private EnemyBaseController enemy;

    public DeathState(EnemyBaseController enemy) { this.enemy = enemy; }

    public void Enter()
    {
        enemy.Stop();
        enemy.animator.SetTrigger("EnemyDeath");
    }

    public void Update()
    {

    }

    public void Exit() { }
}
