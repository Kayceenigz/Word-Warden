using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Added Intermission state for the stat screen
    public enum GameState { Menu, Playing, Intermission, Paused, GameOver }
    public GameState currentState;

    [Header("Wave Settings")]
    public int currentWave = 0;
    public int enemiesRemaining;
    public float difficultyScale = 1.0f;
    public float maxDifficulty = 3.0f;

    [Header("Mask Abilities (Active Buffs)")]
    public bool hasDifficultyMask = false; // Reduces difficulty scale by 10%
    public bool hasSpeedMask = false;      // Reduces zombie speed by 10%
    public bool hasHealthMask = false;     // Increases Fortress Health

    [HideInInspector] public bool isSpawning = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Time.timeScale = 1;
    }

    void Start()
    {
        if (HUDController.Instance != null)
            HUDController.Instance.UpdateWaveUI(currentWave);

        Invoke("StartNextWave", 2f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }

    public void StartNextWave()
    {
        // Close Stat Screen/Intermission UI
        if (HUDController.Instance != null)
            HUDController.Instance.ToggleStatScreen(false);

        CancelInvoke("StartNextWave");
        ClearActiveEntities();

        // Reset Typing Stats for the new wave
        TypingManager.Instance.ResetStats();

        currentWave++;

        // --- APPLY MASK ABILITY: Difficulty Reduction ---
        float baseDifficulty = 1.0f + (currentWave * 0.15f);
        difficultyScale = hasDifficultyMask ? baseDifficulty * 0.9f : baseDifficulty;
        difficultyScale = Mathf.Min(maxDifficulty, difficultyScale);

        currentState = GameState.Playing;

        if (HUDController.Instance != null)
            HUDController.Instance.UpdateWaveUI(currentWave);

        WaveSpawner.Instance.SpawnWave(currentWave);
    }

    public void EnemyDefeated()
    {
        enemiesRemaining--;

        // Check for stragglers
        EntityBase[] stragglers = FindObjectsOfType<EntityBase>();
        int actualActiveEnemies = 0;
        foreach (var e in stragglers)
        {
            if (TypingManager.Instance.activeTargets.Contains(e)) actualActiveEnemies++;
        }

        if ((enemiesRemaining <= 0 || actualActiveEnemies == 0) && !isSpawning)
        {
            EndWave();
        }
    }

    void EndWave()
    {
        currentState = GameState.Intermission;

        // CALCULATE STATS
        int correct = TypingManager.Instance.totalCorrectKeystrokes;
        int errors = TypingManager.Instance.totalTypos;
        int total = correct + errors;

        float accuracy = total > 0 ? ((float)correct / total) * 100f : 0f;

        // Display Stats on HUD
        if (HUDController.Instance != null)
        {
            HUDController.Instance.DisplayWaveResults(correct, errors, accuracy);
            HUDController.Instance.ToggleStatScreen(true);
        }
    }

    // --- MASK COLLECTION SYSTEM ---
    // This will be called by your Mask Pickups
    public void CollectMask(int maskType)
    {
        switch (maskType)
        {
            case 0: // Difficulty Mask
                hasDifficultyMask = true;
                break;
            case 1: // Speed Mask
                hasSpeedMask = true;
                break;
            case 2: // Health Mask
                hasHealthMask = true;
                // Call the new StackManager function to increase max health
                StackManager.Instance.IncreaseMaxHealth(50f);
                break;
        }

        if (HUDController.Instance != null)
            HUDController.Instance.UpdateMaskIcons();
    }

    public void TriggerGameOver()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        currentState = GameState.GameOver;
        Time.timeScale = 0;
        if (HUDController.Instance != null)
            HUDController.Instance.gameOverPanel.SetActive(true);
    }

    public void TogglePause()
    {
        if (currentState == GameState.Playing)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
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
        EntityBase[] entities = FindObjectsOfType<EntityBase>();
        foreach (EntityBase e in entities)
        {
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