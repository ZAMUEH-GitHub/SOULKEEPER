using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform patrolTarget;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnDistanceToPlayer = 10f;
    [SerializeField] private float despawnDistanceToPlayer = 14f;
    [SerializeField] private float respawnDelay = 3f;

    [Header("Debug Info")]
    [SerializeField] private float currentDistanceToPlayer;

    private GameObject spawnedEnemy;
    private Transform playerTransform;
    private bool canRespawn = true;
    private float respawnTimer;

    private void Start()
    {
        if (PlayerController.Instance != null)
            playerTransform = PlayerController.Instance.transform;
        else
            Debug.LogError("[EnemySpawner] PlayerController Instance not found!");
    }

    private void Update()
    {
        if (playerTransform == null) return;

        currentDistanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (spawnedEnemy != null && currentDistanceToPlayer > despawnDistanceToPlayer)
        {
            Destroy(spawnedEnemy);
            spawnedEnemy = null;
            canRespawn = false;
            respawnTimer = 0f;
        }

        if (!canRespawn)
        {
            respawnTimer += Time.deltaTime;
            if (respawnTimer >= respawnDelay)
                canRespawn = true;
        }

        if (spawnedEnemy == null && canRespawn && currentDistanceToPlayer <= spawnDistanceToPlayer)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("[EnemySpawner] No enemy prefab assigned.");
            return;
        }

        spawnedEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        var enemyController = spawnedEnemy.GetComponent<EnemyBaseController>();
        if (enemyController != null)
        {
            enemyController.patrolStart = transform.position;

            if (patrolTarget != null)
                enemyController.patrolTarget = patrolTarget;
            else
                Debug.LogWarning($"[EnemySpawner] No patrol target assigned for {name}");

            enemyController.currentTarget = patrolTarget != null
                ? patrolTarget.position
                : transform.position;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnDistanceToPlayer);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, despawnDistanceToPlayer);
    }
#endif
}
