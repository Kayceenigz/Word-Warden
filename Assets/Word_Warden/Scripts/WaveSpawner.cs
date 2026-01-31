using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner Instance;

    public GameObject zombiePrefab;
    public GameObject survivorPrefab;
    public Transform spawnPoint;

    // We store the original speed so we don't multiply it infinitely
    private float baseZombieSpeed;

    private void Awake()
    {
        Instance = this;
        // Grab the speed from the prefab once at the start
        baseZombieSpeed = zombiePrefab.GetComponent<EnemyController>().moveSpeed;
    }

    public void SpawnWave(int waveNumber)
    {
        int totalToSpawn = 5 + (waveNumber * 2);
        GameManager.Instance.enemiesRemaining = totalToSpawn;

        StopAllCoroutines(); // Clean up any old waves
        StartCoroutine(WaveRoutine(totalToSpawn, waveNumber));
    }

    IEnumerator WaveRoutine(int count, int waveNumber)
    {
        for (int i = 0; i < count; i++)
        {
            // Stop spawning if game is over
            if (GameManager.Instance.currentState == GameManager.GameState.GameOver) yield break;

            SpawnEntity(waveNumber);

            float currentSpawnRate = Mathf.Max(0.5f, 2.0f - (waveNumber * 0.1f));
            yield return new WaitForSeconds(currentSpawnRate);
        }
    }

    void SpawnEntity(int waveNumber)
    {
        bool isSurvivor = Random.value > 0.8f;
        GameObject obj;

        if (isSurvivor)
        {
            obj = Instantiate(survivorPrefab, spawnPoint.position, Quaternion.identity);
            SurvivorController sc = obj.GetComponent<SurvivorController>();

            sc.assignedWord = WordBank.Instance.GetWordByDifficulty(0);
            sc.secondWord = WordBank.Instance.GetWordByDifficulty(0);
        }
        else
        {
            obj = Instantiate(zombiePrefab, spawnPoint.position, Quaternion.identity);
            EnemyController ec = obj.GetComponent<EnemyController>();

            int wordDifficulty = 0;
            if (waveNumber > 3) wordDifficulty = 1;
            if (waveNumber > 7) wordDifficulty = 2;

            ec.assignedWord = WordBank.Instance.GetWordByDifficulty(wordDifficulty);

            // FIX: Use the base speed multiplied by scale, don't multiply the current speed
            ec.moveSpeed = baseZombieSpeed * GameManager.Instance.difficultyScale;
        }
    }
}