using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner Instance;

    public GameObject zombiePrefab;
    public GameObject survivorPrefab;
    public Transform spawnPoint;

    private float baseZombieSpeed;

    private void Awake()
    {
        Instance = this;
        // Grab the speed from the prefab once at the start
        if (zombiePrefab != null)
            baseZombieSpeed = zombiePrefab.GetComponent<EnemyController>().moveSpeed;
    }

    public void SpawnWave(int waveNumber)
    {
        // 1. Calculate count
        int totalToSpawn = 5 + (waveNumber * 2);

        // 2. Sync with GameManager
        GameManager.Instance.enemiesRemaining = totalToSpawn;

        StopAllCoroutines();
        StartCoroutine(WaveRoutine(totalToSpawn, waveNumber));
    }

    IEnumerator WaveRoutine(int count, int waveNumber)
    {
        // FIX: Use 'i < count' to spawn exactly the right amount
        for (int i = 0; i < count; i++)
        {
            if (GameManager.Instance.currentState == GameManager.GameState.GameOver) yield break;

            SpawnEntity(waveNumber);

            float currentSpawnRate = Mathf.Max(0.5f, 2.0f - (waveNumber * 0.1f));
            yield return new WaitForSeconds(currentSpawnRate);
        }

        Debug.Log("Spawner: All entities for wave " + waveNumber + " have been sent out.");
    }

    void SpawnEntity(int waveNumber)
    {
        bool isSurvivor = Random.value > 0.8f;
        GameObject obj;

        if (isSurvivor)
        {
            if(StackManager.Instance.stackUnits.Count<3)
            {
                obj = Instantiate(survivorPrefab, spawnPoint.position, Quaternion.identity);
                SurvivorController sc = obj.GetComponent<SurvivorController>();

                sc.assignedWord = WordBank.Instance.GetWordByDifficulty(0);
                sc.secondWord = WordBank.Instance.GetWordByDifficulty(0);
            }
        }
        else
        {
            obj = Instantiate(zombiePrefab, spawnPoint.position, Quaternion.identity);
            EnemyController ec = obj.GetComponent<EnemyController>();

            int wordDifficulty = 0;
            if (waveNumber > 3) wordDifficulty = 1;
            if (waveNumber > 7) wordDifficulty = 2;

            ec.assignedWord = WordBank.Instance.GetWordByDifficulty(wordDifficulty);
            ec.moveSpeed = baseZombieSpeed * GameManager.Instance.difficultyScale;
        }
    }
}