using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState { Menu, Playing, Shop, Intermission, Paused, GameOver }
    public GameState currentState;

    [Header("Economy")]
    public int currentCoins = 0;

    [Header("Wave Settings")]
    public int currentWave = 0;
    public int enemiesRemaining;
    public float difficultyScale = 1.0f;
    public float maxDifficulty = 3.0f; // Prevent the game from becoming impossible

    public GameObject shopPanel;

    [Header("Upgrades")]
    public int speedUpgradeLevel = 0;
    public int coinBonusLevel = 0;

    public void UpgradeRecruitSpeed()
    {
        if (currentCoins >= 100)
        {
            currentCoins -= 100;
            speedUpgradeLevel++;
            HUDController.Instance.UpdateEconomyUI(currentCoins);
        }
    }

    public void ToggleShopUI(bool isOpen)
    {
        shopPanel.SetActive(isOpen);

        // Unlock/Lock cursor so player can click buttons
        Cursor.visible = isOpen;
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    // Example Shop Button Logic
    public void BuyFortressHealth()
    {
        if (GameManager.Instance.currentCoins >= 50)
        {
            GameManager.Instance.currentCoins -= 50;
            StackManager.Instance.fortressHealth += 20;
            HUDController.Instance.UpdateEconomyUI(currentCoins);
        }
    }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Give the player 2 seconds to get ready before Wave 1
        Invoke("StartNextWave", 2f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }

    public void StartNextWave()
    {
        HUDController.Instance.shopPanel.SetActive(false);
        CancelInvoke("StartNextWave");

        // CLEANUP FIRST: Wipe the slate clean
        ClearActiveEntities();

        currentWave++;
        difficultyScale = Mathf.Min(maxDifficulty, 1.0f + (currentWave * 0.15f));
        currentState = GameState.Playing;

        if (HUDController.Instance != null)
            HUDController.Instance.UpdateWaveUI(currentWave);

        WaveSpawner.Instance.SpawnWave(currentWave);
    }

    public void EnemyDefeated()
    {
        enemiesRemaining--;

        // Debug check: This helps you find which prefab is missing the call
        Debug.Log($"Enemy Down. {enemiesRemaining} left in Wave {currentWave}");

        if (enemiesRemaining <= 0 && currentState == GameState.Playing)
        {
            EndWave();
        }
    }

    void EndWave()
    {
        currentState = GameState.Shop; // Set state to Shop

        // Show the Shop UI instead of a timer
        if (HUDController.Instance != null)
            HUDController.Instance.ToggleShopUI(true);
    }

    // THIS IS CALLED BY YOUR BUTTON
    public void OnReadyButtonPressed()
    {
        if (HUDController.Instance != null)
            HUDController.Instance.ToggleShopUI(false);

        StartNextWave();
    }

    void HideWaveUI()
    {
        if (HUDController.Instance != null)
            HUDController.Instance.ShowWaveClear(false);
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        if (HUDController.Instance != null)
            HUDController.Instance.UpdateEconomyUI(currentCoins);
    }

    public void TriggerGameOver()
    {
        HUDController.Instance.gameOverPanel.SetActive(true);
        currentState = GameState.GameOver;
        Time.timeScale = 0;
        Debug.Log("GAME OVER: Base Destroyed.");
        // Programmer A: Show Game Over Screen here
    }

    public void Reload()
    {
        Debug.Log("Reloading Game");
        SceneManager.LoadScene("Level");
    }
    public void KillApp()
    {
        Debug.Log("This game has quit.");
        Application.Quit();
    }

    public void ClearActiveEntities()
    {
        // Find every Zombie and Survivor currently in the scene
        EntityBase[] remainingEntities = FindObjectsOfType<EntityBase>();

        foreach (EntityBase entity in remainingEntities)
        {
            // Only clear entities that AREN'T already in your stack
            // We check this by seeing if they are still 'typeable'
            if (TypingManager.Instance.activeTargets.Contains(entity))
            {
                // Use the Die logic to ensure they unregister from the TypingManager
                Destroy(entity.gameObject);
            }
        }

        // Reset the counter just to be safe
        enemiesRemaining = 0;

        // Clear the player's current typing progress so they start fresh
        TypingManager.Instance.currentInput = "";
    }

    public void TogglePause()
    {
        if (currentState == GameState.Playing)
        {
            currentState = GameState.Paused;
            Time.timeScale = 0;
        }
        else if (currentState == GameState.Paused)
        {
            currentState = GameState.Playing;
            Time.timeScale = 1;
        }
    }
}