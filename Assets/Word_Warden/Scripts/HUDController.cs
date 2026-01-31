using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance;

    [Header("Top HUD")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI waveText;

    [Header("Overlay & Panels")]
    public GameObject shopPanel;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject waveClearOverlay;

    [Header("Typing HUD")]
    public TextMeshProUGUI inputFieldText; // The text at the bottom center of the screen

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Start()
    {
        // Set initial states
        if (shopPanel != null) shopPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (waveClearOverlay != null) waveClearOverlay.SetActive(false);

        // Sync with GameManager values on start
        if (GameManager.Instance != null)
        {
            UpdateEconomyUI(GameManager.Instance.currentCoins);
            UpdateWaveUI(GameManager.Instance.currentWave);
        }
    }

    // This shows the player exactly what they've typed so far for their current target
    public void UpdateTypingUI(string currentInput)
    {
        if (inputFieldText == null) return;

        inputFieldText.text = currentInput;

        // Visual Polish: Only show the UI element if there's actually something typed
        // This keeps the screen clean when the player isn't actively targeting someone
        inputFieldText.gameObject.SetActive(!string.IsNullOrEmpty(currentInput));
    }

    public void ToggleShopUI(bool isOpen)
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(isOpen);

            // Manage the mouse cursor for shop navigation
            Cursor.visible = isOpen;
            Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;

            // Time Management: Usually, you'll want to pause the game while shopping
            // Time.timeScale = isOpen ? 0 : 1; 
        }
    }

    public void ShowWaveClear(bool show)
    {
        if (waveClearOverlay != null)
            waveClearOverlay.SetActive(show);
    }

    public void UpdateEconomyUI(int coins)
    {
        if (coinText != null) coinText.text = "Coins: " + coins;
    }

    public void UpdateWaveUI(int wave)
    {
        if (waveText != null) waveText.text = "Wave: " + wave;
    }
}