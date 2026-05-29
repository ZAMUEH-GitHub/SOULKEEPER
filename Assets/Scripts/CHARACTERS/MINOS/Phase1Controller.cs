using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase1Controller : MonoBehaviour
{
    public enum BossRound
    {
        Round1 = 0,
        Round2 = 1,
        Round3 = 2,
        Round4 = 3,
        Round5 = 4,
        Complete = 5
    }

    [System.Serializable]
    public struct WaveConfiguration
    {
        [Tooltip("The exact sequence of enemies to spawn during this wave.")]
        public GameObject[] enemiesToSpawn;

        [Tooltip("Time in seconds between each enemy spawn.")]
        public float spawnRate;

        [Tooltip("Locations where enemies can randomly spawn.")]
        public Transform[] spawnPoints;
    }

    [Header("State References")]
    public BossStateManager stateManager;
    public Animator bossAnimator;

    [Header("Wave Settings")]
    public BossRound currentRound = BossRound.Round1;
    public List<WaveConfiguration> waves = new List<WaveConfiguration>();

    [Tooltip("How long the boss waits after a wave clears before starting the next one.")]
    public float delayBetweenRounds = 2.0f;

    private int activeEnemies = 0;
    private int enemiesSpawnedThisRound = 0;
    private bool isSpawning = false;

    public void StartWaves()
    {
        currentRound = BossRound.Round1;
        AdvanceRound();
    }

    public void AdvanceRound()
    {
        if (currentRound == BossRound.Complete)
        {
            stateManager.TransitionToPhase2();
            return;
        }

        if (bossAnimator != null)
        {
            bossAnimator.SetTrigger("Start" + currentRound.ToString());
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        int roundIndex = (int)currentRound;

        if (roundIndex >= waves.Count)
        {
            Debug.LogWarning("[BossWaveSpawner] Missing wave configuration for " + currentRound);
            yield break;
        }

        WaveConfiguration currentWave = waves[roundIndex];
        enemiesSpawnedThisRound = 0;
        isSpawning = true;

        foreach (GameObject enemyPrefab in currentWave.enemiesToSpawn)
        {
            SpawnEnemy(enemyPrefab, currentWave.spawnPoints);
            enemiesSpawnedThisRound++;
            yield return new WaitForSeconds(currentWave.spawnRate);
        }

        isSpawning = false;
    }

    private void SpawnEnemy(GameObject prefab, Transform[] spawnPoints)
    {
        if (prefab == null || spawnPoints.Length == 0) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject enemyObject = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        EnemyBaseController enemyController = enemyObject.GetComponent<EnemyBaseController>();

        if (enemyController != null)
        {
            activeEnemies++;
            enemyController.OnEnemyDied += HandleEnemyDeath;
        }
    }

    private void HandleEnemyDeath(EnemyBaseController enemy)
    {
        enemy.OnEnemyDied -= HandleEnemyDeath;
        activeEnemies--;

        CheckRoundCompletion();
    }

    private void CheckRoundCompletion()
    {
        if (!isSpawning && activeEnemies <= 0)
        {
            currentRound++;
            StartCoroutine(PrepareNextRound());
        }
    }

    private IEnumerator PrepareNextRound()
    {
        yield return new WaitForSeconds(delayBetweenRounds);
        AdvanceRound();
    }
}