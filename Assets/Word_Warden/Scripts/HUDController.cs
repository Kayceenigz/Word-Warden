using UnityEngine;
using UnityEngine.UI; // Added for Mask Icons (Images)
using TMPro;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance;

    [Header("Top HUD")]
    public TextMeshProUGUI waveText;
    // Note: coinText removed as per your new stat-focused design, 
    // but keep it if you still use currency for something else.

    [Header("Overlay & Stat Panels")]
    public GameObject statScreenPanel; // Formerly Shop Panel
    public GameObject gameOverPanel;
    public GameObject pausePanel;

    [Header("Wave Results (Stat Screen)")]
    public TextMeshProUGUI accuracyResultText;
    public TextMeshProUGUI correctKeysText;
    public TextMeshProUGUI typosText;

    [Header("Mask Inventory UI")]
    public Image difficultyMaskIcon; // Dim these by default, light up when collected
    public Image speedMaskIcon;
    public Image healthMaskIcon;

    [Header("Typing HUD")]
    public TextMeshProUGUI inputFieldText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Start()
    {
        // Initial States
        if (statScreenPanel != null) statScreenPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        // Initialize Mask Icons to look "Locked" (Semi-transparent)
        InitializeMaskUI();

        if (GameManager.Instance != null)
        {
            UpdateWaveUI(GameManager.Instance.currentWave);
        }
    }

    // --- STAT SCREEN LOGIC ---

    public void DisplayWaveResults(int correct, int errors, float accuracy)
    {
        if (correctKeysText != null) correctKeysText.text = correct.ToString();
        if (typosText != null) typosText.text = errors.ToString();
        if (accuracyResultText != null) accuracyResultText.text = accuracy.ToString("F1") + "%";
    }

    public void ToggleStatScreen(bool isOpen)
    {
        if (statScreenPanel != null)
        {
            statScreenPanel.SetActive(isOpen);

            // Manage cursor for clicking the "Next Wave" button
            Cursor.visible = isOpen;
            Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    // --- MASK UI LOGIC ---

    private void InitializeMaskUI()
    {
        // Start with icons grayed out or low alpha
        if (difficultyMaskIcon) difficultyMaskIcon.color = new Color(1, 1, 1, 0.2f);
        if (speedMaskIcon) speedMaskIcon.color = new Color(1, 1, 1, 0.2f);
        if (healthMaskIcon) healthMaskIcon.color = new Color(1, 1, 1, 0.2f);
    }

    public void UpdateMaskIcons()
    {
        // Light up icons if the player has collected them
        if (GameManager.Instance.hasDifficultyMask) difficultyMaskIcon.color = Color.white;
        if (GameManager.Instance.hasSpeedMask) speedMaskIcon.color = Color.white;
        if (GameManager.Instance.hasHealthMask) healthMaskIcon.color = Color.white;
    }

    // --- CORE HUD UPDATES ---

    public void UpdateTypingUI(string currentInput)
    {
        if (inputFieldText == null) return;
        inputFieldText.text = currentInput;
        inputFieldText.gameObject.SetActive(!string.IsNullOrEmpty(currentInput));
    }

    public void UpdateWaveUI(int wave)
    {
        if (waveText != null) waveText.text = "Wave: " + wave;
    }

    // Logic for other panels remains consistent
    public void TogglePause(bool isOpen)
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(isOpen);
            Cursor.visible = isOpen;
            Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    public void ToggleGameOver(bool isOpen)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(isOpen);
            Cursor.visible = isOpen;
            Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}