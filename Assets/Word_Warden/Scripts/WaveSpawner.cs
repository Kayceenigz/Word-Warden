using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner Instance;

    [Header("Prefabs")]
    public GameObject[] zombieVisualPrefabs; // Array for the 3 cosmetic types
    public GameObject[] maskPrefabs;         // Changed to Array for your 3 separate Mask prefabs
    public Transform spawnPoint;

    private float baseZombieSpeed;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        // Grab the baseline speed from the first zombie variant
        if (zombieVisualPrefabs.Length > 0)
        {
            EnemyController ec = zombieVisualPrefabs[0].GetComponent<EnemyController>();
            if (ec != null) baseZombieSpeed = ec.moveSpeed;
        }
    }

    public void SpawnWave(int waveNumber)
    {
        GameManager.Instance.isSpawning = true;

        int totalToSpawn = 5 + (waveNumber * 2);
        GameManager.Instance.enemiesRemaining = totalToSpawn;

        StopAllCoroutines();
        StartCoroutine(WaveRoutine(totalToSpawn, waveNumber));
    }

    IEnumerator WaveRoutine(int count, int waveNumber)
    {
        for (int i = 0; i < count; i++)
        {
            if (GameManager.Instance.currentState == GameManager.GameState.GameOver)
            {
                GameManager.Instance.isSpawning = false;
                yield break;
            }

            SpawnEntity(waveNumber);

            float currentSpawnRate = Mathf.Max(0.5f, 2.0f - (waveNumber * 0.1f));
            yield return new WaitForSeconds(currentSpawnRate);
        }

        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.isSpawning = false;
        GameManager.Instance.EnemyDefeated();
    }

    void SpawnEntity(int waveNumber)
    {
        EntityBase entityScript = null;
        GameObject obj = null;

        // Determine if we are spawning a mask (e.g., 5% chance)
        bool spawnMask = (Random.value > 0.95f) && !AllMasksCollected();

        if (spawnMask && maskPrefabs.Length > 0)
        {
            // Pick a random Mask prefab from your array (Health, Speed, or Difficulty)
            int randomMaskIndex = Random.Range(0, maskPrefabs.Length);
            obj = Instantiate(maskPrefabs[randomMaskIndex], spawnPoint.position, Quaternion.identity);

            MaskPickup mp = obj.GetComponent<MaskPickup>();

            // Set the first "Catch" word. The second "Hard" word is handled inside MaskPickup's script.
            mp.assignedWord = WordBank.Instance.GetWordByDifficulty(0);

            entityScript = mp;
        }
        else
        {
            // Spawn a random zombie variant
            int randomIndex = Random.Range(0, zombieVisualPrefabs.Length);
            obj = Instantiate(zombieVisualPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);

            EnemyController ec = obj.GetComponent<EnemyController>();

            // Difficulty scaling for words
            int wordDifficulty = (waveNumber > 7) ? 2 : (waveNumber > 3) ? 1 : 0;
            ec.assignedWord = WordBank.Instance.GetWordByDifficulty(wordDifficulty);

            // Apply the 10% speed reduction if the player has the Speed Mask
            float speedMultiplier = GameManager.Instance.hasSpeedMask ? 0.9f : 1.0f;
            ec.moveSpeed = baseZombieSpeed * GameManager.Instance.difficultyScale * speedMultiplier;

            entityScript = ec;
        }

        // Register the target so TypingManager can track it
        if (TypingManager.Instance != null && entityScript != null)
        {
            TypingManager.Instance.AddTarget(entityScript);
        }
    }

    private bool AllMasksCollected()
    {
        if (GameManager.Instance == null) return false;

        return GameManager.Instance.hasDifficultyMask &&
               GameManager.Instance.hasSpeedMask &&
               GameManager.Instance.hasHealthMask;
    }
}