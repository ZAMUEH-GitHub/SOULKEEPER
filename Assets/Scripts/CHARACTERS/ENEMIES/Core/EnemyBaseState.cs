using UnityEngine;

public abstract class EnemyBaseState : IEnemyState
{
    protected EnemyBaseController enemy;
    protected Transform player;

    protected EnemyBaseState(EnemyBaseController enemy)
    {
        this.enemy = enemy;
        this.player = enemy.player;
    }

    public virtual void Enter() { }

    public virtual void Update() { }

    public virtual void Exit() { }

    protected bool TryEnterDeathState()
    {
        if (enemy == null) return false;

        if (enemy.isDead)
        {
            if (enemy.CanMove)
            {
                enemy.Stop();
            }

            if (enemy.stateMachine.CurrentStateName != "DeathState")
            {
                enemy.stateMachine.ChangeState(new DeathState(enemy));
                Debug.Log($"[{enemy.gameObject.name}] Entered DeathState via helper");
            }

            return true;
        }

        return false;
    }
}