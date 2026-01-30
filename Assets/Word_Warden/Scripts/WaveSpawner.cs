using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public GameObject survivorPrefab;

    public Transform spawnPoint;
    public float spawnRate = 3f;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            SpawnEntity();
            yield return new WaitForSeconds(spawnRate);

            // Ramping difficulty (decrease spawn rate slightly)
            if (spawnRate > 0.5f) spawnRate -= 0.01f;
        }
    }

    void SpawnEntity()
    {
        // 20% Chance for Survivor, 80% Zombie
        bool isSurvivor = Random.value > 0.8f;

        GameObject obj;
        if (isSurvivor)
        {
            obj = Instantiate(survivorPrefab, spawnPoint.position, Quaternion.identity);
            SurvivorController sc = obj.GetComponent<SurvivorController>();

            // Assign Words from WordBank
            sc.assignedWord = WordBank.Instance.GetWordByDifficulty(0);
            sc.secondWord = WordBank.Instance.GetWordByDifficulty(0); // Recruit word
        }
        else
        {
            obj = Instantiate(zombiePrefab, spawnPoint.position, Quaternion.identity);
            EnemyController ec = obj.GetComponent<EnemyController>();

            // Logic for difficulty (Basic vs Armored)
            ec.assignedWord = WordBank.Instance.GetWordByDifficulty(0);
        }
    }
}