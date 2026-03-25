using UnityEngine;

[RequireComponent(typeof(EnemyBaseController))]
[DisallowMultipleComponent]
public class EnemyPatrolController : MonoBehaviour
{
    private EnemyBaseController enemy;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public bool randomPatrol;

    private int currentIndex;
    private Vector2 originPosition;

#if UNITY_EDITOR
    [Header("Debug Settings")]
    public bool ShowPatrolDebug = true;
#endif

    private void Awake()
    {
        enemy = GetComponent<EnemyBaseController>();
    }

    private void Start()
    {
        originPosition = transform.position;
    }

    public Vector2 GetNextPatrolPoint()
    {
        if (randomPatrol)
        {
            return GetRandom2DPoint(originPosition, enemy.enemyStats.patrolRadius);
        }
        else if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Vector2 nextPoint = patrolPoints[currentIndex % patrolPoints.Length].position;
            currentIndex++;
            return nextPoint;
        }

        return transform.position;
    }

    private Vector2 GetRandom2DPoint(Vector2 center, float radius)
    {
        float randomX = center.x + Random.Range(-radius, radius);


        return new Vector2(randomX, center.y);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!ShowPatrolDebug || enemy == null || enemy.enemyStats == null || !randomPatrol) return;

        Gizmos.color = Color.cyan;
        Vector2 center = Application.isPlaying ? originPosition : (Vector2)transform.position;
        Gizmos.DrawLine(center - Vector2.right * enemy.enemyStats.patrolRadius, center + Vector2.right * enemy.enemyStats.patrolRadius);
    }
#endif
}