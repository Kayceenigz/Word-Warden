using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance;

    [Header("Typing HUD")]
    public TextMeshProUGUI inputFieldText;
    public TextMeshProUGUI ghostText;

    [Header("Top HUD")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI comboText; // Note: This will stay empty since we removed combo

    [Header("Overlay & Shop")]
    public GameObject waveClearOverlay;
    public GameObject shopPanel; // ADD THIS in the Inspector
    public GameObject gameOverPanel;
    public GameObject pausePanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Start()
    {
        if(shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    // --- SHOP LOGIC ---
    public void ToggleShopUI(bool isOpen)
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(isOpen);

            // Allow the player to use the mouse to click upgrade buttons
            Cursor.visible = isOpen;
            Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    public void ShowWaveClear(bool show)
    {
        if (waveClearOverlay != null)
            waveClearOverlay.SetActive(show);
    }

    public void UpdateTypingUI(string currentInput)
    {
        if (inputFieldText != null)
            inputFieldText.text = currentInput;

        UpdateGhosts();
    }

    public void UpdateGhosts()
    {
        if (ghostText == null || TypingManager.Instance == null) return;

        string ghosts = "Targets: ";
        foreach (var target in TypingManager.Instance.activeTargets)
        {
            ghosts += target.GetWord() + "  ";
        }
        ghostText.text = ghosts;
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