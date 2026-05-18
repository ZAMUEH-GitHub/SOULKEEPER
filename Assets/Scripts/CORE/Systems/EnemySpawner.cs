using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public enum SpawnerMode { SingleSpawn, SpawnerPoint }

    [Header("Spawner Mode")]
    [SerializeField] private SpawnerMode mode = SpawnerMode.SingleSpawn;

    [Header("Single Spawn Settings")]
    [SerializeField] private GameObject singleEnemyPrefab;

    [Header("Spawner Point Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Vector2 spawnRateRange = new Vector2(3f, 7f);
    [SerializeField] private int maxActiveEnemies = 5;

    [Header("General Settings")]
    [SerializeField] private float spawnDistanceToPlayer = 10f;
    [SerializeField] private float despawnDistanceToPlayer = 14f;
    [SerializeField] private float respawnDelay = 3f;

    [Header("Debug Info")]
    [SerializeField] private float currentDistanceToPlayer;
    [SerializeField] private int currentActiveEnemies;

    private GameObject spawnedSingleEnemy;
    private List<GameObject> spawnedPointEnemies = new List<GameObject>();
    private Transform playerTransform;

    private bool canRespawnSingle = true;
    private bool singleEnemyDiedPermanently = false;
    private float respawnTimer;
    private float nextSpawnTime;

    private void Start()
    {
        if (PlayerController.Instance != null)
            playerTransform = PlayerController.Instance.transform;
        else
            Debug.LogError("[EnemySpawner] PlayerController Instance not found!");

        SetNextSpawnTime();
    }

    private void Update()
    {
        if (playerTransform == null) return;

        currentDistanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (mode == SpawnerMode.SingleSpawn)
        {
            HandleSingleSpawnUpdate();
        }
        else if (mode == SpawnerMode.SpawnerPoint)
        {
            HandleSpawnerPointUpdate();
        }
    }

    private void HandleSingleSpawnUpdate()
    {
        if (singleEnemyDiedPermanently) return;

        if (spawnedSingleEnemy != null && currentDistanceToPlayer > despawnDistanceToPlayer)
        {
            Destroy(spawnedSingleEnemy);
            spawnedSingleEnemy = null;
            canRespawnSingle = false;
            respawnTimer = 0f;
        }

        if (!canRespawnSingle)
        {
            respawnTimer += Time.deltaTime;
            if (respawnTimer >= respawnDelay)
                canRespawnSingle = true;
        }

        if (spawnedSingleEnemy == null && canRespawnSingle && currentDistanceToPlayer <= spawnDistanceToPlayer)
        {
            SpawnSingleEnemy();
        }
    }

    private void HandleSpawnerPointUpdate()
    {
        spawnedPointEnemies.RemoveAll(item => item == null);
        currentActiveEnemies = spawnedPointEnemies.Count;

        for (int i = spawnedPointEnemies.Count - 1; i >= 0; i--)
        {
            if (spawnedPointEnemies[i] != null && Vector2.Distance(spawnedPointEnemies[i].transform.position, playerTransform.position) > despawnDistanceToPlayer)
            {
                Destroy(spawnedPointEnemies[i]);
                spawnedPointEnemies.RemoveAt(i);
            }
        }

        if (currentDistanceToPlayer <= spawnDistanceToPlayer && spawnedPointEnemies.Count < maxActiveEnemies)
        {
            nextSpawnTime -= Time.deltaTime;
            if (nextSpawnTime <= 0f)
            {
                SpawnRandomEnemy();
                SetNextSpawnTime();
            }
        }
    }

    private void SpawnSingleEnemy()
    {
        if (singleEnemyPrefab == null)
        {
            Debug.LogWarning("[EnemySpawner] No single enemy prefab assigned.");
            return;
        }

        spawnedSingleEnemy = Instantiate(singleEnemyPrefab, transform.position, Quaternion.identity);
        SetupEnemy(spawnedSingleEnemy, true);
    }

    private void SpawnRandomEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("[EnemySpawner] No enemy prefabs assigned to array.");
            return;
        }

        GameObject prefabToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        GameObject newEnemy = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);

        spawnedPointEnemies.Add(newEnemy);
        SetupEnemy(newEnemy, false);
    }

    private void SetupEnemy(GameObject enemyObj, bool isSingleSpawn)
    {
        var enemyController = enemyObj.GetComponent<EnemyBaseController>();

        if (enemyController != null)
        {
            if (isSingleSpawn)
                enemyController.OnEnemyDied += HandleSingleEnemyDeath;
            else
                enemyController.OnEnemyDied += HandlePointEnemyDeath;
        }
    }

    private void HandleSingleEnemyDeath(EnemyBaseController enemy)
    {
        enemy.OnEnemyDied -= HandleSingleEnemyDeath;
        if (spawnedSingleEnemy != null)
        {
            Destroy(spawnedSingleEnemy);
            spawnedSingleEnemy = null;
        }

        singleEnemyDiedPermanently = true;
    }

    private void HandlePointEnemyDeath(EnemyBaseController enemy)
    {
        enemy.OnEnemyDied -= HandlePointEnemyDeath;
        if (spawnedPointEnemies.Contains(enemy.gameObject))
        {
            spawnedPointEnemies.Remove(enemy.gameObject);
        }
    }

    private void SetNextSpawnTime()
    {
        nextSpawnTime = Random.Range(spawnRateRange.x, spawnRateRange.y);
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