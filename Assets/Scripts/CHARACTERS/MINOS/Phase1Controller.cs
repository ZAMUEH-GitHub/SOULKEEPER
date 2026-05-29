using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Phase1Controller : MonoBehaviour
{
    [Header("Phase 1 Settings")]
    public float spawnRate;
    public float nextRoundTime;
    public GameObject[] enemySpawners;

    [Header("Drag and Drop Enemy Waves")]
    public List<GameObject> round1Enemies;
    public List<GameObject> round2Enemies;
    public List<GameObject> round3Enemies;
    public List<GameObject> round4Enemies;
    public List<GameObject> round5Enemies;

    public enum SpawnRound { Round1, Round2, Round3, Round4, Round5 };
    public SpawnRound spawnRound = SpawnRound.Round1;

    private int currentSpawnerIndex = 0;
    private bool hasSpawnedRound = false;
    private bool isSpawning = false;
    private bool isAdvancingRound = false;
    public int nextRoundInt = 1;

    private Animator bossAnimator;
    private MinosController minosController;
    private List<EnemyBaseController> activeEnemies = new List<EnemyBaseController>();

    void Start()
    {
        bossAnimator = GetComponentInChildren<Animator>();
        minosController = GetComponent<MinosController>();
    }

    public void RunPhase1()
    {
        if (!hasSpawnedRound && !isSpawning)
        {
            StartCoroutine(SpawnCurrentRound());
            hasSpawnedRound = true;
        }
    }

    private IEnumerator SpawnCurrentRound()
    {
        isSpawning = true;
        currentSpawnerIndex = Random.Range(0, enemySpawners.Length);

        List<GameObject> enemiesToSpawn = new List<GameObject>();
        switch (spawnRound)
        {
            case SpawnRound.Round1: enemiesToSpawn = round1Enemies; break;
            case SpawnRound.Round2: enemiesToSpawn = round2Enemies; break;
            case SpawnRound.Round3: enemiesToSpawn = round3Enemies; break;
            case SpawnRound.Round4: enemiesToSpawn = round4Enemies; break;
            case SpawnRound.Round5: enemiesToSpawn = round5Enemies; break;
        }

        foreach (GameObject enemyPrefab in enemiesToSpawn)
        {
            if (enemyPrefab == null) continue;

            Vector2 spawnPosition = enemySpawners[currentSpawnerIndex].transform.position;
            GameObject newEnemyObj = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            EnemyBaseController enemyController = newEnemyObj.GetComponent<EnemyBaseController>();
            if (enemyController != null)
            {
                activeEnemies.Add(enemyController);
                enemyController.OnEnemyDied += HandleEnemyDeath;
            }

            currentSpawnerIndex = (currentSpawnerIndex + 1) % enemySpawners.Length;
            yield return new WaitForSeconds(spawnRate);
        }

        Debug.Log(spawnRound.ToString() + " Spawned!!");
        isSpawning = false;
    }

    private void HandleEnemyDeath(EnemyBaseController enemy)
    {
        enemy.OnEnemyDied -= HandleEnemyDeath;

        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }

        if (activeEnemies.Count == 0 && !isSpawning && !isAdvancingRound)
        {
            StartCoroutine(AdvanceRound());
        }
    }

    private IEnumerator AdvanceRound()
    {
        isAdvancingRound = true;

        yield return new WaitForSeconds(nextRoundTime);

        hasSpawnedRound = false;
        isAdvancingRound = false;

        if (spawnRound < SpawnRound.Round5)
        {
            spawnRound++;
            bossAnimator.SetInteger("Spawn Round", (int)spawnRound + 1);
        }
        else
        {
            minosController.AdvancePhase();
        }
    }
}