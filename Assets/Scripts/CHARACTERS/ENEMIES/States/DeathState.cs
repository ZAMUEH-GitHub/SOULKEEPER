using UnityEngine;

public class DeathState : EnemyBaseState
{
    private const string CORPSE_LAYER = "Corpse";

    public DeathState(EnemyBaseController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.ForceClearAllMovementLocks();
        enemy.Stop();

        if (LayerMask.NameToLayer(CORPSE_LAYER) != -1)
            enemy.gameObject.layer = LayerMask.NameToLayer(CORPSE_LAYER);

        if (enemy.animController != null)
            enemy.animController.TriggerDeath();

        if (enemy.isAlive)
            enemy.Die();
    }

    public override void Update() { }

    public override void Exit() { }
}