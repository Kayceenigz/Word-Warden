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
        if (zombiePrefab != null)
            baseZombieSpeed = zombiePrefab.GetComponent<EnemyController>().moveSpeed;
    }

    public void SpawnWave(int waveNumber)
    {
        // Tell the GameManager we are officially in spawning mode
        GameManager.Instance.isSpawning = true;

        int totalToSpawn = 5 + (waveNumber * 2);
        GameManager.Instance.enemiesRemaining = totalToSpawn;

        StopAllCoroutines();
        StartCoroutine(WaveRoutine(totalToSpawn, waveNumber));

        // REMOVED: GameManager.Instance.isSpawning = false; 
        // If we put it here, it executes instantly!
    }

    IEnumerator WaveRoutine(int count, int waveNumber)
    {
        for (int i = 0; i < count; i++)
        {
            if (GameManager.Instance.currentState == GameManager.GameState.GameOver) yield break;

            SpawnEntity(waveNumber);

            float currentSpawnRate = Mathf.Max(0.5f, 2.0f - (waveNumber * 0.1f));
            yield return new WaitForSeconds(currentSpawnRate);
        }

        // THIS is where the spawning officially ends
        GameManager.Instance.isSpawning = false;
        Debug.Log("Spawner: Wave " + waveNumber + " is fully dispatched.");
    }

    void SpawnEntity(int waveNumber)
    {
        // 20% chance for a survivor, but only if the stack isn't full
        bool isSurvivor = (Random.value > 0.8f) && (StackManager.Instance.stackUnits.Count < 3);

        GameObject obj;
        EntityBase entityScript;

        if (isSurvivor)
        {
            obj = Instantiate(survivorPrefab, spawnPoint.position, Quaternion.identity);
            SurvivorController sc = obj.GetComponent<SurvivorController>();

            // Set the Mask word and the Recruitment word
            sc.assignedWord = WordBank.Instance.GetWordByDifficulty(0);
            sc.recruitmentWord = WordBank.Instance.GetWordByDifficulty(0); // Ensure this variable name matches your SurvivorController

            entityScript = sc;
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

            entityScript = ec;
        }

        // CRITICAL: Register the new entity with the TypingManager
        // Without this, the zombies will walk past and ignore your typing!
        if (TypingManager.Instance != null && entityScript != null)
        {
            TypingManager.Instance.AddTarget(entityScript);
        }
    }
}