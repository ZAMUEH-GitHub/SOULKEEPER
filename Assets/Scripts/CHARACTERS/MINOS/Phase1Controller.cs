using UnityEngine;
using System.Collections;

public class Phase1Controller : MonoBehaviour
{
    [Header("Phase 1 Settings")]
    public float spawnRate;
    public float nextRoundTime;

    public GameObject[] enemyPrefabs;
    public GameObject[] enemySpawners;
    public enum SpawnRound { Round1, Round2, Round3, Round4, Round5 };
    public SpawnRound spawnRound = SpawnRound.Round1;

    private int currentSpawnerIndex = 0;
    private bool hasSpawnedRound = false;
    private bool isSpawning = false;
    private bool isAdvancingRound = false;
    private GameObject[] currentEnemies;
    public int nextRoundInt = 1;

    private Animator bossAnimator;
    private MinosController minosController;

    void Start()
    {
        bossAnimator = GetComponent<Animator>();
        minosController = GetComponent<MinosController>();
    }

    public void RunPhase1()
    {
        if (!hasSpawnedRound && !isSpawning)
        {
            StartCoroutine(SpawnCurrentRound());
            hasSpawnedRound = true;
        }

        if (!isSpawning && !isAdvancingRound)
        {
            currentEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (currentEnemies.Length == 0)
            {
                StartCoroutine(AdvanceRound());
            }
        }
    }

    private IEnumerator SpawnCurrentRound()
    {
        isSpawning = true;
        currentSpawnerIndex = Random.Range(0, enemySpawners.Length);

        switch (spawnRound)
        {
            case SpawnRound.Round1:
                yield return StartCoroutine(SpawnEnemies(0, 3));
                Debug.Log("Round 1!!");
                break;
            case SpawnRound.Round2:
                yield return StartCoroutine(SpawnEnemies(0, 3));
                yield return StartCoroutine(SpawnEnemies(1, 1));
                Debug.Log("Round 2!!");
                break;
            case SpawnRound.Round3:
                yield return StartCoroutine(SpawnEnemies(0, 5));
                yield return StartCoroutine(SpawnEnemies(1, 2));
                Debug.Log("Round 3!!");
                break;
            case SpawnRound.Round4:
                yield return StartCoroutine(SpawnEnemies(0, 5));
                yield return StartCoroutine(SpawnEnemies(1, 3));
                yield return StartCoroutine(SpawnEnemies(2, 1));
                Debug.Log("Round 4!!");
                break;
            case SpawnRound.Round5:
                yield return StartCoroutine(SpawnEnemies(0, 5));
                yield return StartCoroutine(SpawnEnemies(1, 3));
                yield return StartCoroutine(SpawnEnemies(2, 3));
                Debug.Log("Round 5!!");
                break;
        }

        isSpawning = false;
    }

    private IEnumerator SpawnEnemies(int enemyType, int enemyAmount)
    {
        for (int i = 0; i < enemyAmount; i++)
        {
            Vector2 spawnPosition = enemySpawners[currentSpawnerIndex].transform.position;
            Instantiate(enemyPrefabs[enemyType], spawnPosition, Quaternion.identity);
            currentSpawnerIndex = (currentSpawnerIndex + 1) % enemySpawners.Length;
            yield return new WaitForSeconds(spawnRate);
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

