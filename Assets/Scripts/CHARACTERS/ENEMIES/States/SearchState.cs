using UnityEngine;

public class SearchState : EnemyBaseState
{
    private Vector2 searchTarget;
    private float searchTimer = 0f;
    private float moveTimeout = 0f;
    private bool reachedSearchPoint;

    public SearchState(EnemyBaseController enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.ForceClearAllMovementLocks();

        reachedSearchPoint = false;
        searchTimer = 0f;
        moveTimeout = 0f;

        if (enemy.animController != null)
        {
            enemy.animController.SetMoving(true);
        }

        float searchRadius = enemy.enemyStats.searchRadius > 0 ? enemy.enemyStats.searchRadius : 4f;
        searchTarget = new Vector2(
            enemy.lastKnownPlayerPosition.x + Random.Range(-searchRadius, searchRadius),
            enemy.transform.position.y
        );
    }

    public override void Update()
    {
        if (TryEnterDeathState()) return;

        if (enemy.targetPlayer)
        {
            enemy.stateMachine.ChangeState(new AlertState(enemy));
            return;
        }

        if (!reachedSearchPoint)
        {
            enemy.MoveToTarget(searchTarget, enemy.enemyStats.speed);
            enemy.Flip(searchTarget);

            float distanceToTarget = Mathf.Abs(enemy.transform.position.x - searchTarget.x);
            moveTimeout += Time.deltaTime;

            if (distanceToTarget <= 0.5f || moveTimeout > 5.0f)
            {
                reachedSearchPoint = true;
                enemy.Stop();
                enemy.RequestMovementLock("SearchWait");
                searchTimer = 0f;
                if (enemy.animController != null) enemy.animController.SetMoving(false);
            }
            return;
        }

        searchTimer += Time.deltaTime;
        float searchDuration = enemy.enemyStats.searchDuration > 0 ? enemy.enemyStats.searchDuration : 4f;

        if (searchTimer >= searchDuration)
        {
            enemy.stateMachine.ChangeState(new PatrolState(enemy));
        }
    }

    public override void Exit()
    {
        enemy.ReleaseMovementLock("SearchWait");
        if (enemy.animController != null) enemy.animController.SetMoving(false);
    }
}