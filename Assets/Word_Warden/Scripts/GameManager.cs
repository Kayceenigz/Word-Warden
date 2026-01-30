using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState { Menu, Playing, Shop, Paused, GameOver }
    public GameState currentState;

    [Header("Economy")]
    public int currentCoins = 0;
    public int comboMultiplier = 1;
    public int currentWave = 1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartWave();
    }

    void Update()
    {
        // GDD: ESC to Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void StartWave()
    {
        currentState = GameState.Playing;
    }

    public void AddCoins(int amount)
    {
        // GDD: 10 x word length + bonuses (simplified here)
        currentCoins += amount * comboMultiplier;

        // Update HUD if it exists
        if (HUDController.Instance != null)
            HUDController.Instance.UpdateEconomyUI(currentCoins, comboMultiplier);
    }

    public void TriggerGameOver()
    {
        currentState = GameState.GameOver;
        Time.timeScale = 0; // Freeze game
        Debug.Log("GAME OVER: The Fortress has fallen.");
    }

    void TogglePause()
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