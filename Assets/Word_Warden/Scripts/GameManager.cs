using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState { Menu, Playing, Shop, Paused, GameOver }
    public GameState currentState;

    [Header("Economy")]
    public int currentCoins = 0;

    [Header("Wave Settings")]
    public int currentWave = 0;
    public int enemiesRemaining;
    public float difficultyScale = 1.0f;
    public float maxDifficulty = 3.0f;

    [Header("Global Upgrades (Level 0+)")]
    public int speedUpgradeLevel = 0;
    public int damageUpgradeLevel = 0;

    // This must be set to true by WaveSpawner.cs when it starts spawning
    [HideInInspector] public bool isSpawning = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Time.timeScale = 1; // Ensure time is running on start
    }

    void Start()
    {
        // Initial UI Update
        if (HUDController.Instance != null)
        {
            HUDController.Instance.UpdateEconomyUI(currentCoins);
            HUDController.Instance.UpdateWaveUI(currentWave);
        }

        // Give the player 2 seconds to get ready before Wave 1
        Invoke("StartNextWave", 2f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }

    public void StartNextWave()
    {
        // Close Shop if it was open
        if (HUDController.Instance != null)
            HUDController.Instance.ToggleShopUI(false);

        CancelInvoke("StartNextWave");

        // 1. Clean up any leftover enemies from last wave
        ClearActiveEntities();

        // 2. Increment Wave
        currentWave++;
        difficultyScale = Mathf.Min(maxDifficulty, 1.0f + (currentWave * 0.15f));
        currentState = GameState.Playing;

        // 3. Update HUD
        if (HUDController.Instance != null)
            HUDController.Instance.UpdateWaveUI(currentWave);

        // 4. Trigger Spawner
        WaveSpawner.Instance.SpawnWave(currentWave);
    }

    public void EnemyDefeated()
    {
        enemiesRemaining--;
        Debug.Log($"Entity Cleared. {enemiesRemaining} left in wave.");

        // Check if wave is over
        if (enemiesRemaining <= 0 && !isSpawning && currentState == GameState.Playing)
        {
            EndWave();
        }
    }

    void EndWave()
    {
        currentState = GameState.Shop;

        // Open the Shop automatically between waves
        if (HUDController.Instance != null)
            HUDController.Instance.ToggleShopUI(true);
    }

    // --- SHOP UPGRADES ---

    public void UpgradeGlobalSpeed()
    {
        int cost = 100 + (speedUpgradeLevel * 50);
        if (currentCoins >= cost)
        {
            currentCoins -= cost;
            speedUpgradeLevel++;
            UpdateEconomy();
            Debug.Log("Fire Rate Upgraded!");
        }
    }

    public void UpgradeGlobalDamage()
    {
        int cost = 100 + (damageUpgradeLevel * 50);
        if (currentCoins >= cost)
        {
            currentCoins -= cost;
            damageUpgradeLevel++;
            UpdateEconomy();
            Debug.Log("Bullet Damage Upgraded!");
        }
    }

    public void BuyFortressHealth()
    {
        int cost = 50;
        if (currentCoins >= cost)
        {
            currentCoins -= cost;
            StackManager.Instance.fortressHealth += 20;
            UpdateEconomy();
        }
    }

    private void UpdateEconomy()
    {
        if (HUDController.Instance != null)
            HUDController.Instance.UpdateEconomyUI(currentCoins);
    }

    // --- UTILITY ---

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateEconomy();
    }

    public void TriggerGameOver()
    {
        currentState = GameState.GameOver;
        Time.timeScale = 0;
        if (HUDController.Instance != null)
            HUDController.Instance.gameOverPanel.SetActive(true);
    }

    public void TogglePause()
    {

        if (currentState == GameState.Playing)
        {
            currentState = GameState.Paused;
            Time.timeScale = 0;
            HUDController.Instance.pausePanel.SetActive(true);
        }
        else if (currentState == GameState.Paused)
        {
            currentState = GameState.Playing;
            Time.timeScale = 1;
            HUDController.Instance.pausePanel.SetActive(false);
        }
    }

    public void ClearActiveEntities()
    {
        // Remove entities currently in the field (walking/kneeling)
        EntityBase[] entities = FindObjectsOfType<EntityBase>();
        foreach (EntityBase e in entities)
        {
            // If they are in the activeTargets list, they are not in the stack yet
            if (TypingManager.Instance.activeTargets.Contains(e))
            {
                Destroy(e.gameObject);
            }
        }

        enemiesRemaining = 0;
        TypingManager.Instance.ResetTyping();
    }

    public void Reload() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void KillApp() => Application.Quit();
}