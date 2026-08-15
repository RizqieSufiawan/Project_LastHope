using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject baseEnemyPrefab;
    public GameObject secondEnemyPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Base Timing")]
    public float spawnInterval = 10f;
    public int baseSpawnCount = 1;
    public float delayBetweenIndividualSpawns = 0.3f;

    [Header("Count Scaling")]
    public float countIncreaseInterval = 30f;
    public int countIncreaseAmount = 1;

    [Header("Second Enemy Unlock")]
    public float secondEnemyUnlockTime = 90f;

    [Header("Buff Scaling")]
    public float buffStartTime = 270f;
    public float buffInterval = 30f;
    public float healthMultiplierPerStep = 1.2f;
    public float damageMultiplierPerStep = 1.2f;

    private float elapsedTime;
    private float spawnTimer;
    private int roundRobinIndex; 

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            StartCoroutine(SpawnBatch());
        }
    }

    private IEnumerator SpawnBatch()
    {
        int count = GetCurrentSpawnCount();
        bool secondEnemyUnlocked = elapsedTime >= secondEnemyUnlockTime;
        int buffSteps = GetBuffSteps();

        Debug.Log($"Spawning {count} enemies at t={elapsedTime:F0}s, buffSteps={buffSteps}");

        for (int i = 0; i < count; i++)
        {
            GameObject prefabToSpawn = baseEnemyPrefab;

            if (secondEnemyUnlocked && secondEnemyPrefab != null && Random.value > 0.5f)
            {
                prefabToSpawn = secondEnemyPrefab;
            }

            SpawnEnemy(prefabToSpawn, buffSteps);

            yield return new WaitForSeconds(delayBetweenIndividualSpawns);
        }
    }

    private int GetCurrentSpawnCount()
    {
        int increases = Mathf.FloorToInt(elapsedTime / countIncreaseInterval);
        return baseSpawnCount + (increases * countIncreaseAmount);
    }

    private int GetBuffSteps()
    {
        if (elapsedTime < buffStartTime) return 0;
        return Mathf.FloorToInt((elapsedTime - buffStartTime) / buffInterval) + 1;
    }

    private void SpawnEnemy(GameObject prefab, int buffSteps)
    {
        if (spawnPoints.Length == 0 || prefab == null) return;

        Transform point = spawnPoints[roundRobinIndex];
        roundRobinIndex = (roundRobinIndex + 1) % spawnPoints.Length;

        GameObject enemy = Instantiate(prefab, point.position, Quaternion.identity);

        if (buffSteps > 0)
        {
            ApplyBuff(enemy, buffSteps);
        }
    }

    private void ApplyBuff(GameObject enemy, int buffSteps)
    {
        var health = enemy.GetComponent<Health>();
        if (health != null)
        {
            int buffedMax = Mathf.RoundToInt(health.maxHealth * Mathf.Pow(healthMultiplierPerStep, buffSteps));
            health.SetMaxHealth(buffedMax);
        }

        var ai = enemy.GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.attackDamage = Mathf.RoundToInt(ai.attackDamage * Mathf.Pow(damageMultiplierPerStep, buffSteps));
        }
    }
}